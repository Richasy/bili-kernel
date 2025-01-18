// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.User.Core;

internal sealed class UserCardDetailResponse
{
    [JsonPropertyName("card")]
    public UserCardDetail Card { get; set; }

    [JsonPropertyName("following")]
    public bool Following { get; set; }

    [JsonPropertyName("follower")]
    public int FollowerCount { get; set; }

    [JsonPropertyName("like_num")]
    public int LikeCount { get; set; }
}

internal sealed class UserCardDetail
{
    [JsonPropertyName("mid")]
    public string Mid { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("face")]
    public string Face { get; set; }

    [JsonPropertyName("fans")]
    public int Fans { get; set; }

    [JsonPropertyName("attention")]
    public int Attention { get; set; }

    [JsonPropertyName("sign")]
    public string Sign { get; set; }

    [JsonPropertyName("level_info")]
    public UserLevelInfo Level { get; set; }

    [JsonPropertyName("vip")]
    public Vip Vip { get; set; }

    [JsonPropertyName("is_senior_member")]
    public int IsSeniorMember { get; set; }
}

internal sealed class UserLevelInfo
{
    [JsonPropertyName("current_level")]
    public int CurrentLevel { get; set; }
}
