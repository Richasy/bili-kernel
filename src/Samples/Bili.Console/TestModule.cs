// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;
using RichasyKernel;
using Spectre.Console;

namespace Bili.Console;

internal sealed class TestModule : IFeatureModule
{
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly IPlayerService _playerService;

    public TestModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _playerService = kernel.GetRequiredService<IPlayerService>();
    }

    public async Task RunAsync()
    {
        AnsiConsole.MarkupLine("开始测试...");
        var video = new MediaIdentifier("BV1nb421B7cg", default, default);
        var info = await _playerService.GetVideoPageDetailAsync(video, _cancellationToken).ConfigureAwait(true);
        AnsiConsole.MarkupLine($"获取视频详情成功，标题为：[green]{info.Information.Identifier.Title}[/]");

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }
}
