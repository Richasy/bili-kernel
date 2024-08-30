// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// PGC 播放器视图信息.
/// </summary>
public sealed class PgcPlayerView(
    SeasonInformation information,
    IList<SeasonInformation>? seasons = null,
    IList<EpisodeInformation>? episodes = null,
    Dictionary<string, IList<EpisodeInformation>>? extras = null,
    PlayerProgress? progress = null,
    string? warning = default)
{

    /// <summary>
    /// 剧集信息.
    /// </summary>
    public SeasonInformation Information { get; } = information;

    /// <summary>
    /// 关联的 PGC 内容季度信息.
    /// </summary>
    public IList<SeasonInformation>? Seasons { get; } = seasons;

    /// <summary>
    /// 剧集列表.
    /// </summary>
    public IList<EpisodeInformation>? Episodes { get; } = episodes;

    /// <summary>
    /// 附加的视频列表，比如预告片、宣传片、片花等.
    /// </summary>
    public Dictionary<string, IList<EpisodeInformation>>? Extras { get; } = extras;

    /// <summary>
    /// 上次播放进度.
    /// </summary>
    public PlayerProgress? Progress { get; set; } = progress;

    /// <summary>
    /// 播放警告.
    /// </summary>
    public string? Warning { get; } = warning;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is PgcPlayerView view && EqualityComparer<SeasonInformation>.Default.Equals(Information, view.Information);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Information);
}
