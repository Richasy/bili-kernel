// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Bilibili.App.Dynamic.V2;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Moment;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.Moment.Core;

internal static class MomentAdapter
{
    public static MomentInformation ToMomentInformation(this DynamicItem item)
    {
        var modules = item.Modules;
        var userModule = modules.Where(p => p.ModuleType == DynModuleType.ModuleAuthor).FirstOrDefault()?.ModuleAuthor;
        var descModule = modules.Where(p => p.ModuleType == DynModuleType.ModuleDesc).FirstOrDefault()?.ModuleDesc;
        var mainModule = modules.Where(p => p.ModuleType == DynModuleType.ModuleDynamic).FirstOrDefault()?.ModuleDynamic;
        var dataModule = modules.Where(p => p.ModuleType == DynModuleType.ModuleStat).FirstOrDefault()?.ModuleStat;

        if (mainModule is null)
        {
            return default;
        }

        UserProfile user = default;
        string tip = default;
        EmoteText description = default;
        MomentCommunityInformation communityInfo = default;
        var momentId = item.Extend.DynIdStr;
        var replyType = GetReplyTypeFromDynamicType(item.CardType);
        var replyId = replyType == CommentTargetType.Moment
            ? momentId
            : mainModule.ModuleItemCase == ModuleDynamic.ModuleItemOneofCase.DynPgc
                ? mainModule.DynPgc.Aid.ToString()
                : item.Extend.BusinessId;
        var momentType = GetMomentItemType(mainModule);
        var momentData = GetMomentContent(mainModule);
        if (userModule != null)
        {
            var author = userModule.Author;
            user = UserAdapterBase.CreateUserProfile(author.Mid, author.Name, author.Face, 64d);
            tip = userModule.PtimeLabelText;
        }
        else
        {
            var forwardUserModule = modules.Where(p => p.ModuleType == DynModuleType.ModuleAuthorForward).FirstOrDefault()?.ModuleAuthorForward;
            if (forwardUserModule != null)
            {
                var name = forwardUserModule.Title.FirstOrDefault()?.Text ?? "--";
                user = UserAdapterBase.CreateUserProfile(forwardUserModule.Uid, name, forwardUserModule.FaceUrl, 48d);
                tip = forwardUserModule.PtimeLabelText;
            }
        }

        if (descModule != null)
        {
            description = descModule.ToEmoteText();
        }

        if (dataModule != null)
        {
            communityInfo = new MomentCommunityInformation(
                momentId,
                dataModule.Like,
                dataModule.Reply,
                dataModule.LikeInfo?.IsLike ?? false);
        }

        return new MomentInformation(
            momentId,
            user,
            tip,
            communityInfo,
            replyType,
            replyId,
            momentType,
            momentData,
            description);
    }

    /// <summary>
    /// 将动态的描述模块 <see cref="ModuleDesc"/> 转换为表情文本.
    /// </summary>
    /// <param name="description">动态的描述模块.</param>
    /// <returns><see cref="EmoteText"/>.</returns>
    public static EmoteText ToEmoteText(this ModuleDesc description)
    {
        var text = description.Text;
        var descs = description.Desc;
        var emoteDict = new Dictionary<string, BiliImage>();

        // 判断是否有表情存在.
        if (descs.Count > 0 && descs.Any(p => p.Type == DescType.Emoji))
        {
            foreach (var item in (IEnumerable<Description>)descs.Where(p => p.Type == DescType.Emoji && !string.IsNullOrEmpty(p.Uri)))
            {
                var t = item.Text;
                if (!emoteDict.ContainsKey(t))
                {
                    emoteDict.Add(t, item.Uri.ToImage());
                }
            }
        }
        else
        {
            emoteDict = null;
        }

        return new EmoteText(text, emoteDict);
    }

    public static MomentInformation ToMomentInformation(this MdlDynForward forward)
    {
        var item = forward.Item;
        return item.ToMomentInformation();
    }

    private static CommentTargetType GetReplyTypeFromDynamicType(DynamicType type)
    {
        return type switch
        {
            DynamicType.Forward or
            DynamicType.Word or
            DynamicType.Live => CommentTargetType.Moment,
            DynamicType.Draw => CommentTargetType.Album,
            DynamicType.Av or
            DynamicType.Pgc or
            DynamicType.UgcSeason or
            DynamicType.Medialist => CommentTargetType.Video,
            DynamicType.Courses or
            DynamicType.CoursesSeason => CommentTargetType.Course,
            DynamicType.Article => CommentTargetType.Article,
            DynamicType.Music => CommentTargetType.Music,
            _ => CommentTargetType.None,
        };
    }

    private static MomentItemType GetMomentItemType(ModuleDynamic dynamic)
    {
        return dynamic == null
            ? MomentItemType.PlainText
            : dynamic.Type switch
            {
                ModuleDynamicType.MdlDynArchive => dynamic.DynArchive.IsPGC
                    ? MomentItemType.Pgc
                    : MomentItemType.Video,
                ModuleDynamicType.MdlDynPgc => MomentItemType.Pgc,
                ModuleDynamicType.MdlDynForward => MomentItemType.Forward,
                ModuleDynamicType.MdlDynDraw => MomentItemType.Image,
                ModuleDynamicType.MdlDynArticle => MomentItemType.Article,
                _ => MomentItemType.Unsupported,
            };
    }

    private static object? GetMomentContent(ModuleDynamic dynamic)
    {
        if (dynamic == null)
        {
            return null;
        }

        if (dynamic.Type == ModuleDynamicType.MdlDynPgc)
        {
            return dynamic.DynPgc.ToEpisodeInformation();
        }
        else if (dynamic.Type == ModuleDynamicType.MdlDynArchive)
        {
            return dynamic.DynArchive.IsPGC
                ? dynamic.DynArchive.ToEpisodeInformation()
                : dynamic.DynArchive.ToVideoInformation();
        }
        else if (dynamic.Type == ModuleDynamicType.MdlDynForward)
        {
            return dynamic.DynForward.ToMomentInformation();
        }
        else if (dynamic.Type == ModuleDynamicType.MdlDynDraw)
        {
            return dynamic.DynDraw.Items.Select(p => p.Src.ToImage(240d, 240d)).ToList();
        }
        else if (dynamic.Type == ModuleDynamicType.MdlDynArticle)
        {
            return dynamic.DynArticle.ToArticleInformation();
        }

        return null;
    }
}
