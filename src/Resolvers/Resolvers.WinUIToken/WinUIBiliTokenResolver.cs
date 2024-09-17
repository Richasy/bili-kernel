// Copyright (c) Richasy. All rights reserved.

using System.Text.Json;
using System.Text.Json.Serialization;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models.Authorization;
using Windows.Storage;

namespace Richasy.BiliKernel.Resolvers.WinUIToken;

/// <summary>
/// 适用于 WinUI 的令牌解析器.
/// </summary>
public sealed class WinUIBiliTokenResolver : IBiliTokenResolver
{
    private const string TokenKey = "BiliUserTokenInfo";
    private BiliToken? _cacheToken;

    /// <inheritdoc/>
    public BiliToken? GetToken()
    {
        if (_cacheToken != null)
        {
            return _cacheToken;
        }

        var container = GetContainer();
        if (container.Values.TryGetValue(TokenKey, out var token))
        {
            _cacheToken = JsonSerializer.Deserialize(token.ToString(), SourceGenerationContext.Default.BiliToken);
        }

        return _cacheToken;
    }

    /// <inheritdoc/>
    public void RemoveToken()
    {
        _cacheToken = null;
        var container = GetContainer();
        container.Values.Remove(TokenKey);
    }

    /// <inheritdoc/>
    public void SaveToken(BiliToken token)
    {
        _cacheToken = token;
        var container = GetContainer();
        container.Values[TokenKey] = JsonSerializer.Serialize(token, SourceGenerationContext.Default.BiliToken);
    }

    private static ApplicationDataContainer GetContainer()
        => ApplicationData.Current.LocalSettings.CreateContainer("BiliKernel", ApplicationDataCreateDisposition.Always);
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(BiliToken))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}
