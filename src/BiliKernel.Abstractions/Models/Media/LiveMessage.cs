// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 直播消息.
/// </summary>
public sealed class LiveMessage(
    LiveMessageType type,
    object data)
{
    /// <summary>
    /// 类型.
    /// </summary>
    public LiveMessageType Type { get; } = type;

    /// <summary>
    /// 数据.
    /// </summary>
    public object Data { get; } = data;
}
