// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 剧集条目.
/// </summary>
public sealed class SeasonItem : ReactiveControl<SeasonItemViewModel>, IRepeaterItem
{
    /// <summary>
    /// <see cref="Information"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty InformationProperty =
        DependencyProperty.Register(nameof(Information), typeof(SeasonInformation), typeof(SeasonItem), new PropertyMetadata(default, new PropertyChangedCallback(OnInformationChanged)));

    /// <summary>
    /// <see cref="CoverWidth"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty CoverWidthProperty =
        DependencyProperty.Register(nameof(CoverWidth), typeof(double), typeof(SeasonItem), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="CoverHeight"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty CoverHeightProperty =
       DependencyProperty.Register(nameof(CoverHeight), typeof(double), typeof(SeasonItem), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="SeasonItem"/> class.
    /// </summary>
    public SeasonItem()
        => DefaultStyleKey = typeof(SeasonItem);

    /// <summary>
    /// 剧集信息.
    /// </summary>
    public SeasonInformation Information
    {
        get => (SeasonInformation)GetValue(InformationProperty);
        set => SetValue(InformationProperty, value);
    }

    /// <summary>
    /// 封面宽度.
    /// </summary>
    public double CoverWidth
    {
        get => (double)GetValue(CoverWidthProperty);
        set => SetValue(CoverWidthProperty, value);
    }

    /// <summary>
    /// 封面高度.
    /// </summary>
    public double CoverHeight
    {
        get => (double)GetValue(CoverHeightProperty);
        set => SetValue(CoverHeightProperty, value);
    }

    /// <inheritdoc/>
    public Size GetHolderSize()
        => new(200, 240);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        var rootCard = (FrameworkElement)GetTemplateChild("RootCard");
        if (ContextFlyout != null)
        {
            rootCard.ContextFlyout = ContextFlyout;
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

    private static void OnInformationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as SeasonItem;
        if (e.NewValue is SeasonInformation information)
        {
            var vm = new SeasonItemViewModel(information);
            instance.ViewModel = vm;
        }
    }

    private async void OnDebugItemClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new DebugDialog(ViewModel.Data);
        dialog.XamlRoot = XamlRoot;
        await dialog.ShowAsync();
    }
}
