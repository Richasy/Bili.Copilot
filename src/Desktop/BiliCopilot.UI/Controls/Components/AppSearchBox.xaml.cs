// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
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

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    private void OnKeywordChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            ViewModel.ReloadSuggestionsCommand.Execute(default);
        }
    }

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
            var vm = new SearchSuggestItemViewModel(new SearchSuggestItem(context.Keyword, context.Keyword));
            ViewModel.SearchCommand.Execute(vm);
        }

        HotSearchFlyout.Hide();
    }

    private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is SearchSuggestItemViewModel item)
        {
            ViewModel.SearchCommand.Execute(item);
        }
        else if (!string.IsNullOrEmpty(args.QueryText))
        {
            var vm = new SearchSuggestItemViewModel(new SearchSuggestItem(args.QueryText, args.QueryText));
            ViewModel.SearchCommand.Execute(vm);
        }

        ViewModel.TryCancelSuggestCommand.Execute(default);
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, ViewModel.Suggestion.Clear);
    }
}

/// <summary>
/// 应用搜索框的基类.
/// </summary>
public abstract class AppSearchBoxBase : LayoutUserControlBase<SearchBoxViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppSearchBoxBase"/> class.
    /// </summary>
    protected AppSearchBoxBase() => ViewModel = this.Get<SearchBoxViewModel>();
}
