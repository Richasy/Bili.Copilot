// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Controls.AI;

/// <summary>
/// AI 头部.
/// </summary>
public sealed partial class AIHeader : AIControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AIHeader"/> class.
    /// </summary>
    public AIHeader() => InitializeComponent();

    private void OnServiceItemClick(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement).DataContext as AIServiceItemViewModel;
        ViewModel.SelectServiceCommand.Execute(context);
        ServiceFlyout.Hide();
    }
}
