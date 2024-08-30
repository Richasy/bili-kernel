// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core.Models;

namespace Richasy.BiliKernel.Services.Media.Core;

internal static class LiveAdapter
{
    public static LiveInformation ToLiveInformation(this LiveRoomCard card)
    {
        var title = card.Title;
        var roomId = card.RoomId.ToString();
        var user = UserAdapterBase.CreateUserProfile(card.UserId ?? 0, card.UpName, default, 0d);
        var viewerCount = card.CoverRightContent.Text.ToCountNumber();
        var subtitle = card.CoverLeftContent.Text;
        var cover = card.Cover.ToVideoCover();
        var identifier = new MediaIdentifier(roomId, title, cover);
        var info = new LiveInformation(identifier, user, default);
        info.AddExtensionIfNotNull(LiveExtensionDataId.ViewerCount, viewerCount);
        info.AddExtensionIfNotNull(LiveExtensionDataId.Subtitle, subtitle);
        return info;
    }

    public static LiveInformation ToLiveInformation(this LiveFeedRoom room)
    {
        var title = room.Title;
        var roomId = room.RoomId.ToString();
        var viewerCount = System.Convert.ToDouble(room.ViewerCount);
        var user = UserAdapterBase.CreateUserProfile(room.UserId ?? 0, room.UserName, room.UserAvatar, 48d);
        var cover = room.Cover.ToVideoCover();
        var subtitle = room.AreaName;
        var identifier = new MediaIdentifier(roomId, title, cover);
        var info = new LiveInformation(identifier, user);
        info.AddExtensionIfNotNull(LiveExtensionDataId.ViewerCount, viewerCount);
        info.AddExtensionIfNotNull(LiveExtensionDataId.Subtitle, subtitle);
        return info;
    }

    public static LivePlayerView ToLivePlayerView(this LiveRoomDetailResponse detail)
    {
        var roomInfo = detail.RoomInformation;
        var title = roomInfo.Title;
        var roomId = roomInfo.RoomId.ToString();
        var desc = string.IsNullOrEmpty(roomInfo.Description) ? default : Regex.Replace(roomInfo.Description, @"<[^>]*>", string.Empty);

        if (!string.IsNullOrEmpty(desc) && desc.Length > 100)
        {
            desc = WebUtility.HtmlDecode(desc).Trim();
        }

        if (string.IsNullOrEmpty(desc))
        {
            desc = "暂无直播间介绍";
        }

        var viewerCount = roomInfo.ViewerCount;
        var cover = (roomInfo.Cover ?? roomInfo.Keyframe).ToVideoCover();
        var userInfo = detail.AnchorInformation.UserBasicInformation;
        var userProfile = UserAdapterBase.CreateUserProfile(roomInfo.UserId, userInfo.UserName, userInfo.Avatar, 96d);
        var partition = new LiveTag(roomInfo.AreaId.ToString(), roomInfo.AreaName, default);
        var startTime = DateTimeOffset.FromUnixTimeSeconds(roomInfo.LiveStartTime).ToLocalTime();
        var identifier = new MediaIdentifier(roomId, title, cover);
        var info = new LiveInformation(identifier, userProfile);
        info.AddExtensionIfNotNull(LiveExtensionDataId.ViewerCount, viewerCount);
        info.AddExtensionIfNotNull(LiveExtensionDataId.Description, desc);
        info.AddExtensionIfNotNull(LiveExtensionDataId.IsLiving, roomInfo.LiveStatus == 1);
        info.AddExtensionIfNotNull(LiveExtensionDataId.StartTime, startTime);
        return new LivePlayerView(info, partition);
    }

    public static LiveMediaInformation ToLiveMediaInformation(this LiveAppPlayResponse information)
    {
        if (information.PlayUrlInfo is null)
        {
            return default;
        }

        var id = information.RoomId.ToString();
        var playInfo = information.PlayUrlInfo.PlayUrl;
        var formats = new List<PlayerFormatInformation>();
        foreach (var item in playInfo.Descriptions)
        {
            var desc = item.Description;
            formats.Add(new PlayerFormatInformation(item.Quality, desc, false, false));
        }

        var lines = new List<LiveLineInformation>();
        foreach (var stream in playInfo.StreamList)
        {
            foreach (var format in stream.FormatList)
            {
                foreach (var codec in format.CodecList)
                {
                    var name = codec.CodecName;
                    var urls = codec.Urls.Select(p => new LivePlayUrl(stream.ProtocolName, p.Host, codec.BaseUrl, p.Extra)).ToList();
                    lines.Add(new LiveLineInformation(name, codec.CurrentQuality, codec.AcceptQualities, urls));
                }
            }
        }

        return new LiveMediaInformation(id, formats, lines);
    }

    public static Partition ToPartition(this LiveAreaGroup group)
    {
        var id = group.Id.ToString();
        var name = group.Name;
        var children = group.AreaList.Select(p => p.ToPartition()).ToList();
        return new Partition(id, name, children: children);
    }

    public static Partition ToPartition(this LiveArea area)
    {
        var id = area.Id.ToString();
        var name = area.Name;
        var logo = string.IsNullOrEmpty(area.Cover) ? default : area.Cover.ToImage();
        return new Partition(id, name, logo, parentId: area.ParentId.ToString());
    }

    public static LiveTag ToLiveTag(this LiveAreaDetailTag tag)
    {
        var id = tag.Id.ToString();
        var name = tag.Name;
        var sortType = tag.SortType;
        return new LiveTag(id, name, sortType);
    }
}
