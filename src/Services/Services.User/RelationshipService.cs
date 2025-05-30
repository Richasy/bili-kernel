﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.User;
using Richasy.BiliKernel.Services.User.Core;

namespace Richasy.BiliKernel.Services.User;

/// <summary>
/// 用户资料服务.
/// </summary>
public sealed class RelationshipService : IRelationshipService
{
    private readonly MyClient _myClient;

    /// <summary>
    /// 初始化 <see cref="MyProfileService"/> 类的新实例.
    /// </summary>
    public RelationshipService(
        BiliHttpClient biliHttpClient,
        IAuthenticationService authenticationService,
        IBiliTokenResolver tokenResolver,
        BiliAuthenticator basicAuthenticator)
    {
        _myClient = new MyClient(biliHttpClient, authenticationService, tokenResolver, basicAuthenticator);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<UserGroup>> GetMyFollowUserGroupsAsync(CancellationToken cancellationToken = default)
        => _myClient.GetMyFollowUserGroupsAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<UserCard> Users, int NextPageNumber)> GetMyFollowUserGroupDetailAsync(string groupId, int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 0 ? 1 : pageNumber + 1;
        var list = await _myClient.GetMyFollowUserGroupDetailAsync(groupId, pageNumber, cancellationToken).ConfigureAwait(false);
        return (list, pageNumber++);
    }

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<UserCard> Users, int Count, int NextPageNumber)> GetMyFansAsync(int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 0 ? 1 : pageNumber + 1;
        var (users, count) = await _myClient.GetMyFansAsync(pageNumber, cancellationToken).ConfigureAwait(false);
        return (users, count, pageNumber++);
    }

    /// <inheritdoc/>
    public Task FollowUserAsync(string userId, CancellationToken cancellationToken = default)
        => _myClient.ModifyRelationshipAsync(userId, isFollow: true, cancellationToken);

    /// <inheritdoc/>
    public Task UnfollowUserAsync(string userId, CancellationToken cancellationToken = default)
        => _myClient.ModifyRelationshipAsync(userId, isFollow: false, cancellationToken);

    /// <inheritdoc/>
    public Task<UserRelationStatus> GetRelationshipAsync(string userId, CancellationToken cancellationToken = default)
        => _myClient.GetRelationshipAsync(userId, cancellationToken);
}
