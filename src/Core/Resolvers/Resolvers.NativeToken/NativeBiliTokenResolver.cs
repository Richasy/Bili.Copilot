// Copyright (c) Richasy. All rights reserved.

using System.IO;
using System.Text.Json;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models.Authorization;

namespace Richasy.BiliKernel.Resolvers.NativeToken;

/// <summary>
/// 本地令牌解析器.
/// </summary>
public sealed class NativeBiliTokenResolver : IBiliTokenResolver
{
    private const string TokenFileName = "token.json";
    private BiliToken? _cacheToken;

    /// <inheritdoc/>
    public BiliToken? GetToken()
    {
        if (_cacheToken != null)
        {
            return _cacheToken;
        }

        if (File.Exists(TokenFileName))
        {
            var json = File.ReadAllText(TokenFileName);
            _cacheToken = JsonSerializer.Deserialize<BiliToken>(json);
        }

        return _cacheToken;
    }

    /// <inheritdoc/>
    public void RemoveToken()
    {
        if (!File.Exists(TokenFileName))
        {
            return;
        }

        _cacheToken = default;
        File.Delete(TokenFileName);
    }

    /// <inheritdoc/>
    public void SaveToken(BiliToken token)
    {
        _cacheToken = token;
        File.WriteAllText(TokenFileName, JsonSerializer.Serialize(token));
    }
}
