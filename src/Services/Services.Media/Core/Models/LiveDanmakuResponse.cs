﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Media.Core.Models;

/// <summary>
/// 直播弹幕响应.
/// </summary>
internal sealed class LiveDanmakuResponse
{
    /// <summary>
    /// 分组.
    /// </summary>
    [JsonPropertyName("group")]
    public string Group { get; set; }

    /// <summary>
    /// 业务ID.
    /// </summary>
    [JsonPropertyName("business_id")]
    public int BusinessId { get; set; }

    [JsonPropertyName("refresh_row_factor")]
    public float RefreshRowFactor { get; set; }

    [JsonPropertyName("refresh_rate")]
    public int RefreshRate { get; set; }

    [JsonPropertyName("max_delay")]
    public int MaxDelay { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("host_list")]
    public List<WebLiveDanmakuHost> HostList { get; set; }
}

internal sealed class WebLiveDanmakuHost
{
    [JsonPropertyName("host")]
    public string Host { get; set; }

    [JsonPropertyName("port")]
    public int Port { get; set; }

    [JsonPropertyName("wss_port")]
    public int WssPort { get; set; }

    [JsonPropertyName("ws_port")]
    public int WsPort { get; set; }
}
