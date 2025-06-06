﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Models;

/// <summary>
/// 消息类型.
/// </summary>
public enum ChatMessageType
{
    /// <summary>
    /// 文本消息.
    /// </summary>
    Text,

    /// <summary>
    /// 图片消息.
    /// </summary>
    Image,

    /// <summary>
    /// 撤回.
    /// </summary>
    Withdrawn,

    /// <summary>
    /// 未知消息.
    /// </summary>
    Unknown,
}
