// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls;

/// <summary>
/// AI 总结对话框.
/// </summary>
public sealed partial class AIFeatureDialog : ContentDialog
{
    private readonly AIFeatureType _type;
    private readonly object _data;
    private readonly AIFeatureViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="AIFeatureDialog"/> class.
    /// </summary>
    public AIFeatureDialog(AIFeatureType type)
    {
        InitializeComponent();
        _type = type;
        _viewModel = new AIFeatureViewModel();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AIFeatureDialog"/> class.
    /// </summary>
    public AIFeatureDialog(object video, AIFeatureType type)
        : this(type)
        => _data = video;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_type == AIFeatureType.VideoSummarize)
        {
            _viewModel.SummarizeVideoCommand.Execute((VideoIdentifier)_data);
        }
        else if (_type == AIFeatureType.VideoEvaluation)
        {
            _viewModel.EvaluateVideoCommand.Execute((VideoIdentifier)_data);
        }
        else if(_type == AIFeatureType.ArticleSummarize)
        {
            _viewModel.SummarizeArticleCommand.Execute((ArticleIdentifier)_data);
        }
    }

    private void OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        => _viewModel.CancelCommand.Execute(default);
}
