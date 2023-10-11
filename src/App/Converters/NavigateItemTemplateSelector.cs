// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels.Items;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Converters;

internal sealed class NavigateItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate Item { get; set; }

    public DataTemplate Header { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        var navItem = item as NavigateItemViewModel;
        return navItem.IsHeader ? Header : Item;
    }
}
