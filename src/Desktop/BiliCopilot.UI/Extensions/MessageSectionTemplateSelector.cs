// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;

namespace BiliCopilot.UI.Extensions;

internal sealed class MessageSectionTemplateSelector : DataTemplateSelector
{
    public DataTemplate NotifyTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is NotifyMessageSectionDetailViewModel)
        {
            return NotifyTemplate;
        }

        return default;
    }
}
