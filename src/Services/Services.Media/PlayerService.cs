// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core;

namespace Richasy.BiliKernel.Services.Media;

/// <summary>
/// 播放器服务.
/// </summary>
public sealed class PlayerService : IPlayerService
{
    private readonly PlayerClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerService"/> class.
    /// </summary>
    public PlayerService(
        BiliHttpClient httpClient,
        BiliAuthenticator authenticator,
        IBiliTokenResolver tokenResolver) => _client = new PlayerClient(httpClient, authenticator, tokenResolver);

    /// <inheritdoc/>
    public async Task<VideoPlayerView> GetVideoPageDetailAsync(MediaIdentifier video, CancellationToken cancellationToken = default)
    {
        VideoPlayerView? view;
        try
        {
            view = await _client.GetVideoPageDetailWithGrpcAsync(video, cancellationToken).ConfigureAwait(false);
        }
        catch (System.Exception)
        {
            try
            {
                // 部分新发布的视频无法通过 gRPC 获取详情，改用网页 API 获取.
                view = await _client.GetVideoPageDetailWithRestAsync(video, cancellationToken).ConfigureAwait(false);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        return view;
    }

    /// <inheritdoc/>
    public Task<DashMediaInformation> GetVideoPlayDetailAsync(MediaIdentifier video, long cid, CancellationToken cancellationToken = default)
        => _client.GetVideoPlayDetailWithRestAsync(video, cid, cancellationToken);

    /// <inheritdoc/>
    public Task<LivePlayerView> GetLivePageDetailAsync(MediaIdentifier live, CancellationToken cancellationToken = default)
        => _client.GetLivePageDetailAsync(live, cancellationToken);

    /// <inheritdoc/>
    public Task<LiveMediaInformation> GetLivePlayDetailAsync(MediaIdentifier live, int quality = 400, bool audioOnly = false, CancellationToken cancellationToken = default)
        => _client.GetLivePlayDetailAsync(live, quality, audioOnly, cancellationToken);

    /// <inheritdoc/>
    public Task<PgcPlayerView> GetPgcPageDetailAsync(string? seasonId = null, string? episodeId = null, CancellationToken cancellationToken = default)
        => _client.GetPgcPageDetailAsync(seasonId, episodeId, cancellationToken);

    /// <inheritdoc/>
    public Task<DashMediaInformation> GetPgcPlayDetailAsync(string cid, string episodeId, string seasonType, CancellationToken cancellationToken = default)
        => _client.GetPgcPlayDetailAsync(cid, episodeId, seasonType, cancellationToken);

    /// <inheritdoc/>
    public async Task<OnlineViewer> GetOnlineViewerAsync(string aid, string cid, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.GetOnlineViewerAsync(aid, cid, cancellationToken).ConfigureAwait(false);
        }
        catch (System.Exception)
        {
            return new OnlineViewer(0, "--");
        }
    }

    /// <inheritdoc/>
    public Task ToggleVideoLikeAsync(string aid, bool isLike, CancellationToken cancellationToken = default)
        => _client.ToggleVideoLikeAsync(aid, isLike, cancellationToken);

    /// <inheritdoc/>
    public Task CoinVideoAsync(string aid, int number, bool alsoLike, CancellationToken cancellationToken = default)
        => _client.CoinVideoAsync(aid, number, alsoLike, cancellationToken);

    /// <inheritdoc/>
    public Task FavoriteVideoAsync(string aid, IList<string> favoriteIds, IList<string> unfavoriteIds, CancellationToken cancellationToken = default)
        => _client.FavoriteVideoAsync(aid, favoriteIds, unfavoriteIds, true, cancellationToken);

    /// <inheritdoc/>
    public Task FavoriteEpisodeAsync(string aid, IList<string> favoriteIds, IList<string> unfavoriteIds, CancellationToken cancellationToken = default)
        => _client.FavoriteVideoAsync(aid, favoriteIds, unfavoriteIds, false, cancellationToken);

    /// <inheritdoc/>
    public Task TripleVideoAsync(string aid, CancellationToken cancellationToken = default)
        => _client.TripleAsync(aid, cancellationToken);

    /// <inheritdoc/>
    public Task ReportVideoProgressAsync(string aid, string cid, int progress, CancellationToken cancellationToken = default)
        => _client.ReportProgressAsync(aid, cid, progress, cancellationToken);

    /// <inheritdoc/>
    public Task ReportEpisodeProgressAsync(string aid, string cid, string episodeId, string seasonId, int progress, CancellationToken cancellationToken = default)
        => _client.ReportProgressAsync(aid, cid, episodeId, seasonId, progress, cancellationToken);
    
    /// <inheritdoc/>
    public Task<VideoOperationInformation> GetEpisodeOperationInformationAsync(string episodeId, CancellationToken cancellationToken = default)
        => _client.GetEpisodeOpeartionInformationAsync(episodeId, cancellationToken);

    /// <inheritdoc/>
    public Task<ClientWebSocket> EnterLiveRoomAsync(string roomId, CancellationToken cancellationToken)
        => _client.EnterLiveRoomAsync(roomId, cancellationToken);

    /// <inheritdoc/>
    public Task SendLiveHeartBeatAsync(ClientWebSocket client, CancellationToken cancellationToken)
        => PlayerClient.SendLiveHeartBeatAsync(client, cancellationToken);

    /// <inheritdoc/>
    public Task SendLiveMessageAsync(ClientWebSocket client, object data, int action, CancellationToken cancellationToken)
        => PlayerClient.SendLiveMessageAsync(client, data, action, cancellationToken);

    /// <inheritdoc/>
    public Task<IReadOnlyList<LiveMessage>?> GetLiveSocketMessagesAsync(ClientWebSocket client, CancellationToken cancellationToken)
        => PlayerClient.GetLiveSocketMessagesAsync(client, cancellationToken);
}
