// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Content;
using RichasyKernel;
using System.Text.Json;

namespace Richasy.BiliKernel.Http;

public sealed partial class BiliHttpClient
{
    private readonly HttpClient _client;

    /// <summary>
    /// 从响应中获取 Cookie.
    /// </summary>
    /// <param name="response">响应结果.</param>
    /// <returns>Cookie 列表.</returns>
    public static Dictionary<string, string> GetCookiesFromResponse(HttpResponseMessage response)
    {
        Verify.NotNull(response, nameof(response));
        var cookies = response.Headers.GetValues("Set-Cookie");
        return cookies.Select(p => p.Split(';')[0].Trim().Split('=')).ToDictionary(p => p[0], p => p[1]);
    }

    private static async Task ThrowIfResponseInvalidAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        response.Headers.TryGetValues("Content-Type", out var contentTypes);
        var contentType = contentTypes?.FirstOrDefault() ?? string.Empty;
        if (contentType.Contains("image"))
        {
            return;
        }
        else if (contentType.Contains("grpc"))
        {
            var bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            if (bytes.Length < 5)
            {
                throw new KernelException("哔哩哔哩返回了一个空的响应");
            }

            return;
        }

        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            throw new KernelException("哔哩哔哩返回了一个空的响应");
        }

        var responseObj = JsonSerializer.Deserialize(responseContent, BiliResponseContext.Default.BiliResponse);
        if (!responseObj.IsSuccess())
        {
            throw new KernelException($"哔哩哔哩返回了一个异常响应: {responseObj.Message ?? "N/A"}", new Exception(responseContent));
        }
    }
}
