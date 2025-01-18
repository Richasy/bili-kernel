// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Bilibili.Main.Community.Reply.V1;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Comment;
using Richasy.BiliKernel.Services.Comment.Core.Adapters;
using RichasyKernel;

namespace Richasy.BiliKernel.Services.Comment.Core;

/// <summary>
/// 评论客户端.
/// </summary>
internal sealed class CommentClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BiliAuthenticator _authenticator;

    public CommentClient(
        BiliHttpClient httpClient,
        BiliAuthenticator authenticator)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
    }

    public async Task LiekCommentAsync(bool isLike, string replyId, string targetId, CommentTargetType type, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "od", targetId },
            { "action", isLike ? "1" : "0" },
            { "type", ((int)type).ToString() },
            { "rpid", replyId }
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new System.Uri(BiliApis.Community.LikeReply));
        _authenticator.AuthorizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<EmotePackage>> GetEmotePackagesAsync(CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "business", "reply" },
        };
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new System.Uri(BiliApis.Community.Emotes));
        _authenticator.AuthorizeRestRequest(request, parameters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponseEmoteResponse).ConfigureAwait(false);
        return responseObj.Data.Packages.Select(p => p.ToEmotePackage()).ToList()
            ?? throw new KernelException("获取表情包失败");
    }

    public async Task<CommentView> GetCommentsAsync(
        string targetId,
        CommentTargetType type,
        CommentSortType sort,
        long offset = 0,
        CancellationToken cancellationToken = default)
    {
        var cursor = new CursorReq
        {
            Mode = sort == CommentSortType.Time ? Mode.MainListTime : Mode.MainListHot,
            Next = offset,
            Prev = 0,
        };

        var req = new MainListReq
        {
            Cursor = cursor,
            Oid = Convert.ToInt64(targetId),
            Type = (int)type,
            Rpid = 0,
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.ReplyMainList), req);
        _authenticator.AuthorizeGrpcRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, MainListReply.Parser).ConfigureAwait(false);
        return responseObj?.ToCommentView(targetId, type)
            ?? throw new KernelException("获取评论失败");
    }

    public async Task<CommentView> GetDetailCommentsAsync(
        string targetId,
        CommentTargetType type,
        CommentSortType sort,
        string rootId,
        long offset = 0,
        CancellationToken cancellationToken = default)
    {
        var cursor = new CursorReq
        {
            Mode = sort == CommentSortType.Time ? Mode.MainListTime : Mode.MainListHot,
            Next = offset,
            Prev = 0,
        };

        var req = new DetailListReq
        {
            Scene = DetailListScene.Reply,
            Cursor = cursor,
            Oid = Convert.ToInt64(targetId),
            Root = Convert.ToInt64(rootId),
            Type = (int)type,
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.ReplyDetailList), req);
        _authenticator.AuthorizeGrpcRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, DetailListReply.Parser).ConfigureAwait(false);
        return responseObj?.ToCommentView(targetId, type)
            ?? throw new KernelException("获取评论失败");
    }

    public async Task SendTextCommentAsync(string message, string targetId, CommentTargetType type, string rootId, string parentId, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "message", message },
            { "oid", targetId },
            { "type", ((int)type).ToString() },
            { "plat", "3" },
            { "root", rootId },
            { "parent", parentId },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Community.AddReply));
        _authenticator.AuthorizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
