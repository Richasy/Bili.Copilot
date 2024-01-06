// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.Models.Data.Dynamic;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.DynamicPageViewModel;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 动态展示器.
/// </summary>
public sealed partial class DynamicPresenter : UserControl
{
    /// <summary>
    /// <see cref="Data"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(nameof(Data), typeof(object), typeof(DynamicPresenter), new PropertyMetadata(default, new PropertyChangedCallback(DataChanged)));

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicPresenter"/> class.
    /// </summary>
    public DynamicPresenter() => InitializeComponent();

    /// <summary>
    /// 数据.
    /// </summary>
    public object Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    private static void DataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as DynamicPresenter;
        var data = e.NewValue;

        if (data is VideoInformation videoInfo)
        {
            var videoVM = new VideoItemViewModel(videoInfo);
            instance.MainPresenter.Content = videoVM;
            instance.MainPresenter.ContentTemplate = DynamicPageViewModel.Instance.IsVideoShown
                ? instance.NormalVideoTemplate
                : instance.InternalVideoTemplate;
        }
        else if (data is EpisodeInformation episodeInfo)
        {
            var episodeVM = new EpisodeItemViewModel(episodeInfo);
            instance.MainPresenter.Content = episodeVM;
            instance.MainPresenter.ContentTemplate = DynamicPageViewModel.Instance.IsVideoShown
                ? instance.NormalEpisodeTemplate
                : instance.InternalEpisodeTemplate;
        }
        else if (data is DynamicInformation dynamicInfo)
        {
            var dynamicVM = new DynamicItemViewModel(dynamicInfo);
            instance.MainPresenter.Content = dynamicVM;
            instance.MainPresenter.ContentTemplate = instance.ForwardTemplate;
        }
        else if (data is List<Models.Data.Appearance.Image> images)
        {
            instance.MainPresenter.Content = images;
            instance.MainPresenter.ContentTemplate = instance.ImageTemplate;
        }
        else if (data is ArticleInformation article)
        {
            var articleVM = new ArticleItemViewModel(article);
            instance.MainPresenter.Content = articleVM;
            instance.MainPresenter.ContentTemplate = instance.ArticleTemplate;
        }
        else
        {
            instance.MainPresenter.Content = null;
            instance.MainPresenter.ContentTemplate = instance.NoneTemplate;
        }
    }
}
