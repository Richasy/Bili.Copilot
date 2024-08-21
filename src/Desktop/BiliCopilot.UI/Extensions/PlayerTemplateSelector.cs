﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;

namespace BiliCopilot.UI.Extensions;

internal sealed class PlayerTemplateSelector : DataTemplateSelector
{
    public DataTemplate MpvTemplate { get; set; }

    public DataTemplate NativeTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is MpvPlayerViewModel)
        {
            return MpvTemplate;
        }
        else if (item is NativePlayerViewModel)
        {
            return NativeTemplate;
        }

        return default;
    }
}
