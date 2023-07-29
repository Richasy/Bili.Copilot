// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 用户条目.
/// </summary>
public sealed class UserItem : ReactiveControl<UserItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserItem"/> class.
    /// </summary>
    public UserItem() => DefaultStyleKey = typeof(UserItem);
}
