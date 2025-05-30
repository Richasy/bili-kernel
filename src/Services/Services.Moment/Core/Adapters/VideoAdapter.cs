﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Bilibili.App.Dynamic.V2;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.Media;
using RichasyKernel;

namespace Richasy.BiliKernel.Services.Moment.Core;

internal static class VideoAdapter
{
    public static VideoInformation ToVideoInformation(this MdlDynArchive archive)
    {
        if (archive.IsPGC)
        {
            throw new KernelException($"该视频 {archive.Title} 是 PGC 内容，请使用 PGC 适配器转换");
        }

        var title = archive.Title;
        if (string.IsNullOrEmpty(title))
        {
            title = "动态视频";
        }

        var id = archive.Avid;
        var bvid = archive.Bvid;
        var duration = archive.Duration;
        var cover = archive.Cover.ToImage(600d, 400d);
        double playCount = archive.View;
        if (playCount == 0 && !string.IsNullOrEmpty(archive.CoverLeftText2))
        {
            playCount = archive.CoverLeftText2.ToCountNumber("观看");
        }
        var danmakuCount = archive.CoverLeftText1.Contains("弹幕")
            ? archive.CoverLeftText1.ToCountNumber("弹幕")
            : archive.CoverLeftText3.ToCountNumber("弹幕");
        var communityInfo = new VideoCommunityInformation(archive.Avid.ToString(), playCount, danmakuCount);
        var identifier = new MediaIdentifier(id.ToString(), title, cover);
        var info = new VideoInformation(identifier, default, duration, bvid, communityInformation: communityInfo);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Cid, archive.Cid);
        info.AddExtensionIfNotNull(VideoExtensionDataId.MediaType, Models.MediaType.Video);
        return info;
    }
}
