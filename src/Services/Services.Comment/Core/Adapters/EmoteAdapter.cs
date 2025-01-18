// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Services.Comment.Core.Models;

namespace Richasy.BiliKernel.Services.Comment.Core.Adapters;

internal static class EmoteAdapter
{
    public static EmotePackage ToEmotePackage(this BiliEmotePackage package)
    {
        var pack = new EmotePackage();
        pack.Name = package.Text;
        pack.Icon = package.Url.ToImage();
        pack.Images = package.Emotes.Select(e => e.ToEmote()).ToList();
        return pack;
    }

    public static Emote ToEmote(this BiliEmote emote)
    {
        var id = emote.Id.ToString();
        var key = emote.Text;
        var img = emote.Url.StartsWith("http") ? emote.Url.ToImage() : null;

        return new Emote(id, key, img);
    }

    public static EmoteText ToEmoteText(this Bilibili.Main.Community.Reply.V1.Content content)
    {
        var text = content.Message.Replace("[请升级到App最新版本查看图文评论]", string.Empty);
        var emotes = content.Emotes;
        var emoteDict = new Dictionary<string, BiliImage>();
        var pictures = new List<BiliImage>();

        // 判断是否有表情存在.
        if (emotes?.Count > 0)
        {
            foreach (var item in emotes)
            {
                var k = item.Key;
                if (!emoteDict.ContainsKey(k))
                {
                    emoteDict.Add(k, item.Value.Url.ToImage());
                }
            }
        }
        else
        {
            emoteDict = null;
        }

        if (content.Pictures?.Count > 0)
        {
            pictures = content.Pictures.Select(p => p.ImgSrc.ToImage(p.ImgWidth, p.ImgHeight)).ToList();
        }

        return new EmoteText(text, emoteDict, pictures);
    }
}
