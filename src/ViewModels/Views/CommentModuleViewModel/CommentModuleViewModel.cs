// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Models.Constants.Bili;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 评论页面/模块视图模型.
/// </summary>
public sealed partial class CommentModuleViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentModuleViewModel"/> class.
    /// </summary>
    public CommentModuleViewModel()
    {
        MainViewModel = new CommentMainModuleViewModel();
        DetailViewModel = new CommentDetailModuleViewModel();
        IsMainShown = true;
        IsDetailShown = false;

        MainViewModel.RequestShowDetail += OnRequestShowDetail;
        DetailViewModel.RequestBackToMain += OnRequestBackToMain;
    }

    /// <summary>
    /// 设置数据.
    /// </summary>
    public void SetData(string sourceId, CommentType type, CommentSortType sortType = CommentSortType.Hot)
    {
        ShowMainView();
        MainViewModel.SetTarget(sourceId, type, sortType);
    }

    /// <summary>
    /// 清除数据.
    /// </summary>
    public void ClearData()
    {
        MainViewModel.ClearData();
        DetailViewModel.ClearData();
    }

    private void OnRequestShowDetail(object sender, CommentItemViewModel e)
    {
        IsMainShown = false;
        IsDetailShown = true;
        DetailViewModel.SetRoot(e);
    }

    private void OnRequestBackToMain(object sender, EventArgs e) => ShowMainView();

    private void ShowMainView()
    {
        IsMainShown = true;
        IsDetailShown = false;
        DetailViewModel.ClearData();
    }
}
