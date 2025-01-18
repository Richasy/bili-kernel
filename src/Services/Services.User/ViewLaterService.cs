// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.User.Core;

namespace Richasy.BiliKernel.Services.User;

/// <summary>
/// 稍后再看服务.
/// </summary>
public sealed class ViewLaterService : IViewLaterService
{
    private readonly ViewLaterClient _viewLaterClient;

    /// <summary>
    /// 初始化 <see cref="ViewLaterService"/> 类的新实例.
    /// </summary>
    public ViewLaterService(
        BiliHttpClient biliHttpClient,
        IAuthenticationService authenticationService,
        BiliAuthenticator basicAuthenticator)
    {
        _viewLaterClient = new ViewLaterClient(biliHttpClient, authenticationService, basicAuthenticator);
    }

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<VideoInformation>? Videos, int Count, int NextPageNumber)> GetViewLaterSetAsync(int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        var (videos, count) = await _viewLaterClient.GetMyViewLaterAsync(pageNumber, cancellationToken).ConfigureAwait(false);
        return (videos, count, pageNumber + 1);
    }

    /// <inheritdoc/>
    public Task AddAsync(string aid, CancellationToken cancellationToken = default)
        => _viewLaterClient.AddAsync(aid, cancellationToken);

    /// <inheritdoc/>
    public Task CleanAsync(ViewLaterCleanType cleanType, CancellationToken cancellationToken = default)
        => _viewLaterClient.CleanAsync(cleanType, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveAsync(string[] aids, CancellationToken cancellationToken = default)
        => _viewLaterClient.RemoveAsync(aids, cancellationToken);
}
