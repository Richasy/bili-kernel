// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Bili.User;

/// <summary>
/// 用户服务.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 获取用户信息.
    /// </summary>
    /// <returns><see cref="UserCard"/>.</returns>
    Task<UserCard> GetUserInformationAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取用户信息.
    /// </summary>
    /// <returns>用户列表.</returns>
    Task<IReadOnlyList<UserProfile>> BatchGetUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default);
}
