// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;

namespace BiliCopilot.UI.Extensions;

internal sealed partial class PlayerTemplateSelector : DataTemplateSelector
{
    public DataTemplate MpvTemplate { get; set; }

    public DataTemplate NativeTemplate { get; set; }

    public DataTemplate ExternalTemplate { get; set; }

    public DataTemplate IslandTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is NativePlayerViewModel)
        {
            return NativeTemplate;
        }
        else if (item is ExternalPlayerViewModel)
        {
            return ExternalTemplate;
        }

        return default;
    }
}
