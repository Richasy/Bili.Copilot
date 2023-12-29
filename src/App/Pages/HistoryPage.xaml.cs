// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 历史记录页面.
/// </summary>
public sealed partial class HistoryPage : HistoryPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryPage"/> class.
    /// </summary>
    public HistoryPage()
    {
        InitializeComponent();
        ViewModel = HistoryDetailViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// <see cref="HistoryPage"/> 的基类.
/// </summary>
public abstract class HistoryPageBase : PageBase<HistoryDetailViewModel>
{
}
