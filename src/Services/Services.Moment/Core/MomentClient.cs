// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Bilibili.App.Dynamic.V2;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Moment;
using Richasy.BiliKernel.Models.User;
using RichasyKernel;

namespace Richasy.BiliKernel.Services.Moment.Core;

internal sealed class MomentClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BiliAuthenticator _authenticator;
    private readonly IBiliTokenResolver _tokenResolver;

    public MomentClient(
        BiliHttpClient httpClient,
        BiliAuthenticator authenticator,
        IBiliTokenResolver tokenResolver)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
        _tokenResolver = tokenResolver;
    }

    public async Task<(IReadOnlyList<MomentInformation> Moments, string Offset, bool HasMore)> GetUserComprehensiveMomentsAsync(UserProfile user, string? offset = default, CancellationToken cancellationToken = default)
    {
        var req = new DynAllPersonalReq
        {
            HostUid = Convert.ToInt64(user.Id),
            IsPreload = 0,
            Offset = offset ?? string.Empty,
            LocalTime = BasicExtensions.GetLocalTimeZoneNumber(),
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.DynamicAllPersonal), req);
        _authenticator.AuthorizeGrpcRequest(request, false);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, DynAllPersonalReply.Parser).ConfigureAwait(false);
        var moments = responseObj.List.Select(p => p.ToMomentInformation()).Where(p => p is not null).ToList().AsReadOnly();
        return moments.Count == 0
            ? throw new KernelException("没有获取到动态信息，请稍后重试")
            : (moments, responseObj.Offset, responseObj.HasMore);
    }

    public async Task<(IReadOnlyList<MomentInformation> Moments, string Offset, bool HasMore)> GetUserVideoMomentsAsync(UserProfile user, string? offset = default, CancellationToken cancellationToken = default)
    {
        var req = new DynVideoPersonalReq
        {
            HostUid = Convert.ToInt64(user.Id),
            IsPreload = 0,
            Offset = offset ?? string.Empty,
            LocalTime = BasicExtensions.GetLocalTimeZoneNumber(),
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.DynamicVideoPersonal), req);
        _authenticator.AuthorizeGrpcRequest(request, false);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, DynAllPersonalReply.Parser).ConfigureAwait(false);
        var moments = responseObj.List.Select(p => p.ToMomentInformation()).Where(p => p is not null).ToList().AsReadOnly();
        return moments.Count == 0
            ? throw new KernelException("没有获取到动态信息，请稍后重试")
            : (moments, responseObj.Offset, responseObj.HasMore);
    }

    public async Task<(IReadOnlyList<MomentInformation> Moments, string Offset, bool HasMore)> GetMyMomentsAsync(string? offset = default, CancellationToken cancellationToken = default)
    {
        var req = new DynSpaceReq
        {
            HostUid = _tokenResolver.GetToken().UserId,
            HistoryOffset = offset ?? string.Empty,
            LocalTime = BasicExtensions.GetLocalTimeZoneNumber(),
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.DynamicSpace), req);
        _authenticator.AuthorizeGrpcRequest(request, false);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, DynSpaceRsp.Parser).ConfigureAwait(false);
        var moments = responseObj.List.Select(p => p.ToMomentInformation()).Where(p => p is not null).ToList().AsReadOnly();
        return moments.Count == 0
            ? throw new KernelException("没有获取到动态信息，请稍后重试")
            : (moments, responseObj.HistoryOffset, responseObj.HasMore);
    }

    public async Task<MomentView> GetComprehensiveMomentsAsync(string? offset = default, string? baseline = default, CancellationToken cancellationToken = default)
    {
        var type = string.IsNullOrEmpty(offset) ? Refresh.New : Refresh.History;
        var req = new DynAllReq
        {
            RefreshType = type,
            Offset = offset ?? string.Empty,
            UpdateBaseline = baseline ?? string.Empty,
            LocalTime = BasicExtensions.GetLocalTimeZoneNumber(),
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.DynamicAll), req);
        _authenticator.AuthorizeGrpcRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, DynAllReply.Parser).ConfigureAwait(false);
        var moments = responseObj.DynamicList.List.Select(p => p.ToMomentInformation()).Where(p => p is not null).ToList().AsReadOnly();
        var nextOffset = responseObj.DynamicList?.HistoryOffset;
        var nextBaseline = responseObj.DynamicList?.UpdateBaseline;
        var ups = responseObj.UpList?.List.Select(p => p.ToMomentProfile()).ToList().AsReadOnly();
        var hasMore = responseObj.DynamicList?.HasMore;
        return new MomentView(moments, ups, nextOffset, nextBaseline, hasMore);
    }

    public async Task<MomentView> GetVideoMomentsAsync(string? offset = default, string? baseline = default, CancellationToken cancellationToken = default)
    {
        var type = string.IsNullOrEmpty(offset) ? Refresh.New : Refresh.History;
        var req = new DynVideoReq
        {
            RefreshType = type,
            Offset = offset ?? string.Empty,
            UpdateBaseline = baseline ?? string.Empty,
            LocalTime = BasicExtensions.GetLocalTimeZoneNumber()
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.DynamicVideo), req);
        _authenticator.AuthorizeGrpcRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, DynVideoReply.Parser).ConfigureAwait(false);
        var moments = responseObj.DynamicList.List.Where(p =>
            p.CardType is DynamicType.Av
            or DynamicType.Pgc
            or DynamicType.UgcSeason)
            .Select(p => p.ToMomentInformation())
            .Where(p => p is not null)
            .ToList()
            .AsReadOnly();
        var nextOffset = responseObj.DynamicList?.HistoryOffset;
        var nextBaseline = responseObj.DynamicList?.UpdateBaseline;
        var ups = responseObj.VideoUpList?.List.Select(p => p.ToMomentProfile()).ToList().AsReadOnly();
        var hasMore = responseObj.DynamicList?.HasMore;
        return new MomentView(moments, ups, nextOffset, nextBaseline, hasMore);
    }

    public async Task LikeMomentAsync(MomentInformation moment, bool isLike, CancellationToken cancellationToken)
    {
        var req = new DynThumbReq
        {
            Type = isLike ? ThumbType.Thumb : ThumbType.Cancel,
            DynId = Convert.ToInt64(moment.Id),
            Uid = Convert.ToInt64(moment.User.Id),
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.LikeDynamic), req);
        _authenticator.AuthorizeGrpcRequest(request);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task MarkAsReadAsync(string userId, string? offset, CancellationToken cancellationToken)
    {
        var req = new DynAllUpdOffsetReq
        {
            HostUid = Convert.ToInt64(userId),
            ReadOffset = offset ?? string.Empty,
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.DynamicSpaceMarkRead), req);
        _authenticator.AuthorizeGrpcRequest(request);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
