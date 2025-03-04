// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// 设置页面.
/// </summary>
public sealed partial class SettingsPage : SettingsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    public SettingsPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        ViewModel.InitializeCommand.Execute(default);
        SectionSelector.SelectedItem = SectionSelector.Items[0];
    }

    /// <inheritdoc/>
    protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        => await ViewModel.CheckSaveServicesAsync();

    private void OnJoinGroupButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);

    private async void OnSectionSelectorChangedAsync(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        var index = Convert.ToInt32(sender.SelectedItem.Tag);
        if (index == 0)
        {
            GenericContainer.Visibility = Visibility.Visible;
            AIContainer.Visibility = Visibility.Collapsed;
        }
        else
        {
            GenericContainer.Visibility = Visibility.Collapsed;
            AIContainer.Visibility = Visibility.Visible;
            await ViewModel.InitializeChatServicesAsync();
            if (AIPanel.Children.Count == 0)
            {
                await LoadChatControlsAsync();
            }
        }
    }

    private async Task LoadChatControlsAsync()
    {
        foreach (var vm in ViewModel.ChatServices)
        {
            var control = vm.GetSettingControl();

            if (control != null)
            {
                await vm.InitializeCommand.ExecuteAsync(default);
                AIPanel.Children.Add(control);
            }
        }
    }
}

/// <summary>
/// 设置页面基类.
/// </summary>
public abstract class SettingsPageBase : LayoutPageBase<SettingsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageBase"/> class.
    /// </summary>
    protected SettingsPageBase() => ViewModel = this.Get<SettingsPageViewModel>();
}
