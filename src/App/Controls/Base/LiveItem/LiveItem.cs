// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 直播间视频卡片.
/// </summary>
public sealed class LiveItem : ReactiveControl<LiveItemViewModel>, IRepeaterItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveItem"/> class.
    /// </summary>
    public LiveItem()
        => DefaultStyleKey = typeof(LiveItem);

    /// <inheritdoc/>
    public Size GetHolderSize() => new(210, 248);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        var rootCard = (FrameworkElement)GetTemplateChild("RootCard");
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
}
