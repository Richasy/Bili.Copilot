// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// MPV 设置视图.
/// </summary>
public sealed partial class MpvSettingsView : MpvSettingsViewBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MpvSettingsView"/> class.
    /// </summary>
    public MpvSettingsView()
        => InitializeComponent();

    private void OnFormatSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0)
        {
            return;
        }

        if (e.AddedItems[0] is FormatInformation format && ViewModel.CurrentFormat != format)
        {
            ViewModel.ChangeFormatCommand.Execute(format);
        }
    }

    private void OnReplayButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.ChangeFormatCommand.Execute(ViewModel.CurrentFormat);
}

/// <summary>
/// <see cref="MpvSettingsView"/> 的基类.
/// </summary>
public abstract class MpvSettingsViewBase : ReactiveUserControl<PlayerDetailViewModel>
{
}
