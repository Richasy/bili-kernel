﻿// Copyright (c) Richasy. All rights reserved.
// <auto-generated />

using System.Collections.Generic;

namespace Richasy.BiliKernel.Services.Media.Core.Models;

internal sealed class VideoPageResponse
{
    public VideoPageVideo View { get; set; }
    public VideoPageAuthor Card { get; set; }
    public IList<VideoPageTag> Tags { get; set; }
    public IList<VideoPageRelated> Related { get; set; }
}

internal sealed class VideoPageVideo
{
    public string bvid { get; set; }
    public long aid { get; set; }
    public int videos { get; set; }
    public int tid { get; set; }
    public string tname { get; set; }
    public int copyright { get; set; }
    public string pic { get; set; }
    public string title { get; set; }
    public int pubdate { get; set; }
    public int ctime { get; set; }
    public string desc { get; set; }
    public int state { get; set; }
    public int duration { get; set; }
    public PublisherInfo owner { get; set; }
    public VideoStatusInfo stat { get; set; }
    public UgcSeason ugc_season { get; set; }
    public string dynamic { get; set; }
    public long cid { get; set; }
    public Dimension dimension { get; set; }
    public int teenage_mode { get; set; }
    public bool is_chargeable_season { get; set; }
    public bool is_story { get; set; }
    public bool is_upower_exclusive { get; set; }
    public bool is_upower_play { get; set; }
    public bool is_upower_preview { get; set; }
    public int enable_vt { get; set; }
    public string vt_display { get; set; }
    public bool no_cache { get; set; }
    public IList<VideoPagePart> pages { get; set; }
    public bool need_jump_bv { get; set; }
    public bool disable_show_up_info { get; set; }
    public int is_story_play { get; set; }
    public Rights rights { get; set; }

    internal sealed class Rights
    {
        public int is_stein_gate { get; set; }
    }
}

internal sealed class Dimension
{
    public int width { get; set; }
    public int height { get; set; }
    public int rotate { get; set; }
}

internal sealed class VideoPagePart
{
    public long cid { get; set; }
    public int page { get; set; }
    public string from { get; set; }
    public string part { get; set; }
    public int duration { get; set; }
    public string vid { get; set; }
    public string weblink { get; set; }
    public Dimension dimension { get; set; }
    public string first_frame { get; set; }
}

internal sealed class VideoPageAuthor
{
    public VideoPageAuthorDetail card { get; set; }
    public bool following { get; set; }
    public int archive_count { get; set; }
    public int article_count { get; set; }
    public int follower { get; set; }
    public int like_num { get; set; }
}

internal sealed class VideoPageAuthorDetail
{
    public string mid { get; set; }
    public string name { get; set; }
    public bool approve { get; set; }
    public string sex { get; set; }
    public string rank { get; set; }
    public string face { get; set; }
    public int face_nft { get; set; }
    public int face_nft_type { get; set; }
    public string DisplayRank { get; set; }
    public int regtime { get; set; }
    public int spacesta { get; set; }
    public string birthday { get; set; }
    public string place { get; set; }
    public string description { get; set; }
    public int article { get; set; }
    public object[] attentions { get; set; }
    public int fans { get; set; }
    public int friend { get; set; }
    public int attention { get; set; }
    public string sign { get; set; }
    public Level_Info level_info { get; set; }
    public Vip vip { get; set; }
    public int is_senior_member { get; set; }
    public object name_render { get; set; }

    internal sealed class Level_Info
    {
        public int current_level { get; set; }
        public int current_min { get; set; }
        public int current_exp { get; set; }
        public int next_exp { get; set; }
    }
}

internal sealed class Vip
{
    public int type { get; set; }
}

internal sealed class VideoPageTag
{
    public int tag_id { get; set; }
    public string tag_name { get; set; }
    public string music_id { get; set; }
    public string tag_type { get; set; }
    public string jump_url { get; set; }
}

internal sealed class VideoPageRelated
{
    public long aid { get; set; }
    public int videos { get; set; }
    public int tid { get; set; }
    public string tname { get; set; }
    public int copyright { get; set; }
    public string pic { get; set; }
    public string title { get; set; }
    public int pubdate { get; set; }
    public int ctime { get; set; }
    public string desc { get; set; }
    public int state { get; set; }
    public int duration { get; set; }
    public PublisherInfo owner { get; set; }
    public VideoStatusInfo stat { get; set; }
    public string dynamic { get; set; }
    public long cid { get; set; }
    public Dimension dimension { get; set; }
    public string short_link_v2 { get; set; }
    public int up_from_v2 { get; set; }
    public string first_frame { get; set; }
    public string pub_location { get; set; }
    public string cover43 { get; set; }
    public string bvid { get; set; }
    public int season_type { get; set; }
    public bool is_ogv { get; set; }
    public object ogv_info { get; set; }
    public string rcmd_reason { get; set; }
    public int enable_vt { get; set; }
    public int mission_id { get; set; }
    public int season_id { get; set; }
}

internal sealed class UgcSeason
{
    public int id { get; set; }
    public string title { get; set; }
    public string cover { get; set; }
    public long mid { get; set; }
    public string intro { get; set; }
    public int sign_state { get; set; }
    public int attribute { get; set; }
    public List<VideoPageSection> sections { get; set; }
    public int ep_count { get; set; }
    public int season_type { get; set; }
    public bool is_pay_season { get; set; }
    public int enable_vt { get; set; }
}

internal sealed class VideoPageSection
{
    public int season_id { get; set; }
    public int id { get; set; }
    public string title { get; set; }
    public int type { get; set; }
    public List<VideoEpisode> episodes { get; set; }
}

internal sealed class VideoEpisode
{
    public int season_id { get; set; }
    public int section_id { get; set; }
    public int id { get; set; }
    public long aid { get; set; }
    public long cid { get; set; }
    public string title { get; set; }
    public int attribute { get; set; }
    public VideoEpisodeArc arc { get; set; }
    public string bvid { get; set; }
}

internal sealed class VideoEpisodeArc
{
    public long aid { get; set; }
    public int videos { get; set; }
    public int copyright { get; set; }
    public string pic { get; set; }
    public string title { get; set; }
    public int pubdate { get; set; }
    public int ctime { get; set; }
    public string desc { get; set; }
    public int state { get; set; }
    public int duration { get; set; }
    public PublisherInfo author { get; set; }
    public VideoStatusInfo stat { get; set; }
    public string dynamic { get; set; }
    public bool is_chargeable_season { get; set; }
    public bool is_blooper { get; set; }
    public int enable_vt { get; set; }
    public string vt_display { get; set; }
}
