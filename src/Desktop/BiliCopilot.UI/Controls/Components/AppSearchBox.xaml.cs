// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.BiliKernel.Models.Search;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 应用搜索框.
/// </summary>
public sealed partial class AppSearchBox : AppSearchBoxBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppSearchBox"/> class.
    /// </summary>
    public AppSearchBox() => InitializeComponent();

    private void OnKeywordChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        => ViewModel.ReloadSuggestionsCommand.Execute(default);

    private void OnBoxGettingFocus(UIElement sender, Microsoft.UI.Xaml.Input.GettingFocusEventArgs args)
    {
        if (!Box.IsSuggestionListOpen)
        {
            ViewModel.Keyword = " ";
            ViewModel.Keyword = string.Empty;
        }
    }

    private void OnHotSearchFlyoutOpened(object sender, object e)
        => ViewModel.LoadHotSearchCommand.Execute(default);

    private void OnHotSearchItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        var context = args.InvokedItem as HotSearchItem;
        if (context is not null)
        {
            ViewModel.Keyword = context.Keyword;
        }

        HotSearchFlyout.Hide();
    }
}

/// <summary>
/// 应用搜索框的基类.
/// </summary>
public abstract class AppSearchBoxBase : LayoutUserControlBase<SearchViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppSearchBoxBase"/> class.
    /// </summary>
    protected AppSearchBoxBase() => ViewModel = this.Get<SearchViewModel>();
}
