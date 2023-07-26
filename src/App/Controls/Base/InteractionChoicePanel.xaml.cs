// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 互动视频选项面板.
/// </summary>
public sealed partial class InteractionChoicePanel : InteractionChoicePanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionChoicePanel"/> class.
    /// </summary>
    public InteractionChoicePanel() => InitializeComponent();

    private void OnChoiceClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as InteractionInformation;
        ViewModel.SelectInteractionChoiceCommand.Execute(data);
    }
}

/// <summary>
/// <see cref="InteractionChoicePanel"/> 的基类.
/// </summary>
public abstract class InteractionChoicePanelBase : ReactiveUserControl<PlayerDetailViewModel>
{
}
