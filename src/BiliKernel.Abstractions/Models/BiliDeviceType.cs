// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Models;

/// <summary>
/// 哔哩 API 类型.
/// </summary>
public enum BiliApiType
{
    /// <summary>
    /// App API，需要用到 access key.
    /// </summary>
    App,

    /// <summary>
    /// 网页 API，需要用到 cookie.
    /// </summary>
    Web,

    /// <summary>
    /// 不添加 AppKey 和 Token.
    /// </summary>
    None,
}
