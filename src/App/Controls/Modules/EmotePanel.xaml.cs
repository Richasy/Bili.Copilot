// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels.Components;
using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 表情模块.
/// </summary>
public sealed partial class EmotePanel : EmotePanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmotePanel"/> class.
    /// </summary>
    public EmotePanel()
    {
        InitializeComponent();
        ViewModel = EmoteModuleViewModel.Instance;
        Loaded += OnLoaded;
    }

    /// <summary>
    /// 点击表情.
    /// </summary>
    public event EventHandler<string> ItemClick;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.InitializeCommand.Execute(default);
    }

    private void OnItemClick(object sender, RoutedEventArgs e)
    {
        var key = (sender as FrameworkElement).Tag as string;
        ItemClick?.Invoke(this, key);
    }

    private void OnPackageClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as EmotePackageViewModel;
        if (data != ViewModel.Current)
        {
            ViewModel.SelectPackageCommand.Execute(data);
        }
    }
}

/// <summary>
/// <see cref="EmotePanel"/> 的基类.
/// </summary>
public abstract class EmotePanelBase : ReactiveUserControl<EmoteModuleViewModel>
{
}
