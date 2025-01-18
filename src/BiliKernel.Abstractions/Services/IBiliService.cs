// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Services;

/// <summary>
/// Represents an Bili service.
/// </summary>
public interface IBiliService
{
    /// <summary>
    /// Gets the Bili service attributes.
    /// </summary>
    IReadOnlyDictionary<string, object?> Attributes { get; }
}
