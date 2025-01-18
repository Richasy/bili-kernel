// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Http;
using RichasyKernel;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核扩展.
/// </summary>
public static class KernelExtensions
{
    /// <summary>
    /// 添加基础Bili客户端.
    /// </summary>
    /// <param name="builder">内核构建器.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBiliClient(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<BiliHttpClient>();
        return builder;
    }

    /// <summary>
    /// 添加Bili认证器.
    /// </summary>
    /// <param name="builder">内核构建器.</param>
    /// <returns><see cref="IKernelBuilder"/>.</returns>
    public static IKernelBuilder AddBiliAuthenticator(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<BiliAuthenticator>();
        return builder;
    }
}
