// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// DASH 播放轨道信息.
/// </summary>
public sealed class DashSegmentInformation(
    string id,
    string baseUrl,
    int bandwidth,
    string mimeType,
    string codecs,
    int width,
    int height,
    string initialization,
    string indexRange,
    IList<string>? backupUrls = default,
    int? codecId = default)
{

    /// <summary>
    /// Dash Id.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// 基链接.
    /// </summary>
    public string BaseUrl { get; set; } = baseUrl;

    /// <summary>
    /// 媒体要求的带宽.
    /// </summary>
    public int Bandwidth { get; } = bandwidth;

    /// <summary>
    /// 媒体格式.
    /// </summary>
    public string MimeType { get; } = mimeType;

    /// <summary>
    /// 备份链接.
    /// </summary>
    public IList<string>? BackupUrls { get; } = backupUrls;

    /// <summary>
    /// 媒体编码.
    /// </summary>
    public string Codecs { get; } = codecs;

    /// <summary>
    /// 媒体宽度.
    /// </summary>
    public int Width { get; } = width;

    /// <summary>
    /// 媒体高度.
    /// </summary>
    public int Height { get; } = height;

    /// <summary>
    /// 起始位置.
    /// </summary>
    public string Initialization { get; } = initialization;

    /// <summary>
    /// 索引范围.
    /// </summary>
    public string IndexRange { get; } = indexRange;

    /// <summary>
    /// 编解码器 Id.
    /// </summary>
    public int? CodecId { get; set; } = codecId;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is DashSegmentInformation information && Id == information.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
