// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Bilibili.Polymer.App.Search.V1;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Search.Core;

internal static class LiveAdapter
{
    public static LiveInformation? ToLiveInformation(this Item item)
    {
        var live = item.LiveRoom;
        if (live is null)
        {
            return default;
        }

        var identifier = new MediaIdentifier(live.Roomid.ToString(), live.Title, live.Cover.ToVideoCover());
        var user = UserAdapterBase.CreateUserProfile(live.Mid, live.Name, default, 0d);
        var info = new LiveInformation(identifier, user);
        info.AddExtensionIfNotNull(LiveExtensionDataId.IsLiving, live.LiveStatus == 1);
        info.AddExtensionIfNotNull(LiveExtensionDataId.ViewerCount, System.Convert.ToDouble(live.Online));
        info.AddExtensionIfNotNull(LiveExtensionDataId.TagName, live.AreaV2Name);
        return info;
    }
}
