// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;
using Windows.Foundation;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 消息条目.
/// </summary>
public sealed class MessageItem : ReactiveControl<MessageItemViewModel>, IRepeaterItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageItem"/> class.
    /// </summary>
    public MessageItem() => DefaultStyleKey = typeof(MessageItem);

    /// <inheritdoc/>
    public Size GetHolderSize() => new Size(double.PositiveInfinity, 120);
}
