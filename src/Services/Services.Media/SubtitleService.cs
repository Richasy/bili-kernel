// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Subtitle;
using Richasy.BiliKernel.Services.Media.Core;

namespace Richasy.BiliKernel.Services.Media;

/// <summary>
/// 字幕服务.
/// </summary>
public sealed class SubtitleService : ISubtitleService
{
    private readonly SubtitleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubtitleService"/> class.
    /// </summary>
    public SubtitleService(
        BiliHttpClient httpClient,
        BiliAuthenticator authenticator)
    {
        _client = new SubtitleClient(httpClient, authenticator);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<SubtitleInformation>> GetSubtitleDetailAsync(SubtitleMeta meta, CancellationToken cancellationToken = default)
        => _client.GetSubtitleDetailAsync(meta, cancellationToken);

    /// <inheritdoc/>
    public Task<IReadOnlyList<SubtitleMeta>> GetSubtitleMetasAsync(string aid, string cid, CancellationToken cancellationToken = default)
        => _client.GetSubtitleMetasAsync(aid, cid, cancellationToken);
}
