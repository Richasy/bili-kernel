// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.Subtitle;

namespace Richasy.BiliKernel.Bili.Media;

/// <summary>
/// 字幕服务.
/// </summary>
public interface ISubtitleService
{
    /// <summary>
    /// 获取视频的字幕元数据.
    /// </summary>
    Task<IReadOnlyList<SubtitleMeta>> GetSubtitleMetasAsync(string aid, string cid, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取字幕详细信息.
    /// </summary>
    Task<IReadOnlyList<SubtitleInformation>> GetSubtitleDetailAsync(SubtitleMeta meta, CancellationToken cancellationToken = default);
}
