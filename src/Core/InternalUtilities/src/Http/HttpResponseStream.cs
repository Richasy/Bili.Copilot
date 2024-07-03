// Copyright (c) Richasy. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;

namespace Richasy.BiliKernel.Http;

/// <summary>
/// Associate a response stream with its parent response for parity in life-cycle management.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is an internal utility.")]
[ExcludeFromCodeCoverage]
internal sealed class HttpResponseStream(Stream stream, HttpResponseMessage response) : Stream
{
    private readonly Stream _stream = stream;
    private readonly HttpResponseMessage _response = response;

    public override bool CanRead => _stream.CanRead;

    public override bool CanSeek => _stream.CanSeek;

    public override bool CanWrite => _stream.CanWrite;

    public override long Length => _stream.Length;

    public override long Position { get => _stream.Position; set => _stream.Position = value; }

    public override void Flush()
    {
        _stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _stream.Dispose();
            _response.Dispose();
        }
    }
}
