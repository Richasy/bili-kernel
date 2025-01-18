// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Bilibili.App.Playeronline.V1;
using Bilibili.App.Playurl.V1;
using Bilibili.App.View.V1;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core.Models;
using RichasyKernel;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class PlayerClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BiliAuthenticator _authenticator;
    private readonly IBiliTokenResolver _tokenResolver;

    public PlayerClient(
        BiliHttpClient httpClient,
        BiliAuthenticator authenticator,
        IBiliTokenResolver tokenResolver)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
        _tokenResolver = tokenResolver;
    }

    public async Task<VideoPlayerView> GetVideoPageDetailWithGrpcAsync(MediaIdentifier video, CancellationToken cancellationToken)
    {
        var idType = GetVideoIdType(video.Id, out var videoId);
        if (string.IsNullOrEmpty(idType))
        {
            throw new ArgumentOutOfRangeException(nameof(video), "无法识别的视频 ID");
        }

        var viewReq = new ViewReq();
        if (idType == "av")
        {
            viewReq.Aid = Convert.ToInt64(videoId);
        }
        else if (idType == "bv")
        {
            viewReq.Bvid = videoId;
        }

        viewReq.Fnval = 16;
        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Video.DetailGrpc), viewReq);
        _authenticator.AuthorizeGrpcRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, ViewReply.Parser).ConfigureAwait(false)
            ?? throw new KernelException("解析视频详情失败");

        IList<VideoSeason>? seasons = default;
        // GRPC 接口中不包含合集信息，需要通过 Web 接口获取.
        if (responseObj.Arc.SeasonId != 0)
        {
            var pageResponse = await GetWebVideoPageResponseAsync(responseObj.Arc.Aid.ToString(), cancellationToken).ConfigureAwait(false);
            seasons = pageResponse.View.ugc_season?.sections?.Select(p => p.ToVideoSeason()).ToList();
        }

        return responseObj.ToVideoPlayerView(seasons);
    }

    public async Task<VideoPlayerView> GetVideoPageDetailWithRestAsync(MediaIdentifier video, CancellationToken cancellationToken)
    {
        var idType = GetVideoIdType(video.Id, out var videoId);
        if (string.IsNullOrEmpty(idType))
        {
            throw new ArgumentOutOfRangeException(nameof(video), "无法识别的视频 ID");
        }

        var queryParameters = new Dictionary<string, string>();
        if (idType == "av")
        {
            queryParameters.Add("aid", videoId);
        }
        else if (idType == "bv")
        {
            queryParameters.Add("bvid", videoId);
        }

        var pageResponse = await GetWebVideoPageResponseAsync(videoId, cancellationToken).ConfigureAwait(false);
        var progress = await GetWebVideoProgressAsync(pageResponse.View.aid.ToString(), pageResponse.View.cid.ToString(), cancellationToken).ConfigureAwait(false);
        var operation = await GetWebVideoOperationInformationAsync(pageResponse.View.aid.ToString(), cancellationToken).ConfigureAwait(false);
        return pageResponse.ToVideoPlayerView(progress, operation);
    }

    public async Task<DashMediaInformation> GetVideoPlayDetailWithRestAsync(MediaIdentifier video, long cid, CancellationToken cancellationToken)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { "fnver", "0" },
            { "cid", cid.ToString() },
            { "fourk", "1" },
            { "fnval", "4048" },
            { "qn", "64" },
            { "oType", "json" },
            { "avid", video.Id },
            { "mid", _tokenResolver.GetToken().UserId.ToString() },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Video.PlayInformation));
        _authenticator.AuthorizeRestRequest(request, queryParameters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponsePlayerInformation).ConfigureAwait(false)
            ?? throw new KernelException("解析视频播放详情失败");
        return responseObj.Data.ToDashMediaInformation();
    }

    public async Task<DashMediaInformation> GetVideoPlayDetailWithGrpcAsync(MediaIdentifier video, long cid, CancellationToken cancellationToken)
    {
        var req = new PlayViewReq();
        req.Aid = Convert.ToInt64(video.Id);
        req.Cid = cid;
        req.Fnver = 0;
        req.Fnval = 4048;
        req.Fourk = true;
        req.Qn = 112;
        req.PreferCodecType = CodeType.Code264;

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Video.PlayUrl), req);
        _authenticator.AuthorizeGrpcRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, PlayViewReply.Parser).ConfigureAwait(false)
            ?? throw new KernelException("解析视频播放详情失败");

        return responseObj.ToDashMediaInformation();
    }

    public async Task<LivePlayerView> GetLivePageDetailAsync(MediaIdentifier live, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "room_id", live.Id },
            { "device", "phone" },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Live.RoomDetail));
        _authenticator.AuthorizeRestRequest(request, parameters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponseLiveRoomDetailResponse).ConfigureAwait(false)
            ?? throw new KernelException("解析直播间详情失败");
        return responseObj.Data.ToLivePlayerView();
    }

    public async Task<LiveMediaInformation> GetLivePlayDetailAsync(MediaIdentifier live, int quality, bool audioOnly, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "room_id", live.Id },
            { "no_playurl", "0" },
            { "qn", quality.ToString() },
            { "codec", "0,1,2" },
            { "dolby", "5" },
            { "format", "0,1,2" },
            { "only_audio", audioOnly ? "1" : "0" },
            { "protocol", "0,1" },
            { "mask", "1" },
            { "panorama", "1" },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Live.WebPlayInformation));
        _authenticator.AuthorizeRestRequest(request, parameters, new BiliAuthorizeExecutionSettings { NeedCSRF = true, ForceNoToken = true, RequireCookie = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponseLiveAppPlayResponse).ConfigureAwait(false)
            ?? throw new KernelException("解析直播播放详情失败");

        return responseObj.Data.ToLiveMediaInformation();
    }

    public async Task<PgcPlayerView> GetPgcPageDetailAsync(string? seasonId, string? episodeId, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "autoplay", "0" },
            { "is_show_all_series", "0" },
        };

        if (!string.IsNullOrEmpty(episodeId))
        {
            parameters.Add("ep_id", episodeId);
        }

        if (!string.IsNullOrEmpty(seasonId))
        {
            parameters.Add("season_id", seasonId);
        }

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Pgc.SeasonDetail()));
        _authenticator.AuthorizeRestRequest(request, parameters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponsePgcDetailResponse).ConfigureAwait(false)
            ?? throw new KernelException("解析PGC详情失败");
        return responseObj.Data.ToPgcPlayerView();
    }

    public async Task<DashMediaInformation> GetPgcPlayDetailAsync(string? cid, string? episodeId, string seasonType, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "fnver", "0" },
            { "cid", cid.ToString() },
            { "fourk", "1" },
            { "fnval", "4048" },
            { "qn", "64" },
            { "oType", "json" },
            { "module", "bangumi" },
            { "season_type", seasonType },
            { "ep_id", episodeId },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Pgc.PlayInformation()));
        _authenticator.AuthorizeRestRequest(request, parameters, new BiliAuthorizeExecutionSettings { ApiType = BiliApiType.Web });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliResultResponsePlayerInformation).ConfigureAwait(false)
            ?? throw new KernelException("解析PGC播放详情失败");

        return responseObj.Result.ToDashMediaInformation();
    }

    public async Task<OnlineViewer> GetOnlineViewerAsync(string aid, string cid, CancellationToken cancellationToken)
    {
        var req = new PlayerOnlineReq
        {
            Aid = Convert.ToInt64(aid),
            Cid = Convert.ToInt64(cid),
            PlayOpen = true
        };
        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Video.OnlineViewerCount), req);
        _authenticator.AuthorizeGrpcRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, PlayerOnlineReply.Parser).ConfigureAwait(false)
            ?? throw new KernelException("解析在线观看人数失败");
        return new OnlineViewer(Convert.ToInt32(responseObj.TotalNumber), responseObj.TotalText);
    }

    public async Task ToggleVideoLikeAsync(string aid, bool isLike, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "aid", aid },
            { "like", isLike ? "0" : "1" },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Video.Like));
        _authenticator.AuthorizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task CoinVideoAsync(string aid, int number, bool alsoLike, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "aid", aid },
            { "multiply", number.ToString() },
        };

        if (alsoLike)
        {
            parameters.Add("select_like", "1");
        }

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Video.Coin));
        _authenticator.AuthorizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task FavoriteVideoAsync(string aid, IList<string> favoriteIds, IList<string> unfavoriteIds, bool isVideo, CancellationToken cancellationToken)
    {
        var resourceId = isVideo ? "2" : "24";
        var parameters = new Dictionary<string, string>
        {
            { "resources", $"{aid}:{resourceId}" },
        };

        if (favoriteIds?.Count > 0)
        {
            parameters.Add("add_media_ids", string.Join(",", favoriteIds));
        }

        if (unfavoriteIds?.Count > 0)
        {
            parameters.Add("del_media_ids", string.Join(",", unfavoriteIds));
        }

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Video.ModifyFavorite));
        _authenticator.AuthorizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task TripleAsync(string aid, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "aid", aid },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Video.Triple));
        _authenticator.AuthorizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task ReportProgressAsync(string aid, string cid, int progress, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "aid", aid },
            { "cid", cid },
            { "progress", progress.ToString() },
            { "type", "3" },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Video.ProgressReport));
        _authenticator.AuthorizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task ReportProgressAsync(string aid, string cid, string episodeId, string seasonId, int progress, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "aid", aid },
            { "cid", cid },
            { "epid", episodeId },
            { "sid", seasonId },
            { "progress", progress.ToString() },
            { "type", "4" },
            { "sub_type", "1" },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Video.ProgressReport));
        _authenticator.AuthorizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<VideoOperationInformation> GetEpisodeOpeartionInformationAsync(string episodeId, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "ep_id", episodeId },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Pgc.EpisodeInteraction));
        _authenticator.AuthorizeRestRequest(request, parameters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponseEpisodeInteractionResponse).ConfigureAwait(false);
        return new VideoOperationInformation(episodeId, responseObj.Data.IsLike == 1, responseObj.Data.CoinNumber > 0, responseObj.Data.IsFavorite == 1);
    }

    public async Task<ClientWebSocket> EnterLiveRoomAsync(string roomId, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "room_id", roomId },
            { "actionKey", "appkey" },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Live.EnterRoom));
        _authenticator.AuthorizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request).ConfigureAwait(false);
        var liveDanmakuInfo = await GetLiveDanmakuInfoAsync(roomId).ConfigureAwait(false);
        var socket = CreateLiveSocket();
        var host = liveDanmakuInfo.HostList.FirstOrDefault()?.Host;
        if (string.IsNullOrEmpty(host))
        {
            throw new KernelException("无法获取直播弹幕服务器地址");
        }

        await socket.ConnectAsync(new Uri($"wss://{host}/sub"), cancellationToken).ConfigureAwait(false);
        if (socket.State != WebSocketState.Open)
        {
            throw new KernelException("连接直播弹幕服务器失败");
        }

        var buvid = await GetBuvidAsync(cancellationToken).ConfigureAwait(false);
        await SendLiveMessageAsync(
            socket,
            new EnterRoomMessage
            {
                roomid = Convert.ToInt32(roomId),
                uid = _tokenResolver.GetToken().UserId,
                buvid = buvid,
                protover = 2,
                key = liveDanmakuInfo.Token,
            },
            7,
            cancellationToken).ConfigureAwait(false);
        await SendLiveHeartBeatAsync(socket, cancellationToken).ConfigureAwait(false);
        return socket;
    }

    public static Task SendLiveHeartBeatAsync(ClientWebSocket socket, CancellationToken cancellationToken)
        => SendLiveMessageAsync(socket, string.Empty, 2, cancellationToken);

    public static async Task SendLiveMessageAsync(ClientWebSocket socket, object data, int action, CancellationToken cancellationToken)
    {
        var msg = data is string str ? str : string.Empty;
        if (data is EnterRoomMessage erm)
        {
            msg = JsonSerializer.Serialize(erm, SourceGenerationContext.Default.EnterRoomMessage);
        }

        var msgData = EncodeLiveData(msg, action);
        await socket.SendAsync(new ArraySegment<byte>(msgData), WebSocketMessageType.Binary, true, cancellationToken).ConfigureAwait(false);
    }

    public static async Task<IReadOnlyList<LiveMessage>?> GetLiveSocketMessagesAsync(ClientWebSocket socket, CancellationToken cancellationToken)
    {
        if (socket.State != WebSocketState.Open)
        {
            return default;
        }

        var buffer = new byte[4096];
        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken).ConfigureAwait(false);
        return result.MessageType == WebSocketMessageType.Close
            ? default
            : ParseLiveData(buffer);
    }

    private static IReadOnlyList<LiveMessage>? ParseLiveData(byte[] data)
    {
        // 协议版本。
        // 0为JSON，可以直接解析；
        // 1为房间人气值,Body为Int32；
        // 2为压缩过Buffer，需要解压再处理
        var protocolVersion = BitConverter.ToInt32(new byte[4] { data[7], data[6], 0, 0 }, 0);

        // 操作类型。
        // 3=心跳回应，内容为房间人气值；
        // 5=通知，弹幕、广播等全部信息；
        // 8=进房回应，空
        var operation = BitConverter.ToInt32(data.Skip(8).Take(4).Reverse().ToArray(), 0);

        // 内容
        var body = data.Skip(16).ToArray();
        if (operation == 8)
        {
            return [new LiveMessage(LiveMessageType.ConnectSuccess, "弹幕连接成功")];
        }
        else if (operation == 3)
        {
            body.Reverse();
            var online = BitConverter.ToInt32(body, 0);
            return [new LiveMessage(LiveMessageType.Online, online)];
        }
        else if (operation == 5)
        {
            if (protocolVersion == 2)
            {
                body = DecompressData(body);
            }

            var text = Encoding.UTF8.GetString(body);

            // 可能有多条数据，做个分割
            var textLines = Regex.Split(text, "[\x00-\x1f]+").Where(x => x.Length > 2 && x[0] == '{').ToArray();
            var messages = new List<LiveMessage>();
            foreach (var item in textLines)
            {
                var m = ParseMessage(item);
                if (m != null)
                {
                    messages.Add(m);
                }
            }

            return messages;
        }

        return default;
    }

    private static LiveMessage? ParseMessage(string jsonMessage)
    {
        try
        {
#if DEBUG
            Debug.WriteLine(jsonMessage);
#endif
            if (!jsonMessage.EndsWith("}"))
            {
                return default;
            }

            var doc = JsonDocument.Parse(jsonMessage);
            var obj = doc.RootElement;
            var cmd = obj.GetProperty("cmd").ToString();
            if (cmd.Contains("DANMU_MSG"))
            {
                var msg = new LiveDanmakuMessage();
                if (obj.TryGetProperty("info", out var info) && info.GetArrayLength() != 0)
                {
                    var array = info.EnumerateArray().ToArray();
                    msg.Text = array.ElementAt(1).ToString();
                    if (array[2].GetArrayLength() != 0)
                    {
                        msg.UserName = array[2][1].ToString() + ":";

                        if (Convert.ToInt32(array[2][3].ToString()) == 1)
                        {
                            msg.VipText = "老爷";
                            msg.IsVip = true;
                        }

                        if (Convert.ToInt32(array[2][4].ToString()) == 1)
                        {
                            msg.VipText = "年费老爷";
                            msg.IsVip = false;
                            msg.IsBigVip = true;
                        }

                        if (Convert.ToInt32(array[2][2].ToString()) == 1)
                        {
                            msg.VipText = "房管";
                            msg.IsAdmin = true;
                        }
                    }

                    if (array[3].GetArrayLength() != 0)
                    {
                        msg.MedalName = array[3][1].ToString();
                        msg.MedalLevel = array[3][0].ToString();
                        msg.MedalColor = array[3][4].ToString();
                        msg.HasMedal = true;
                    }

                    if (array[4].GetArrayLength() != 0)
                    {
                        msg.Level = "UL" + array[4][0].ToString();
                        msg.LevelColor = array[4][2].ToString();
                    }

                    if (array[5].GetArrayLength() != 0)
                    {
                        msg.UserTitle = array[5][0].ToString();
                        msg.HasTitle = true;
                    }

                    var danmakuInfo = new LiveDanmakuInformation(msg.Text, msg.ContentColor, msg.UserName, msg.Level, msg.LevelColor, msg.IsAdmin);
                    return new LiveMessage(LiveMessageType.Danmaku, danmakuInfo);
                }
            }
        }
        catch (Exception ex)
        {
            // TODO: 记录错误.
            Debug.WriteLine(ex.Message);
        }

        return default;
    }

    private static string GetVideoIdType(string id, out string newId)
    {
        newId = null;
        if (id.StartsWith("bv", System.StringComparison.InvariantCultureIgnoreCase))
        {
            newId = id;
            return "bv";
        }
        else
        {
            id = id.ToLowerInvariant().Replace("av", string.Empty);
            if (long.TryParse(id, out _))
            {
                newId = id;
                return "av";
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 对数据进行编码.
    /// </summary>
    /// <param name="msg">文本内容.</param>
    /// <param name="action">2=心跳，7=进房.</param>
    /// <returns>编码后的数据.</returns>
    private static byte[] EncodeLiveData(string msg, int action)
    {
        var data = Encoding.UTF8.GetBytes(msg);

        // 头部长度固定16
        var length = data.Length + 16;
        var buffer = new byte[length];
        using var ms = new MemoryStream(buffer);

        // 数据包长度
        var b = BitConverter.GetBytes(buffer.Length).ToArray();
        b.Reverse();
        ms.Write(b, 0, 4);

        // 数据包头部长度,固定16
        b = BitConverter.GetBytes(16);
        b.Reverse();
        ms.Write(b, 2, 2);

        // 协议版本，0=JSON,1=Int32,2=Buffer
        b = BitConverter.GetBytes(0);
        b.Reverse();
        ms.Write(b, 0, 2);

        // 操作类型
        b = BitConverter.GetBytes(action);
        b.Reverse();
        ms.Write(b, 0, 4);

        // 数据包头部长度,固定1
        b = BitConverter.GetBytes(1);
        b.Reverse();
        ms.Write(b, 0, 4);

        // 数据
        ms.Write(data, 0, data.Length);

        var bytes = ms.ToArray();
        ms.Flush();
        return bytes;
    }

    /// <summary>
    /// 解压直播数据.
    /// </summary>
    /// <param name="data">数据.</param>
    /// <returns>解压后的数据.</returns>
    private static byte[] DecompressData(byte[] data)
    {
        using var outBuffer = new MemoryStream();
        using var compressedZipStream = new DeflateStream(new MemoryStream(data, 2, data.Length - 2), CompressionMode.Decompress);
        var block = new byte[1024];
        while (true)
        {
            var bytesRead = compressedZipStream.Read(block, 0, block.Length);
            if (bytesRead <= 0)
            {
                break;
            }
            else
            {
                outBuffer.Write(block, 0, bytesRead);
            }
        }

        compressedZipStream.Close();
        return outBuffer.ToArray();
    }

    private static ClientWebSocket CreateLiveSocket()
    {
        var socket = new ClientWebSocket();
        socket.Options.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.69");
        return socket;
    }

    private async Task<string> GetBuvidAsync(CancellationToken cancellationToken)
    {
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Account.Buvid));
        _authenticator.AuthorizeRestRequest(request, default, new BiliAuthorizeExecutionSettings { RequireCookie = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponseBuvidResponse).ConfigureAwait(false);
        return responseObj.Data.B3;
    }

    private async Task<LiveDanmakuResponse> GetLiveDanmakuInfoAsync(string roomId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { "id", roomId },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Live.WebDanmakuInformation));
        _authenticator.AuthorizeRestRequest(request, queryParameters, new BiliAuthorizeExecutionSettings { RequireCookie = true });
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
        var danmakuRes = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponseLiveDanmakuResponse).ConfigureAwait(false);
        return danmakuRes.Data;
    }

    private async Task<PlayerProgress?> GetWebVideoProgressAsync(string aid, string cid, CancellationToken cancellationToken)
    {
        try
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "aid", aid },
                { "cid", cid },
            };
            var wbiRequest = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Video.PlayerWbi));
            _authenticator.AuthorizeRestRequest(wbiRequest, queryParameters, new BiliAuthorizeExecutionSettings { ApiType = BiliApiType.Web, ForceNoToken = true, RequireCookie = true, NeedRID = true });
            var wbiResponse = await _httpClient.SendAsync(wbiRequest, cancellationToken).ConfigureAwait(false);
            var wbiResponseObj = await BiliHttpClient.ParseAsync(wbiResponse, SourceGenerationContext.Default.BiliDataResponsePlayerWbiResponse).ConfigureAwait(false);
            return wbiResponseObj.Data.ToPlayerProgress();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        return default;
    }

    private async Task<VideoOperationInformation?> GetWebVideoOperationInformationAsync(string aid, CancellationToken cancellationToken)
    {
        try
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "aid", aid },
            };

            var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Video.WebVideoActions));
            _authenticator.AuthorizeRestRequest(request, queryParameters, new BiliAuthorizeExecutionSettings { ApiType = BiliKernel.Models.BiliApiType.Web, ForceNoToken = true, RequireCookie = true, NeedRID = true });
            var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponseArchiveRelationResponse).ConfigureAwait(false);
            return responseObj.Data.ToVideoOperationInformation(aid);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        return default;
    }

    private async Task<VideoPageResponse> GetWebVideoPageResponseAsync(string aid, CancellationToken cancellationToken)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { "aid", aid },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Video.Detail));
        _authenticator.AuthorizeRestRequest(request, queryParameters, new BiliAuthorizeExecutionSettings { ApiType = BiliApiType.Web, ForceNoToken = true, RequireCookie = true, NeedRID = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponseVideoPageResponse).ConfigureAwait(false);
        return responseObj.Data;
    }
}
