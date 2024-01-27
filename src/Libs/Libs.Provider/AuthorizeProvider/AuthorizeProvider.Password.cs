// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.BiliBili.Others;
using Windows.Security.Cryptography.Core;
using static Bili.Copilot.Models.App.Constants.ApiConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 授权提供程序.
/// </summary>
public sealed partial class AuthorizeProvider
{
    /// <summary>
    /// 加密密码.
    /// </summary>
    /// <param name="password">密码.</param>
    /// <returns>加密后的密码.</returns>
    public static async Task<string> EncryptedPasswordAsync(string password)
    {
        var base64Str = string.Empty;
        try
        {
            var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Passport.PasswordEncrypt);
            var response = await HttpProvider.Instance.SendAsync(request);
            var data = await HttpProvider.ParseAsync<ServerResponse<PasswordEncryptedResponse>>(response);
            var pwdStr = data.Data.Hash + password;
            var text = Regex.Match(data.Data.Key, "BEGIN PUBLIC KEY-----(?<key>[\\s\\S]+)-----END PUBLIC KEY").Groups["key"].Value.Trim();
            var dataArray = Convert.FromBase64String(text);
            var p = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
            var cryptKey = p.ImportPublicKey(WindowsRuntimeBufferExtensions.AsBuffer(dataArray), 0);
            var buffer = CryptographicEngine.Encrypt(cryptKey, WindowsRuntimeBufferExtensions.AsBuffer(Encoding.UTF8.GetBytes(pwdStr)), default);
            base64Str = Convert.ToBase64String(buffer.ToArray());
        }
        catch (System.Exception)
        {
            base64Str = password;
        }

        return base64Str;
    }
}
