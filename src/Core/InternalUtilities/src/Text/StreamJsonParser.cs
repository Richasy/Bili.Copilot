// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Richasy.BiliKernel.Text;

#pragma warning disable CA1812 // Internal class that is apparently never instantiated
#pragma warning disable CA1846 // Prefer 'AsSpan' over 'Substring' when span-based overloads are available

/// <summary>
/// Internal class for parsing a stream of text which contains a series of discrete JSON strings into en enumerable containing each separate JSON string.
/// </summary>
/// <remarks>
/// This is universal parser for parsing stream of text which contains a series of discrete JSON.<br/>
/// If you need a specialized SSE parser, use <see cref="SseJsonParser"/> instead.<br/>
/// This class is thread-safe.
/// </remarks>
[ExcludeFromCodeCoverage]
internal sealed class StreamJsonParser
{
    /// <summary>
    /// Parses a Stream containing JSON data and yields the individual JSON objects.
    /// </summary>
    /// <param name="stream">The Stream containing the JSON data.</param>
    /// <param name="validateJson">Set to true to enable checking json chunks are well-formed. Default is false.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An enumerable collection of string representing the individual JSON objects.</returns>
    /// <remarks>Stream will be disposed after parsing.</remarks>
    public static async IAsyncEnumerable<string> ParseAsync(
        Stream stream,
        bool validateJson = false,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);
        ChunkParser chunkParser = new(reader);
        while (await chunkParser.ExtractNextChunkAsync(validateJson, cancellationToken).ConfigureAwait(false) is { } json)
        {
            yield return json;
        }
    }

    private sealed class ChunkParser
    {
        private readonly StringBuilder _jsonBuilder = new();
        private readonly StreamReader _reader;

        private int _bracketsCount;
        private int _startBracketIndex = -1;
        private bool _insideQuotes;
        private bool _isEscaping;
        private bool _isCompleteJson;
        private char _currentCharacter;
        private string? _lastLine;

        internal ChunkParser(StreamReader reader)
        {
            _reader = reader;
        }

        internal async Task<string?> ExtractNextChunkAsync(
            bool validateJson,
            CancellationToken cancellationToken)
        {
            ResetState();
            string? line;
            while ((line = await _reader.ReadLineAsync(
#if NET
                cancellationToken
#endif
                ).ConfigureAwait(false)) is not null || _lastLine is not null)
            {
                if (_lastLine is not null)
                {
                    line = _lastLine + line;
                    _lastLine = null;
                }

                if (ProcessLineUntilCompleteJson(line!))
                {
                    return GetJsonString(validateJson);
                }

                AppendLine(line!);
            }

            return null;
        }

        private bool ProcessLineUntilCompleteJson(string line)
        {
            for (var i = 0; i < line!.Length; i++)
            {
                _currentCharacter = line[i];

                if (IsEscapedCharacterInsideQuotes())
                {
                    continue;
                }

                DetermineIfQuoteStartOrEnd();
                HandleCurrentCharacterOutsideQuotes(i);

                if (_isCompleteJson)
                {
                    var nextIndex = i + 1;
                    if (nextIndex < line.Length)
                    {
                        _lastLine = line.Substring(nextIndex);
                        AppendLine(line.Substring(0, nextIndex));
                    }
                    else
                    {
                        AppendLine(line);
                    }

                    return true;
                }

                ResetEscapeFlag();
            }

            return false;
        }

        private void ResetState()
        {
            _jsonBuilder.Clear();
            _bracketsCount = 0;
            _startBracketIndex = -1;
            _insideQuotes = false;
            _isEscaping = false;
            _isCompleteJson = false;
            _currentCharacter = default;
        }

        private void AppendLine(string line)
        {
            switch (_jsonBuilder)
            {
                case { Length: 0 } when _startBracketIndex >= 0:
                    _jsonBuilder.Append(line.Substring(_startBracketIndex));
                    break;
                case { Length: > 0 }:
                    _jsonBuilder.Append(line);
                    break;
            }
        }

        private string GetJsonString(bool validateJson)
        {
            if (!_isCompleteJson)
            {
                throw new InvalidOperationException("Cannot get JSON string when JSON is not complete.");
            }

            var json = _jsonBuilder.ToString();
            if (validateJson)
            {
                _ = JsonNode.Parse(json);
            }

            return json;
        }

        private void MarkJsonAsComplete()
        {
            _isCompleteJson = true;
        }

        private void ResetEscapeFlag() => _isEscaping = false;

        private void HandleCurrentCharacterOutsideQuotes(int index)
        {
            if (_insideQuotes)
            {
                return;
            }

            switch (_currentCharacter)
            {
                case '{':
                    if (++_bracketsCount == 1)
                    {
                        _startBracketIndex = index;
                    }

                    break;
                case '}':
                    if (--_bracketsCount < 0)
                    {
                        throw new InvalidOperationException("Invalid JSON in stream.");
                    }

                    if (_bracketsCount == 0)
                    {
                        MarkJsonAsComplete();
                    }

                    break;
            }
        }

        private void DetermineIfQuoteStartOrEnd()
        {
            if (this is { _currentCharacter: '\"', _isEscaping: false })
            {
                _insideQuotes = !_insideQuotes;
            }
        }

        private bool IsEscapedCharacterInsideQuotes()
        {
            if (this is { _currentCharacter: '\\', _isEscaping: false, _insideQuotes: true })
            {
                _isEscaping = true;
                return true;
            }

            return false;
        }
    }
}
