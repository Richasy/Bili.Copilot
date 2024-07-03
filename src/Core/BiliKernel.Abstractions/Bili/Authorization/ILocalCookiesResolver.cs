﻿// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;

namespace Richasy.BiliKernel.Bili.Authorization;

/// <summary>
/// 本地 Cookie 解析器.
/// </summary>
public interface ILocalBiliCookiesResolver
{
    /// <summary>
    /// 获取 Cookie 字符串.
    /// </summary>
    /// <returns></returns>
    string GetCookieString();

    /// <summary>
    /// 获取 Cookie 列表.
    /// </summary>
    /// <returns>Cookie 列表.</returns>
    Dictionary<string, string> GetCookies();

    /// <summary>
    /// 保存 Cookie.
    /// </summary>
    /// <param name="cookies">Cookie 列表.</param>
    void SaveCookies(Dictionary<string, string> cookies);
}
