// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 用来显示视频条目的 UI 单元.
/// </summary>
public sealed partial class VideoItem : ReactiveControl<VideoItemViewModel>, IRepeaterItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoItem"/> class.
    /// </summary>
    public VideoItem() => DefaultStyleKey = typeof(VideoItem);

    /// <inheritdoc/>
    public Size GetHolderSize() => new(400, 180);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        var rootCard = (FrameworkElement)GetTemplateChild("RootCard");
        if (ContextFlyout != null)
        {
            rootCard.ContextFlyout = ContextFlyout;
        }
        else
        {
            var privatePlayItem = GetTemplateChild("PrivatePlayItem") as MenuFlyoutItem;
            if (privatePlayItem != null)
            {
                privatePlayItem.Click += OnPrivatePlayItemClick;
            }
        }

        var debugItem = new MenuFlyoutItem();
        debugItem.Text = ResourceToolkit.GetLocalizedString(StringNames.DebugInformation);
        debugItem.Icon = new FluentIcon() { Symbol = FluentSymbol.Bug };
        debugItem.Click += OnDebugItemClickAsync;
        if (rootCard != null && rootCard.ContextFlyout is MenuFlyout flyout && flyout.Items != null)
        {
            flyout.Items.Add(debugItem);
        }
    }

    private async void OnDebugItemClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new DebugDialog(ViewModel.Data);
        dialog.XamlRoot = XamlRoot;
        await dialog.ShowAsync();
    }

    private void OnPrivatePlayItemClick(object sender, RoutedEventArgs e)
        => TraceLogger.LogPlayInPrivate();
}
