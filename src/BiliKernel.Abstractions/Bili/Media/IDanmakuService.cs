// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Richasy.BiliKernel.Models.Danmaku;
using Richasy.BiliKernel.Models;

namespace Richasy.BiliKernel.Bili.Media;

/// <summary>
/// 弹幕服务.
/// </summary>
public interface IDanmakuService
{
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
