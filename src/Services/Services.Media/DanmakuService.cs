// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Danmaku;
using Richasy.BiliKernel.Services.Media.Core;

namespace Richasy.BiliKernel.Services.Media;

/// <summary>
/// 弹幕服务.
/// </summary>
public sealed class DanmakuService : IDanmakuService
{
    private readonly DanmakuClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="DanmakuService"/> class.
    /// </summary>
    public DanmakuService(
        BiliHttpClient httpClient,
        BiliAuthenticator authenticator,
        IBiliTokenResolver tokenResolver)
    {
        _client = new DanmakuClient(httpClient, authenticator, tokenResolver);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<DanmakuInformation>> GetSegmentDanmakusAsync(string aid, string cid, int segmentIndex, CancellationToken cancellationToken = default)
        => _client.GetSegmentDanmakusAsync(aid, cid, segmentIndex, cancellationToken);
    
    /// <inheritdoc/>
    public Task SendLiveDanmakuAsync(string content, string roomId, string color, bool isStandardSize = true, DanmakuLocation location = DanmakuLocation.Scroll, CancellationToken cancellationToken = default)
        => _client.SendLiveDanmakuAsync(content, roomId, color, isStandardSize, location, cancellationToken);

    /// <inheritdoc/>
    public Task SendVideoDanmakuAsync(string content, string aid, string cid, int progress, string color, bool isStandardSize = true, DanmakuLocation location = DanmakuLocation.Scroll, CancellationToken cancellationToken = default)
        => _client.SendVideoDanmakuAsync(content, aid, cid, progress, color, isStandardSize, location, cancellationToken);
}
