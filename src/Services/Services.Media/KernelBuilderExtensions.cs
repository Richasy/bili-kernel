﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Services.Media;
using RichasyKernel;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核构建器扩展.
/// </summary>
public static class KernelBuilderExtensions
{
    /// <summary>
    /// 添加视频探索服务.
    /// </summary>
    public static IKernelBuilder AddVideoDiscoveryService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IVideoDiscoveryService, VideoDiscoveryService>();
        return builder;
    }

    /// <summary>
    /// 添加直播探索服务.
    /// </summary>
    public static IKernelBuilder AddLiveDiscoveryService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<ILiveDiscoveryService, LiveDiscoveryService>();
        return builder;
    }

    /// <summary>
    /// 添加动漫/电影/电视剧/纪录片服务.
    /// </summary>
    public static IKernelBuilder AddEntertainmentDiscoveryService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IEntertainmentDiscoveryService, EntertainmentDiscoveryService>();
        return builder;
    }

    /// <summary>
    /// 添加播放服务.
    /// </summary>
    public static IKernelBuilder AddPlayerService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IPlayerService, PlayerService>();
        return builder;
    }

    /// <summary>
    /// 添加弹幕服务.
    /// </summary>
    public static IKernelBuilder AddDanmakuService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IDanmakuService, DanmakuService>();
        return builder;
    }

    /// <summary>
    /// 添加字幕服务.
    /// </summary>
    public static IKernelBuilder AddSubtitleService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<ISubtitleService, SubtitleService>();
        return builder;
    }
}
