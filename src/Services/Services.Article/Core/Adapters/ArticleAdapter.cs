// Copyright (c) Richasy. All rights reserved.

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Article;
using Richasy.BiliKernel.Models.Base;
using Richasy.BiliKernel.Models.User;
using Richasy.BiliKernel.Services.Article.Core.Models;

namespace Richasy.BiliKernel.Services.Article.Core;

internal static partial class ArticleAdapter
{
    public static Partition ToPartition(this ArticleCategory category)
    {
        var id = category.Id.ToString();
        var name = category.Name;
        var children = category.Children?.Select(p => p.ToPartition()).ToList();
        var parentId = category.ParentId?.ToString();
        return new Partition(id, name, default, children, parentId);
    }

    public static ArticleInformation ToArticleInformation(this Article article)
    {
        var identifier = new ArticleIdentifier(article.Id.ToString(), article.Title, article.Summary, article.CoverUrls?.FirstOrDefault()?.ToArticleCover());
        var user = UserAdapterBase.CreateUserProfile(article.Publisher.Mid, article.Publisher.Publisher, article.Publisher.PublisherAvatar, 48d);
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(article.PublishTime).ToLocalTime();
        var communityInfo = new ArticleCommunityInformation(article.Id.ToString(), article.Stats.ViewCount, article.Stats.FavoriteCount, article.Stats.LikeCount, article.Stats.ReplyCount, article.Stats.ShareCount, article.Stats.CoinCount);
        var info = new ArticleInformation(identifier, user, publishTime, communityInfo);
        info.AddExtensionIfNotNull(ArticleExtensionDataId.WordCount, article.WordCount);
        info.AddExtensionIfNotNull(ArticleExtensionDataId.Partition, article.Category.ToPartition());
        info.AddExtensionIfNotNull(ArticleExtensionDataId.RelatedPartitions, article.RelatedCategories?.Select(p => p.ToPartition()).ToList());
        return info;
    }

    public static ArticleDetail ToArticleDetail(this ArticleContentResponse response, ArticleIdentifier identifier, ArticleViewResponse view)
    {
        var author = response.readInfo.author;
        var stats = view.stats;
        var user = UserAdapterBase.CreateUserProfile(author.mid, author.name, author.face, 96d);
        var detailProfile = new UserDetailProfile(user, default, author.level, author.vip?.type > 0);
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(response.readInfo.publish_time).ToLocalTime();
        var communityInfo = new ArticleCommunityInformation(identifier.Id, stats.view, stats.favorite, stats.like, stats.reply, stats.share, stats.coin);
        var tags = response.readInfo.tags?.Select(p => new BiliTag(p.tid.ToString(), p.name)).ToList();
        var isLiked = view.like > 0;
        var isFav = view.favorite;
        var relation = view.attention ? UserRelationStatus.Following : UserRelationStatus.Unfollow;
        var userCard = new UserCard(detailProfile, new UserCommunityInformation(author.mid.ToString(), fansCount: author.fans, relation: relation));
        var content = response.readInfo.opus is null ? response.readInfo.content : BuildHtmlContent(response.readInfo.opus);
        return new ArticleDetail(identifier, userCard, communityInfo, FormatHtmlContent(content), publishTime, tags, isLiked, isFav);
    }

    private static string FormatHtmlContent(string content)
        => content.Replace("data-src", "src").Replace("\"//", "\"http://").Replace("<p></p>", string.Empty).Replace("<p><br></p>", string.Empty).Replace("<p><br/></p>", string.Empty);

    private static string BuildHtmlContent(Opus opus)
    {
        var htmlContent = string.Empty;
        foreach (var item in opus.content.paragraphs)
        {
            if (item.para_type == 1)
            {
                // 文本节点.
                var nodes = item.text.nodes;
                var text = string.Empty;
                foreach (var node in nodes)
                {
                    if (node.node_type == 1)
                    {
                        if (node.word.words == "\\n")
                        {
                            continue;
                        }

                        var fontsize = node.word.font_size == 0 ? 17 : node.word.font_size;
                        var innerText = node.word.style?.bold == true
                            ? $"<strong>{WebUtility.HtmlEncode(node.word.words)}</strong>"
                            : WebUtility.HtmlEncode(node.word.words);
                        text += $"<p style=\"font-size:'{fontsize}'\">{innerText}</p>";
                    }
                    else if(node.word is not null)
                    {
                        text += $"<p>{WebUtility.HtmlEncode(node.word.words)}</p>";
                    }
                }

                htmlContent += $"<p>{text}</p>";
            }
            else if (item.para_type == 2)
            {
                // 图片节点.
                foreach (var img in item.pic.pics)
                {
                    var imgTag = $"""
                        <p class="image-package">
                           <figure class="img-box" contenteditable="false">
                              <img src="{img.url}" width="{img.width}" height="{img.height}" data-size="{img.size}"/>
                           </figure>
                        </p>
                        """;

                    htmlContent += imgTag;
                }
            }
            else if (item.para_type == 3)
            {
                // 分割线.
                var line = item.line;
                var imgTag = $"""
                    <img src="{line.pic.url}" style="max-width: 100%; width: auto; display: block; margin: 0 auto;" height="{line.pic.height}" />
                    """;
                htmlContent += imgTag;
            }
            else
            {
                Debug.WriteLine(JsonSerializer.Serialize(item, JsonContext.Default.OpusParagraph));
            }
        }

        return htmlContent;
    }
}
