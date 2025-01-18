// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Authorization;
// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.User;
using Richasy.BiliKernel.Services.User.Core;

namespace Richasy.BiliKernel.Services.User;

/// <summary>
/// 用户服务.
/// </summary>
public sealed class UserService : IUserService
{
    private readonly MyClient _myClient;

    /// <summary>
    /// 初始化 <see cref="MyProfileService"/> 类的新实例.
    /// </summary>
    public UserService(
        BiliHttpClient biliHttpClient,
        IAuthenticationService authenticationService,
        IBiliTokenResolver tokenResolver,
        BiliAuthenticator basicAuthenticator)
    {
        _myClient = new MyClient(biliHttpClient, authenticationService, tokenResolver, basicAuthenticator);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<UserProfile>> BatchGetUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default)
        => _myClient.BatchGetUsersAsync(userIds, cancellationToken);

    /// <inheritdoc/>
    public Task<UserCard> GetUserInformationAsync(string userId, CancellationToken cancellationToken = default)
        => _myClient.GetUserCardAsync(userId, cancellationToken);
}
