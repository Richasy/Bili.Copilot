// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.App.Extensions;

/// <summary>
/// 分区树视图项模板选择器.
/// </summary>
public sealed class PartitionTreeViewItemTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// 父级分区模板.
    /// </summary>
    public DataTemplate ParentTemplate { get; set; }

    /// <summary>
    /// 子级分区模板.
    /// </summary>
    public DataTemplate ChildTemplate { get; set; }

    /// <inheritdoc/>
    protected override DataTemplate SelectTemplateCore(object item)
    {
        return item is PartitionItemViewModel vm
            ? vm.Children == null ? ChildTemplate : ParentTemplate
            : throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        => SelectTemplateCore(item);
}
