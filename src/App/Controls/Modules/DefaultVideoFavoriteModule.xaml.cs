// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 默认视频收藏夹模块.
/// </summary>
public sealed partial class DefaultVideoFavoriteModule : DefaultVideoFavoriteModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultVideoFavoriteModule"/> class.
    /// </summary>
    public DefaultVideoFavoriteModule()
    {
        InitializeComponent();
        ViewModel = DefaultVideoFavoriteDetailViewModel.Instance;
    }

    private void OnVideoViewIncrementalTriggered(object sender, System.EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);
}

/// <summary>
/// <see cref="DefaultVideoFavoriteModule"/> 的基类.
/// </summary>
public abstract class DefaultVideoFavoriteModuleBase : ReactiveUserControl<DefaultVideoFavoriteDetailViewModel>
{
}
