﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Resolvers.WinUIQRCode;
using RichasyKernel;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核构建器扩展.
/// </summary>
public static class KernelBuilderExtensions
{
    /// <summary>
    /// 添加本地二维码解析器.
    /// </summary>
    public static IKernelBuilder AddWinUIQRCodeResolver(this IKernelBuilder builder, Func<byte[], Task> renderFunc)
    {
        builder.Services.AddSingleton<IQRCodeResolver>(provider => new WinUIQRCodeResolver(renderFunc));
        return builder;
    }
}
