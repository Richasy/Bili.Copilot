// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 视频动态卡片控件.
/// </summary>
public sealed partial class VideoMomentPresenter : MomentCardPresenter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoMomentPresenter"/> class.
    /// </summary>
    public VideoMomentPresenter()
    {
        InitializeComponent();
        // Show/hide AddViewLaterButton on pointer enter/exit
        this.PointerEntered += OnPointerEntered;
        this.PointerExited += OnPointerExited;
        this.Unloaded += OnUnloaded;
    }

    private void OnPointerEntered(object? sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (AddViewLaterButton is not null)
        {
            AddViewLaterButton.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
    }

    private void OnPointerExited(object? sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (AddViewLaterButton is not null)
        {
            AddViewLaterButton.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }

    private void OnUnloaded(object? sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        this.PointerEntered -= OnPointerEntered;
        this.PointerExited -= OnPointerExited;
        this.Unloaded -= OnUnloaded;
    }
}
