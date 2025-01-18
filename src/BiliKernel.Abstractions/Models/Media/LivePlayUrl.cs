// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 直播地址拼接信息.
/// </summary>
public sealed class LivePlayUrl(string protocol, string host, string route, string query)
{
    /// <summary>
    /// 域名.
    /// </summary>
    public string Host { get; } = host;

    /// <summary>
    /// 路由.
    /// </summary>
    public string Route { get; } = route;

    /// <summary>
    /// 查询参数.
    /// </summary>
    public string Query { get; } = query;

    /// <summary>
    /// 协议.
    /// </summary>
    public string Protocol { get; set; } = protocol;

    /// <inheritdoc/>
    public override string ToString()
        => $"{Host}{Route}{Query}";
}
