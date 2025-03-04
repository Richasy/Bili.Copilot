// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;

namespace BiliCopilot.UI.Extensions;

internal sealed partial class MessageSectionTemplateSelector : DataTemplateSelector
{
    public DataTemplate NotifyTemplate { get; set; }

    public DataTemplate ChatTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is NotifyMessageSectionDetailViewModel)
        {
            return NotifyTemplate;
        }
        else if (item is ChatMessageSectionDetailViewModel)
        {
            return ChatTemplate;
        }

        return default;
    }
}
