// Copyright (c) Richasy. All rights reserved.

using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using Richasy.BiliKernel.Models.Appearance;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 媒体标识，表示视频/影视/直播的核心信息.
/// </summary>
[JsonConverter(typeof(MediaIdentifierJsonConverter))]
public readonly struct MediaIdentifier
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediaIdentifier"/> struct.
    /// </summary>
    /// <param name="id">媒体 Id.</param>
    /// <param name="title">媒体名称.</param>
    /// <param name="cover">封面.</param>
    public MediaIdentifier(string id, string? title, BiliImage? cover)
    {
        Id = id;
        Title = title;
        Cover = cover;
    }

    /// <summary>
    /// 媒体标题.
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// 媒体封面.
    /// </summary>
    public BiliImage? Cover { get; }

    /// <summary>
    /// 媒体 Id，属于网站的资源标识符.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Equal
    /// </summary>
    public static bool operator ==(MediaIdentifier left, MediaIdentifier right)
        => left.Equals(right);

    /// <summary>
    /// Not equal.
    /// </summary>
    public static bool operator !=(MediaIdentifier left, MediaIdentifier right)
        => !(left == right);

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is MediaIdentifier identifier && Id == identifier.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);

    /// <inheritdoc/>
    public override string ToString()
        => $"{Title} | {Id}";
}

internal sealed class MediaIdentifierJsonConverter : JsonConverter<MediaIdentifier>
{
    public override MediaIdentifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        string? id = null;
        string? title = null;
        BiliImage? cover = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            reader.Read();

            switch (propertyName)
            {
                case nameof(MediaIdentifier.Id):
                    id = reader.GetString();
                    break;
                case nameof(MediaIdentifier.Title):
                    title = reader.GetString();
                    break;
                case nameof(MediaIdentifier.Cover):
                    cover = JsonSerializer.Deserialize<BiliImage>(ref reader, options);
                    break;
            }
        }

        if (id == null)
        {
            throw new JsonException();
        }

        return new MediaIdentifier(id, title, cover);
    }

    public override void Write(Utf8JsonWriter writer, MediaIdentifier value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString(nameof(MediaIdentifier.Id), value.Id);
        writer.WriteString(nameof(MediaIdentifier.Title), value.Title);
        writer.WritePropertyName(nameof(MediaIdentifier.Cover));
        JsonSerializer.Serialize(writer, value.Cover, options);
        writer.WriteEndObject();
    }
}
