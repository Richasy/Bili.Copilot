// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls.Settings;

/// <summary>
/// 漫游设置区块.
/// </summary>
public sealed partial class RoamingSettingSection : SettingSection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoamingSettingSection"/> class.
    /// </summary>
    public RoamingSettingSection() => InitializeComponent();

    private static void ShowTip(StringNames text, bool isSuccess)
    {
        var type = isSuccess
            ? InfoType.Success
            : InfoType.Error;
        var tip = ResourceToolkit.GetLocalizedString(text);
        AppViewModel.Instance.ShowTip(tip, type);
    }

    private void OnVideoAddressBoxSubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var text = sender.Text;
        if (string.IsNullOrEmpty(text?.Trim()))
        {
            ViewModel.RoamingVideoAddress = string.Empty;
        }
        else
        {
            if (Uri.IsWellFormedUriString(text.Trim(), UriKind.Absolute))
            {
                ViewModel.RoamingVideoAddress = text.TrimEnd('/');
                ShowTip(StringNames.SetAddressSuccess, true);
            }
            else
            {
                ShowTip(StringNames.InvalidAddress, false);
                sender.Text = ViewModel.RoamingVideoAddress;
            }
        }
    }

    private void OnViewAddressBoxSubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var text = sender.Text;
        if (string.IsNullOrEmpty(text?.Trim()))
        {
            ViewModel.RoamingViewAddress = string.Empty;
        }
        else
        {
            if (Uri.IsWellFormedUriString(text.Trim(), UriKind.Absolute))
            {
                ViewModel.RoamingViewAddress = text.TrimEnd('/');
                ShowTip(StringNames.SetAddressSuccess, true);
            }
            else
            {
                ShowTip(StringNames.InvalidAddress, false);
                sender.Text = ViewModel.RoamingViewAddress;
            }
        }
    }

    private void OnSearchAddressBoxSubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var text = sender.Text;
        if (string.IsNullOrEmpty(text?.Trim()))
        {
            ViewModel.RoamingSearchAddress = string.Empty;
        }
        else
        {
            if (Uri.IsWellFormedUriString(text.Trim(), UriKind.Absolute))
            {
                ViewModel.RoamingSearchAddress = text.TrimEnd('/');
                ShowTip(StringNames.SetAddressSuccess, true);
            }
            else
            {
                ShowTip(StringNames.InvalidAddress, false);
                sender.Text = ViewModel.RoamingSearchAddress;
            }
        }
    }
}
