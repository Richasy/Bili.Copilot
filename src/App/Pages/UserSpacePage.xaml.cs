// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 用户空间页面.
/// </summary>
public sealed partial class UserSpacePage : UserSpacePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserSpacePage"/> class.
    /// </summary>
    public UserSpacePage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is UserSpaceViewModel vm)
        {
            ViewModel = vm;
        }
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        ViewModel.InitializeCommand.Execute(default);
        SearchBox.Focus(FocusState.Programmatic);
    }

    private void OnIncrementalTriggered(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);

    private void OnSearchBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (!ViewModel.CanSearch)
        {
            return;
        }

        ViewModel.SearchCommand.Execute(default);
    }

    private void OnBackButtonClick(object sender, EventArgs e)
    {
        ViewModel.IsInFans = false;
        ViewModel.IsInFollows = false;
    }
}

/// <summary>
/// <see cref="UserSpacePage"/> 的基类.
/// </summary>
public abstract class UserSpacePageBase : PageBase<UserSpaceViewModel>
{
}
