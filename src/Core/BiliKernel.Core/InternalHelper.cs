// Copyright (c) Richasy. All rights reserved.

using System.Security.Cryptography;
using System.Text;

namespace Richasy.BiliKernel;

internal static class InternalHelper
{
    public static string Md5ComputeHash(this string input)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        var sb = new StringBuilder();
        foreach (var b in hashBytes)
        {
            _ = sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}
