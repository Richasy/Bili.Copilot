// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels.DynamicPageViewModel;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 综合动态详情模块.
/// </summary>
public sealed partial class AllDynamicDetailModule : DynamicAllModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AllDynamicDetailModule"/> class.
    /// </summary>
    public AllDynamicDetailModule()
    {
        InitializeComponent();
        ViewModel = DynamicPageViewModel.Instance;
    }

    /// <summary>
    /// 滚动到顶部.
    /// </summary>
    public void ScrollToTop()
    {
        DynamicAllScrollViewer?.ChangeView(default, 1, default);
        DynamicUserScrollViewer?.ChangeView(default, 1, default);
    }

    private void OnDynamicViewIncrementalTriggered(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);
}
