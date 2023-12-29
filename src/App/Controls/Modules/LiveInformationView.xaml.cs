// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 直播信息视图.
/// </summary>
public sealed partial class LiveInformationView : LiveInformationViewBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveInformationView"/> class.
    /// </summary>
    public LiveInformationView() => InitializeComponent();

    private void OnOnlyAudioToggledAsync(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var control = sender as ToggleSwitch;
        var isAudioOnly = control.IsOn;
        TraceLogger.LogOnlyAudioChanged(isAudioOnly, "Live");
        if (ViewModel.PlayerDetail.IsAudioOnly != isAudioOnly)
        {
            ViewModel.PlayerDetail.ChangeAudioOnlyCommand.Execute(isAudioOnly);
        }
    }
}

/// <summary>
/// <see cref="LiveInformationView"/> 的基类.
/// </summary>
public abstract class LiveInformationViewBase : ReactiveUserControl<LivePlayerPageViewModel>
{
}
