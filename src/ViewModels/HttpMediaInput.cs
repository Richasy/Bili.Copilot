// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using Windows.Storage.Streams;

namespace Bili.Copilot.ViewModels;

internal sealed class HttpMediaInput : MediaInput
{
    private readonly Uri _requestedUri;
    private HttpClient _client;
    private IInputStream _inputStream;
    private bool _isDisposing;
    private CancellationTokenSource _requestCancellation;

    // No public constructor, factory methods instead to handle async tasks.
    private HttpMediaInput(HttpClient client, Uri uri)
    {
        _client = client;
        _requestedUri = uri;
        Position = 0;
        _requestCancellation = new CancellationTokenSource();
    }

    public ulong Position { get; private set; }

    public string ContentType { get; private set; } = string.Empty;

    public static Task<HttpMediaInput> CreateAsync(HttpClient client, Uri uri)
    {
        var randomStream = new HttpMediaInput(client, uri);

        return AsyncInfo.Run(async (cancellationToken) =>
        {
            await randomStream.SendRequestAsync(CancellationToken.None);
            return randomStream;
        }).AsTask();
    }

    public override bool Open(out ulong size)
    {
        try
        {
            Position = 0;
            var stream = _inputStream.AsStreamForRead();
            try
            {
                size = (ulong)stream.Length;
            }
            catch (Exception)
            {
                size = ulong.MaxValue;
            }

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            return true;
        }
        catch (Exception)
        {
            size = 0uL;
            return false;
        }
    }

    public unsafe override int Read(IntPtr buf, uint len)
    {
        try
        {
            if (_isDisposing)
            {
                return default;
            }

            try
            {
                if (_inputStream == null)
                {
                    SendRequestAsync(_requestCancellation.Token).Wait(_requestCancellation.Token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            var stream = _inputStream.AsStreamForRead();
            if (stream.CanSeek && stream.Position == stream.Length)
            {
                return 0;
            }

            Position += len;

            return stream.Read(new Span<byte>(buf.ToPointer(), (int)Math.Min(len, 2147483647u)));
        }
        catch (Exception)
        {
            return -1;
        }
    }

    public override bool Seek(ulong offset)
    {
        try
        {
            if (Position != offset)
            {
                _inputStream?.Dispose();
                _inputStream = null;
                _requestCancellation?.Cancel();
                _requestCancellation?.Dispose();
                _requestCancellation = null;

                _requestCancellation = new CancellationTokenSource();
                Position = offset;
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override void Close()
    {
        try
        {
            if (_isDisposing)
            {
                return;
            }

            _isDisposing = true;
            _requestCancellation?.Cancel();
            _requestCancellation?.Dispose();
            _requestCancellation = null;
            if (_inputStream != null)
            {
                _inputStream.Dispose();
                _inputStream = null;
            }

            if (_client != null)
            {
                _client?.Dispose();
                _client = null;
            }
        }
        catch (Exception)
        {
        }
    }

    private async Task SendRequestAsync(CancellationToken cancellationToken)
    {
        if (_isDisposing)
        {
            return;
        }

        var request = new HttpRequestMessage(HttpMethod.Get, _requestedUri);

        request.Headers.Add("Range", string.Format("bytes={0}-", Position));
        request.Headers.Add("Connection", "Keep-Alive");

        if (_client == null)
        {
            return;
        }

        var response = await _client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken).ConfigureAwait(false);

        _inputStream = (await response.Content.ReadAsStreamAsync().ConfigureAwait(false)).AsInputStream();
    }
}
