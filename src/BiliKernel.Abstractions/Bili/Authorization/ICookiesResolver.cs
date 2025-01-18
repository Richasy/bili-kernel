// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Bili.Authorization;

/// <summary>
/// Cookie 解析器.
/// </summary>
public interface IBiliCookiesResolver
{
    /// <summary>
    /// 获取 Cookie 字符串.
    /// </summary>
    /// <returns></returns>
    string GetCookieString();

    /// <summary>
    /// 获取 Cookie 列表.
    /// </summary>
    /// <returns>Cookie 列表.</returns>
    IDictionary<string, string> GetCookies();

    /// <summary>
    /// 保存 Cookie.
    /// </summary>
    /// <param name="cookies">Cookie 列表.</param>
    void SaveCookies(IDictionary<string, string> cookies);

    /// <summary>
    /// 移除 Cookie.
    /// </summary>
    void RemoveCookies();
}
