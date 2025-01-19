// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Google.Protobuf;
using RichasyKernel;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Richasy.BiliKernel.Http;

/// <summary>
/// BiliBili 网络请求客户端.
/// </summary>
public sealed partial class BiliHttpClient : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BiliHttpClient"/> class.
    /// </summary>
    public BiliHttpClient()
    {
        _client = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = false,
            UseCookies = true,
            AutomaticDecompression = (DecompressionMethods.Deflate | DecompressionMethods.GZip),
            SslProtocols = SslProtocols.None,
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        }, disposeHandler: true);
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.69");
        _client.Timeout = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// 创建一个请求.
    /// </summary>
    /// <param name="method">请求方法.</param>
    /// <param name="uri">请求地址.</param>
    /// <returns><see cref="HttpRequestMessage"/>.</returns>
    public static HttpRequestMessage CreateRequest(HttpMethod method, Uri uri)
        => new HttpRequestMessage(method, uri);

    /// <summary>
    /// 创建一个 GRPC 请求.
    /// </summary>
    /// <param name="uri">请求地址.</param>
    /// <param name="grpcMessage">消息结构体.</param>
    /// <returns>请求信息.</returns>
    public static HttpRequestMessage CreateRequest(Uri uri, IMessage grpcMessage)
    {
        var request = CreateRequest(HttpMethod.Post, uri);
        var messageBytes = grpcMessage.ToByteArray();
        var stateBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, Convert.ToByte(messageBytes.Length) };
        var bytes = stateBytes.Concat(messageBytes).ToArray();
        var byteArrayContent = new ByteArrayContent(bytes);
        byteArrayContent.Headers.ContentType = new("application/grpc");
        byteArrayContent.Headers.ContentLength = bytes.Length;
        request.Content = byteArrayContent;
        return request;
    }

    /// <summary>
    /// 发送请求.
    /// </summary>
    /// <param name="request">请求信息.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns>响应结果.</returns>
    /// <exception cref="KernelException"></exception>
    /// <exception cref="KernelException"></exception>
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        Verify.NotNull(request, nameof(request));
        try
        {
            var response = await _client.SendAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            await ThrowIfResponseInvalidAsync(response).ConfigureAwait(false);
            return response;
        }
        catch (KernelException)
        {
            throw;
        }
        catch (HttpRequestException httpEx)
        {
            throw new KernelException(httpEx.Message, httpEx);
        }
        catch (Exception ex)
        {
            throw new KernelException($"未知错误：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 直接获取字符串.
    /// </summary>
    public async Task<string> GetStringAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        Verify.NotNull(request, nameof(request));
        var response = await _client.SendAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// 从响应中获取数据.
    /// </summary>
    /// <typeparam name="T">数据类型.</typeparam>
    /// <param name="response">响应.</param>
    /// <param name="deserializeContext">反序列化上下文（用于AOT）.</param>
    /// <returns><see cref="Task{T}"/></returns>
    public static async Task<T> ParseAsync<T>(HttpResponseMessage response, JsonTypeInfo<T> deserializeContext)
    {
        Verify.NotNull(response, nameof(response));
        var contentText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(contentText, deserializeContext);
    }

    /// <summary>
    /// 从 Protobuf 响应中获取数据.
    /// </summary>
    /// <typeparam name="T">数据类型.</typeparam>
    /// <param name="response">响应.</param>
    /// <param name="parser">解析器.</param>
    /// <returns><see cref="Task{T}"/></returns>
    public static async Task<T> ParseAsync<T>(HttpResponseMessage response, MessageParser<T> parser)
        where T : IMessage<T>
    {
        Verify.NotNull(response, nameof(response));
        Verify.NotNull(parser, nameof(parser));
        var bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        return parser.ParseFrom([.. bytes.Skip(5)]);
    }

    /// <inheritdoc/>
    public void Dispose()
        => _client?.Dispose();
}
