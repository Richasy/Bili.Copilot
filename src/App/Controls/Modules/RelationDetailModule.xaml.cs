// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 关系详情模块.
/// </summary>
public sealed partial class RelationDetailModule : RelationDetailModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RelationDetailModule"/> class.
    /// </summary>
    public RelationDetailModule() => InitializeComponent();

    private void OnIncrementalTriggered(object sender, System.EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);
}

/// <summary>
/// <see cref="RelationDetailModule"/> 的基类.
/// </summary>
public abstract class RelationDetailModuleBase : ReactiveUserControl<RelationDetailViewModel>
{
}
