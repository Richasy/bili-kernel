﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Models;

namespace Richasy.BiliKernel.Authenticator;

/// <summary>
/// 哔哩授权执行设置.
/// </summary>
public class BiliAuthorizeExecutionSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BiliAuthorizeExecutionSettings"/> class.
    /// </summary>
    public BiliAuthorizeExecutionSettings(
        BiliApiType apiType = BiliApiType.App,
        bool useToken = false,
        bool forceNoToken = false,
        bool useCookie = false,
        bool onlyUseAppKey = false,
        bool needRID = false,
        bool needCSRF = false,
        string? additionalQuery = default)
    {
        ApiType = apiType;
        RequireToken = useToken;
        ForceNoToken = forceNoToken;
        RequireCookie = useCookie;
        OnlyUseAppKey = onlyUseAppKey;
        NeedRID = needRID;
        NeedCSRF = needCSRF;
        AdditionalQuery = additionalQuery;
    }

    /// <summary>
    /// 设备类型.
    /// </summary>
    public BiliApiType ApiType { get; set; }

    /// <summary>
    /// 是否需要Token.
    /// </summary>
    public bool RequireToken { get; set; }

    /// <summary>
    /// 是否强制不使用Token.
    /// </summary>
    public bool ForceNoToken { get; set; }

    /// <summary>
    /// 是否需要Cookie.
    /// </summary>
    public bool RequireCookie { get; set; }

    /// <summary>
    /// 是否仅使用AppKey.
    /// </summary>
    public bool OnlyUseAppKey { get; set; }

    /// <summary>
    /// 是否需要RID.
    /// </summary>
    public bool NeedRID { get; set; }

    /// <summary>
    /// 是否需要CSRF.
    /// </summary>
    public bool NeedCSRF { get; set; }

    /// <summary>
    /// 附加查询参数.
    /// </summary>
    public string? AdditionalQuery { get; set; }
}
