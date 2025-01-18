// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Bili.Authorization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Storage;

namespace Richasy.BiliKernel.Resolvers.WinUICookies;

/// <summary>
/// 适用于 WinUI 的 Cookie 解析器.
/// </summary>
public sealed class WinUIBiliCookiesResolver : IBiliCookiesResolver
{
    private const string CookiesKey = "BiliUserCookies";
    private IDictionary<string, string>? _cacheCookies;

    /// <inheritdoc/>
    public IDictionary<string, string> GetCookies()
    {
        if (_cacheCookies?.Count > 0)
        {
            return _cacheCookies;
        }

        var container = GetContainer();
        if (container.Values.TryGetValue(CookiesKey, out var cookies))
        {
            _cacheCookies = JsonSerializer.Deserialize(cookies.ToString(), SourceGenerationContext.Default.DictionaryStringString);
        }

        return _cacheCookies;
    }

    /// <inheritdoc/>
    public string GetCookieString()
    {
        var cookies = GetCookies();
        var cookieList = cookies.Select(item => $"{item.Key}={item.Value}");
        return string.Join("; ", cookieList);
    }

    /// <inheritdoc/>
    public void RemoveCookies()
    {
        _cacheCookies = null;
        var container = GetContainer();
        container.Values.Remove(CookiesKey);
    }

    /// <inheritdoc/>
    public void SaveCookies(IDictionary<string, string> cookies)
    {
        _cacheCookies = cookies;
        var container = GetContainer();
        container.Values[CookiesKey] = JsonSerializer.Serialize(cookies, SourceGenerationContext.Default.DictionaryStringString);
    }

    private static ApplicationDataContainer GetContainer()
        => ApplicationData.Current.LocalSettings.CreateContainer("BiliKernel", ApplicationDataCreateDisposition.Always);
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Dictionary<string, string>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}
