// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// 应用导航事件参数.
/// </summary>
public sealed class AppNavigationEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppNavigationEventArgs"/> class.
    /// </summary>
    /// <param name="pageId">导航页面 Id.</param>
    /// <param name="parameter">导航参数.</param>
    public AppNavigationEventArgs(
        PageType pageId,
        object parameter)
    {
        PageId = pageId;
        Parameter = parameter;
    }

    /// <summary>
    /// 页面 Id.
    /// </summary>
    public PageType PageId { get; }

    /// <summary>
    /// 导航参数.
    /// </summary>
    public object Parameter { get; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is AppNavigationEventArgs args && PageId == args.PageId && EqualityComparer<object>.Default.Equals(Parameter, args.Parameter);

    /// <inheritdoc/>
    public override int GetHashCode()
        => PageId.GetHashCode() + Parameter?.GetHashCode() ?? 0;
}

