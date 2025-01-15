// Copyright (c) Richasy. All rights reserved.

using System;
using System.Text.RegularExpressions;
using Bilibili.App.Interfaces.V1;
using Bilibili.Polymer.App.Search.V1;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Search.Core;

internal static class VideoAdapter
{
    public static VideoInformation ToVideoInformation(this Item item)
    {
        if (item.Av is null)
        {
            throw new KernelException("视频信息不完整");
        }

        var av = item.Av;
        var aid = item.Param;
        var bvid = av.Share.Video.Bvid;
        var cid = av.Share.Video.Cid;

        // 这里的标题可能包含关键字标记，需要去除.
        var title = av.Title.Replace("<em class=\"keyword\">", string.Empty).Replace("</em>", string.Empty);
        var identifier = new MediaIdentifier(aid, title, av.Cover.ToVideoCover());
        var user = UserAdapterBase.CreateUserProfile(av.Mid, av.Author, default, 0d);
        var communityInfo = new VideoCommunityInformation(aid, av.Play, av.Danmaku);
        var duration = av.Duration.ToDurationSeconds();
        var info = new VideoInformation(identifier, new Models.User.PublisherProfile(user), duration, bvid, communityInformation: communityInfo);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Cid, cid);
        info.AddExtensionIfNotNull(VideoExtensionDataId.MediaType, MediaType.Video);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Description, av.Desc);
        info.AddExtensionIfNotNull(VideoExtensionDataId.ShortLink, av.Share.Video.ShortLink);
        return info;
    }

    public static VideoInformation ToVideoInformation(this SearchVideo video)
    {
        var aid = video.aid.ToString();
        var bvid = video.bvid;
        var title = video.title.Replace("<em class=\"keyword\">", string.Empty).Replace("</em>", string.Empty);
        var cover = video.pic?.StartsWith("//") == true ? $"https:{video.pic}" : video.pic;
        var identifier = new MediaIdentifier(aid, title, cover.ToVideoCover());
        var user = UserAdapterBase.CreateUserProfile(video.mid, video.author, video.upic, 64d);
        var communityInfo = new VideoCommunityInformation(aid, video.play, video.danmaku, video.like, video.favorites, commentCount: video.review);
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(video.pubdate);
        var duration = 0;
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception
        try
        {
            var durationSplit = video.duration.Split(':');
            duration = durationSplit.Length >= 2
                ? (int.Parse(durationSplit[0]) * 60) + int.Parse(durationSplit[1])
                : int.Parse(durationSplit[0]);
        }
        catch (Exception)
        {
        }
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception

        var info = new VideoInformation(identifier, new Models.User.PublisherProfile(user), duration, bvid, publishTime, communityInformation: communityInfo);
        info.AddExtensionIfNotNull(VideoExtensionDataId.MediaType, MediaType.Video);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Description, video.desc);
        info.AddExtensionIfNotNull(VideoExtensionDataId.TagName, video.typename);
        info.AddExtensionIfNotNull(VideoExtensionDataId.TagId, video.typeid);
        return info;
    }

    public static VideoInformation ToVideoInformation(this CursorItem cursorItem)
    {
        var video = cursorItem.CardUgc;
        var viewTime = DateTimeOffset.FromUnixTimeSeconds(cursorItem.ViewAt).ToLocalTime();
        var title = Regex.Replace(cursorItem.Title, @"<[^<>]+>", string.Empty);
        var aid = cursorItem.Kid.ToString();
        var bvid = video.Bvid;
        var owner = UserAdapterBase.CreateUserProfile(video.Mid, video.Name, default, 0d);
        var cover = video.Cover.ToVideoCover();
        var identifier = new MediaIdentifier(aid, title, cover);
        var communityInfo = new VideoCommunityInformation(cursorItem.Kid.ToString(), video.View);

        var info = new VideoInformation(
            identifier,
            new(owner),
            video.Duration,
            bvid,
            communityInformation: communityInfo);

        info.AddExtensionIfNotNull(VideoExtensionDataId.CollectTime, viewTime);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Progress, Convert.ToInt32(video.Progress));
        info.AddExtensionIfNotNull(VideoExtensionDataId.MediaType, MediaType.Video);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Cid, video.Cid);

        return info;
    }

    public static VideoInformation ToVideoInformation(this Arc video)
    {
        var archive = video.Archive;
        var title = Regex.Replace(archive.Title, @"<[^<>]+>", string.Empty);
        var id = archive.Aid.ToString();
        var publisher = UserAdapterBase.CreateUserProfile(archive.Author.Mid, archive.Author.Name, archive.Author.Face, 96d);
        var duration = archive.Duration;
        var cover = ImageAdapter.ToVideoCover(archive.Pic);
        var videoStat = archive.Stat;
        var communityInfo = new VideoCommunityInformation(
            videoStat.Aid.ToString(),
            videoStat.View,
            videoStat.Danmaku,
            videoStat.Like,
            favoriteCount: videoStat.Fav,
            coinCount: videoStat.Coin,
            commentCount: videoStat.Reply,
            shareCount: videoStat.Share);
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(archive.Pubdate).DateTime;
        var description = archive.Desc;

        var identifier = new MediaIdentifier(id, title, cover);
        var info = new VideoInformation(
            identifier,
            new(publisher),
            duration,
            default,
            publishTime,
            default,
            communityInfo);
        info.AddExtensionIfNotNull(VideoExtensionDataId.MediaType, Models.MediaType.Video);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Description, description);
        return info;
    }
}
