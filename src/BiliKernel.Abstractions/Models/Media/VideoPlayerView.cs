// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Base;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 视频播放页面数据.
/// </summary>
public sealed class VideoPlayerView(
    VideoInformation info,
    UserCommunityInformation ownerCommunity,
    IList<VideoPart>? parts = default,
    IList<VideoSeason>? seasons = default,
    IList<VideoInformation>? recommends = default,
    IList<BiliTag>? tags = default,
    PlayerProgress? progress = default,
    InteractiveVideoSnapshot? interactiveVideoSnapshot = default,
    VideoOperationInformation? operation = default,
    MediaAspectRatio? aspectRatio = default,
    bool isInteractionVideo = false)
{

    /// <summary>
    /// 视频信息.
    /// </summary>
    public VideoInformation Information { get; } = info;

    /// <summary>
    /// 所有者社区数据.
    /// </summary>
    public UserCommunityInformation OwnerCommunity { get; } = ownerCommunity;

    /// <summary>
    /// 分集信息.
    /// </summary>
    public IList<VideoPart>? Parts { get; } = parts;

    /// <summary>
    /// 推荐列表.
    /// </summary>
    public IList<VideoInformation>? Recommends { get; } = recommends;

    /// <summary>
    /// 合集列表.
    /// </summary>
    public IList<VideoSeason>? Seasons { get; } = seasons;

    /// <summary>
    /// 标签列表.
    /// </summary>
    public IList<BiliTag>? Tags { get; } = tags;

    /// <summary>
    /// 播放进度.
    /// </summary>
    public PlayerProgress? Progress { get; } = progress;

    /// <summary>
    /// 操作信息.
    /// </summary>
    public VideoOperationInformation? Operation { get; } = operation;

    /// <summary>
    /// 互动视频快照. （暂不支持）.
    /// </summary>
    public InteractiveVideoSnapshot? InteractiveVideoSnapshot { get; } = interactiveVideoSnapshot;

    /// <summary>
    /// 视频宽高比.
    /// </summary>
    public MediaAspectRatio? AspectRatio { get; } = aspectRatio;

    /// <summary>
    /// 是否为互动视频.
    /// </summary>
    public bool IsInteractiveVideo { get; } = isInteractionVideo;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VideoPlayerView view && EqualityComparer<VideoInformation>.Default.Equals(Information, view.Information);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Information);
}
