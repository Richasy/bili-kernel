// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Danmaku;

namespace Richasy.BiliKernel.Bili.Media;

/// <summary>
/// 弹幕服务.
/// </summary>
public interface IDanmakuService
{
    /// <summary>
    /// 获取弹幕元信息.
    /// </summary>
    Task<DanmakuMeta> GetDanmakuMetaAsync(string aid, string cid, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分段弹幕.
    /// </summary>
    Task<IReadOnlyList<DanmakuInformation>> GetSegmentDanmakusAsync(string aid, string cid, int segmentIndex, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送视频弹幕.
    /// </summary>
    Task SendVideoDanmakuAsync(string content, string aid, string cid, int progress, string color, bool isStandardSize = true, DanmakuLocation location = DanmakuLocation.Scroll, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送直播弹幕.
    /// </summary>
    Task SendLiveDanmakuAsync(string content, string roomId, string color, bool isStandardSize = true, DanmakuLocation location = DanmakuLocation.Scroll, CancellationToken cancellationToken = default);
}
