// Copyright (c) Richasy. All rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Richasy.BiliKernel.Text;

/// <summary>
/// Represents a line of a Server-Sent Events (SSE) stream.
/// </summary>
/// <remarks>
/// <a href="https://html.spec.whatwg.org/multipage/server-sent-events.html#parsing-an-event-stream">SSE specification</a>
/// </remarks>
[ExcludeFromCodeCoverage]
internal readonly struct SseLine : IEquatable<SseLine>
{
    private readonly string _original;
    private readonly int _colonIndex;
    private readonly int _valueIndex;

    /// <summary>
    /// Represents an empty SSE line.
    /// </summary>
    /// <remarks>
    /// The <see cref="Empty"/> property is a static instance of the <see cref="SseLine"/> struct.
    /// </remarks>
    internal static SseLine Empty { get; } = new(string.Empty, 0, false, null);

    internal SseLine(string original, int colonIndex, bool hasSpaceAfterColon, string? lastEventName)
    {
        _original = original;
        _colonIndex = colonIndex;
        _valueIndex = colonIndex >= 0 ? colonIndex + (hasSpaceAfterColon ? 2 : 1) : -1;
        if (_valueIndex >= _original.Length)
        {
            _valueIndex = -1;
        }

        EventName = lastEventName;
    }

    /// <summary>
    /// The name of the last event for the Server-Sent Events (SSE) line.
    /// </summary>
    public string? EventName { get; }

    /// <summary>
    /// Determines whether the SseLine is empty.
    /// </summary>
    public bool IsEmpty => _original.Length == 0;

    /// <summary>
    /// Gets a value indicating whether the value of the SseLine is empty.
    /// </summary>
    public bool IsValueEmpty => _valueIndex < 0;

    /// <summary>
    /// Determines whether the SseLine is comment line.
    /// </summary>
    public bool IsComment => !IsEmpty && _original[0] == ':';

    /// <summary>
    /// Represents a field name in a Server-Sent Events (SSE) line.
    /// </summary>
    public ReadOnlyMemory<char> FieldName => _colonIndex >= 0 ? _original.AsMemory(0, _colonIndex) : _original.AsMemory();

    /// <summary>
    /// Represents a field value in Server-Sent Events (SSE) format.
    /// </summary>
    public ReadOnlyMemory<char> FieldValue => _valueIndex >= 0 ? _original.AsMemory(_valueIndex) : string.Empty.AsMemory();

    /// <inheritdoc />
    public override string ToString() => _original;

    /// <inheritdoc />
    public bool Equals(SseLine other) => _original.Equals(other._original, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is SseLine other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(_original);

    /// <summary>
    /// Defines the equality operator for comparing two instances of the SseLine class.
    /// </summary>
    public static bool operator ==(SseLine left, SseLine right) => left.Equals(right);

    /// <summary>
    /// Represents the inequality operator for comparing two SseLine objects.
    /// </summary>
    public static bool operator !=(SseLine left, SseLine right) => !left.Equals(right);
}
