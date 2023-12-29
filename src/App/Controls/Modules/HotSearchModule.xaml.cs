// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Search;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 热搜模块.
/// </summary>
public sealed partial class HotSearchModule : SearchBoxModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HotSearchModule"/> class.
    /// </summary>
    public HotSearchModule()
    {
        InitializeComponent();
        ViewModel = SearchBoxViewModel.Instance;
    }

    /// <summary>
    /// 热搜点击事件.
    /// </summary>
    public event EventHandler ItemClick;

    private void OnHotSearchClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as SearchSuggest;
        TraceLogger.LogHotSearchClick();
        ViewModel.SearchBySuggestCommand.Execute(data);
        ItemClick?.Invoke(this, EventArgs.Empty);
    }
}
