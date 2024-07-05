// Copyright (c) Richasy. All rights reserved.

using System.Text;

namespace Richasy.BiliKernel.Http;

/// <summary>
/// BUVID.
/// </summary>
public sealed class Buvid
{
    private readonly string _mac;

    /// <summary>
    /// Initializes a new instance of the <see cref="Buvid"/> class.
    /// </summary>
    /// <param name="macAddress">MAC 地址.</param>
    public Buvid(string macAddress) => _mac = macAddress;

    /// <summary>
    /// 生成 buvid.
    /// </summary>
    /// <returns>buvid.</returns>
    public string Generate()
    {
        const string buvidPrefix = "XY";
        var inputStrMd5 = _mac.Replace(":", string.Empty).Md5ComputeHash();

        var buvidRaw = new StringBuilder();
        _ = buvidRaw.Append(buvidPrefix);
        _ = buvidRaw.Append(inputStrMd5[2]);
        _ = buvidRaw.Append(inputStrMd5[12]);
        _ = buvidRaw.Append(inputStrMd5[22]);
        _ = buvidRaw.Append(inputStrMd5);

        return buvidRaw.ToString();
    }
}
