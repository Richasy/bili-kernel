﻿// Copyright (c) Richasy. All rights reserved.
// <auto-generated />
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace Richasy.BiliKernel.Services.Article.Core.Models;

internal sealed class ArticleViewResponse
{
    public int like { get; set; }
    public bool attention { get; set; }
    public bool favorite { get; set; }
    public int coin { get; set; }
    public Stats stats { get; set; }
    public string title { get; set; }
    public string banner_url { get; set; }
    public long mid { get; set; }
    public string author_name { get; set; }
    public bool is_author { get; set; }
    public string[] image_urls { get; set; }
    public string[] origin_image_urls { get; set; }
    public bool shareable { get; set; }
    public bool show_later_watch { get; set; }
    public bool show_small_window { get; set; }
    public bool in_list { get; set; }
    public long pre { get; set; }
    public long next { get; set; }
    public int type { get; set; }
    public string video_url { get; set; }
    public string location { get; set; }
    public bool disable_share { get; set; }
}

internal sealed class Stats
{
    public int view { get; set; }
    public int favorite { get; set; }
    public int like { get; set; }
    public int dislike { get; set; }
    public int reply { get; set; }
    public int share { get; set; }
    public int coin { get; set; }
    public int dynamic { get; set; }
}