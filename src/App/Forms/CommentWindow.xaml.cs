// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using WinUIEx;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 视频动态评论窗口.
/// </summary>
public sealed partial class CommentWindow : WindowBase
{
    private readonly CommentViewModel _viewModel = new();
    private bool _isActivated;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentWindow"/> class.
    /// </summary>
    public CommentWindow(ShowCommentEventArgs commentEventArgs)
    {
        InitializeComponent();
        _viewModel.Comments.SetData(commentEventArgs.SourceId, commentEventArgs.Type);
        Title = commentEventArgs.Title;
        TitleBlock.Text = commentEventArgs.Title;
        Width = MainWindow.Instance?.Width ?? 500;
        Height = MainWindow.Instance?.Height ?? 700;
        MinWidth = 400;
        MinHeight = 400;
        IsMaximizable = false;
        Activated += OnActivated;
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (_isActivated)
        {
            return;
        }

        this.CenterOnScreen();
        _isActivated = true;
    }
}
