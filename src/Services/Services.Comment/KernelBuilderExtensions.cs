// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Bili.Comment;
using Richasy.BiliKernel.Services.Comment;
using RichasyKernel;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核构建器扩展.
/// </summary>
public static class KernelBuilderExtensions
{
    /// <summary>
    /// 添加评论服务.
    /// </summary>
    public static IKernelBuilder AddCommentService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<ICommentService, CommentService>();
        return builder;
    }
}
