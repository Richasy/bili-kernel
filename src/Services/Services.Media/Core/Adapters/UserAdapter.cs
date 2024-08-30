// Copyright (c) Richasy. All rights reserved.

using Bilibili.App.Archive.V1;
using Bilibili.App.View.V1;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.User;
using Richasy.BiliKernel.Services.Media.Core.Models;

namespace Richasy.BiliKernel.Services.Media.Core;

internal static class UserAdapter
{
    public static PublisherProfile ToPublisherProfile(this PublisherInfo info)
    {
        var user = UserAdapterBase.CreateUserProfile(info.UserId, info.Publisher, info.PublisherAvatar, 48d);
        return new PublisherProfile(user);
    }

    public static PublisherProfile ToPublisherProfile(this Author author)
        => new PublisherProfile(UserAdapterBase.CreateUserProfile(author.Mid, author.Name, author.Face, 96d));

    public static PublisherProfile ToPublisherProfile(this Staff staff)
    {
        var user = UserAdapterBase.CreateUserProfile(staff.Mid, staff.Name, staff.Face, 96d);
        return new PublisherProfile(user, staff.Title);
    }

    public static UserCommunityInformation ToUserCommunityInformation(this ViewReply reply)
    {
        var relation = UserRelationStatus.Unfollow;
        if (reply.ReqUser.Attention == 1)
        {
            relation = reply.ReqUser.GuestAttention == 1
                ? UserRelationStatus.Friends
                : UserRelationStatus.Following;
        }
        else if (reply.ReqUser.GuestAttention == 1)
        {
            relation = UserRelationStatus.BeFollowed;
        }

        return new UserCommunityInformation(reply.Arc.Author.Mid.ToString(), relation: relation);
    }

    public static UserCommunityInformation ToUserCommunityInformation(this VideoPageAuthor author)
    {
        var relation = author.following
            ? UserRelationStatus.Following
            : UserRelationStatus.Unfollow;
        return new UserCommunityInformation(author.card.mid.ToString(), relation: relation);
    }

    public static PublisherProfile ToPublisherProfile(this PgcCelebrity celebrity)
    {
        var user = UserAdapterBase.CreateUserProfile(celebrity.Id, celebrity.Name, celebrity.Avatar, 96d);
        return new PublisherProfile(user, celebrity.ShortDescription);
    }
}
