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

    /// <inheritdoc/>
    public BiliToken? GetToken()
    {
        if (File.Exists(TokenFileName))
        {
            var json = File.ReadAllText(TokenFileName);
            return JsonSerializer.Deserialize<BiliToken>(json);
        }

        return null;
    }

    /// <inheritdoc/>
    public void RemoveToken()
    {
        if (!File.Exists(TokenFileName))
        {
            return;
        }

        File.Delete(TokenFileName);
    }

    /// <inheritdoc/>
    public void SaveToken(BiliToken token)
        => File.WriteAllText(TokenFileName, JsonSerializer.Serialize(token));
}
