// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// PGC 推荐模块.
/// </summary>
public sealed partial class PgcRecommendModule : PgcRecommendModuleBase
{
    /// <summary>
    /// <see cref="PgcType"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty PgcTypeProperty =
        DependencyProperty.Register(nameof(PgcType), typeof(PgcType), typeof(PgcRecommendModule), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="PgcRecommendModule"/> class.
    /// </summary>
    public PgcRecommendModule()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// PGC 类型.
    /// </summary>
    public PgcType PgcType
    {
        get => (PgcType)GetValue(PgcTypeProperty);
        set => SetValue(PgcTypeProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel = PgcType switch
        {
            PgcType.Bangumi => BangumiRecommendDetailViewModel.Instance,
            PgcType.Domestic => DomesticRecommendDetailViewModel.Instance,
            PgcType.Movie => MovieRecommendDetailViewModel.Instance,
            PgcType.TV => TvRecommendDetailViewModel.Instance,
            PgcType.Documentary => DocumentaryRecommendDetailViewModel.Instance,
            _ => throw new ArgumentOutOfRangeException(nameof(PgcType))
        };

        ViewModel.RequestScrollToTop += OnRequestScrollToTopAsync;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel == null)
        {
            return;
        }

        ViewModel.RequestScrollToTop -= OnRequestScrollToTopAsync;
    }

    private async void OnRequestScrollToTopAsync(object sender, EventArgs e)
    {
        await Task.Delay(200);
        ContentScrollViewer.ChangeView(0, 0, default);
    }

    private void OnSeasonViewIncrementalTriggered(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);
}

/// <summary>
/// <see cref="PgcRecommendModule"/> 的基类.
/// </summary>
public abstract class PgcRecommendModuleBase : ReactiveUserControl<PgcRecommendDetailViewModel>
{
}
