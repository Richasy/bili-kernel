// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Models.Base;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Models.Article;

/// <summary>
/// 文章详情.
/// </summary>
public sealed class ArticleDetail
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleDetail"/> class.
    /// </summary>
    public ArticleDetail(
        ArticleIdentifier identifier,
        UserCard profile,
        ArticleCommunityInformation community,
        string htmlContent,
        DateTimeOffset publishTime,
        IList<BiliTag> tags,
        bool isLiked,
        bool isFavorite)
    {
        Identifier = identifier;
        Author = profile;
        PublishTime = publishTime;
        Tags = tags;
        HtmlContent = htmlContent;
        CommunityInformation = community;
        IsLiked = isLiked;
        IsFavorited = isFavorite;
    }

    /// <summary>
    /// 标识符.
    /// </summary>
    public ArticleIdentifier Identifier { get; }

    /// <summary>
    /// 用户信息.
    /// </summary>
    public UserCard Author { get; }

    /// <summary>
    /// 发布时间.
    /// </summary>
    public DateTimeOffset PublishTime { get; }

    /// <summary>
    /// 社区信息.
    /// </summary>
    public ArticleCommunityInformation CommunityInformation { get; }

    /// <summary>
    /// 标签列表.
    /// </summary>
    public IList<BiliTag> Tags { get; }

    /// <summary>
    /// 是否点赞.
    /// </summary>
    public bool IsLiked { get; }

    /// <summary>
    /// 是否收藏.
    /// </summary>
    public bool IsFavorited { get; }

    /// <summary>
    /// HTML 文本.
    /// </summary>
    public string HtmlContent { get; }
}
