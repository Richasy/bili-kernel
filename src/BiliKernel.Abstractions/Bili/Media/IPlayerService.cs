// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Bili.Media;

/// <summary>
/// 播放器服务，可以获取播放信息.
/// </summary>
public interface IPlayerService
{
    /// <summary>
    /// 获取视频页面详情.
    /// </summary>
    Task<VideoPlayerView> GetVideoPageDetailAsync(MediaIdentifier video, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取视频播放信息.
    /// </summary>
    Task<DashMediaInformation> GetVideoPlayDetailAsync(MediaIdentifier video, long cid, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取直播页面详情.
    /// </summary>
    Task<LivePlayerView> GetLivePageDetailAsync(MediaIdentifier live, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取直播播放信息.
    /// </summary>
    Task<LiveMediaInformation> GetLivePlayDetailAsync(MediaIdentifier live, int quality = 400, bool audioOnly = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 PGC 页面详情.
    /// </summary>
    Task<PgcPlayerView> GetPgcPageDetailAsync(string? seasonId = default, string? episodeId = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 PGC 播放信息.
    /// </summary>
    Task<DashMediaInformation> GetPgcPlayDetailAsync(string cid, string episodeId, string seasonType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取番剧分集交互信息.
    /// </summary>
    Task<VideoOperationInformation> GetEpisodeOperationInformationAsync(string episodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取在线观看人数.
    /// </summary>
    Task<OnlineViewer> GetOnlineViewerAsync(string aid, string cid, CancellationToken cancellationToken);

    /// <summary>
    /// 切换视频点赞状态.
    /// </summary>
    Task ToggleVideoLikeAsync(string aid, bool isLike, CancellationToken cancellationToken = default);

    /// <summary>
    /// 投币.
    /// </summary>
    Task CoinVideoAsync(string aid, int number, bool alsoLike, CancellationToken cancellationToken = default);

    /// <summary>
    /// 收藏视频.
    /// </summary>
    Task FavoriteVideoAsync(string aid, IList<string> favoriteIds, IList<string> unfavoriteIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 收藏剧集.
    /// </summary>
    Task FavoriteEpisodeAsync(string aid, IList<string> favoriteIds, IList<string> unfavoriteIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 一键三连.
    /// </summary>
    Task TripleVideoAsync(string aid, CancellationToken cancellationToken = default);

    /// <summary>
    /// 报告视频播放进度.
    /// </summary>
    Task ReportVideoProgressAsync(string aid, string cid, int progress, CancellationToken cancellationToken = default);

    /// <summary>
    /// 报告剧集播放进度.
    /// </summary>
    Task ReportEpisodeProgressAsync(string aid, string cid, string episodeId, string seasonId, int progress, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取直播间信息.
    /// </summary>
    Task<ClientWebSocket> EnterLiveRoomAsync(string roomId, CancellationToken cancellationToken);

    /// <summary>
    /// 发送直播间心跳，维持直播流.
    /// </summary>
    Task SendLiveHeartBeatAsync(ClientWebSocket client, CancellationToken cancellationToken);

    /// <summary>
    /// 发送直播间消息.
    /// </summary>
    Task SendLiveMessageAsync(ClientWebSocket client, object data, int action, CancellationToken cancellationToken);

    /// <summary>
    /// 获取直播间消息.
    /// </summary>
    Task<IReadOnlyList<LiveMessage>?> GetLiveSocketMessagesAsync(ClientWebSocket client, CancellationToken cancellationToken);
}
