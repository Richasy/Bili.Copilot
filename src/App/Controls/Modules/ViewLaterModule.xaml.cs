// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 稍后再看模块.
/// </summary>
public sealed partial class ViewLaterModule : ViewLaterModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLaterModule"/> class.
    /// </summary>
    public ViewLaterModule()
    {
        InitializeComponent();
        ViewModel = ViewLaterDetailViewModel.Instance;
    }

    private void OnVideoViewIncrementalTriggered(object sender, System.EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);
}

/// <summary>
/// <see cref="ViewLaterModule"/> 的基类.
/// </summary>
public abstract class ViewLaterModuleBase : ReactiveUserControl<ViewLaterDetailViewModel>
{
}
