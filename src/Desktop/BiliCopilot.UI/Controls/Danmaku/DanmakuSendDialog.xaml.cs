// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;

namespace BiliCopilot.UI.Controls.Danmaku;

public sealed partial class DanmakuSendDialog : ContentDialog
{
    public DanmakuSendViewModel ViewModel
    {
        get { return (DanmakuSendViewModel)GetValue(ViewModelProperty); }
        set { SetValue(ViewModelProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(DanmakuSendViewModel), typeof(DanmakuSendDialog), new PropertyMetadata(default));

    private readonly Action _loadAction;

    public DanmakuSendDialog(DanmakuSendViewModel vm, Action loadAction)
    {
        InitializeComponent();
        ViewModel = vm;
        _loadAction = loadAction;
    }

    public string GetInputText() => InputBox.Text.Trim();

    private void OnSizeItemClick(object sender, RoutedEventArgs e)
        => ViewModel.IsStandardSize = (sender as FrameworkElement).Tag.ToString() == "Standard";

    private void OnColorItemTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        => ViewModel.Color = (Windows.UI.Color)(sender as FrameworkElement).DataContext;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loadAction();
    }
}
