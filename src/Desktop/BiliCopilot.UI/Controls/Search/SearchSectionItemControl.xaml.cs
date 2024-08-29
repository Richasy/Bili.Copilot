// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Controls.Search;

/// <summary>
/// 搜索区块控件.
/// </summary>
public sealed partial class SearchSectionItemControl : UserControl
{
    /// <summary>
    /// <see cref="ViewModel"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(ISearchSectionDetailViewModel), typeof(SearchSectionItemControl), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchSectionItemControl"/> class.
    /// </summary>
    public SearchSectionItemControl() => InitializeComponent();

    /// <summary>
    /// 视图模型.
    /// </summary>
    public ISearchSectionDetailViewModel ViewModel
    {
        get => (ISearchSectionDetailViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    internal static int ToCount(int? count)
        => count ?? 0;
}
