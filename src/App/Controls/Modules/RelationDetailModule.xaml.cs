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
    /// <see cref="IsBackButtonShown"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsBackButtonShownProperty =
        DependencyProperty.Register(nameof(IsBackButtonShown), typeof(bool), typeof(RelationDetailModule), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="RelationDetailModule"/> class.
    /// </summary>
    public RelationDetailModule() => InitializeComponent();

    /// <summary>
    /// 返回按钮点击.
    /// </summary>
    public event EventHandler BackButtonClick;

    /// <summary>
    /// 获取或设置是否显示返回按钮.
    /// </summary>
    public bool IsBackButtonShown
    {
        get => (bool)GetValue(IsBackButtonShownProperty);
        set => SetValue(IsBackButtonShownProperty, value);
    }

    private void OnIncrementalTriggered(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);

    private void OnBackButtonClick(object sender, RoutedEventArgs e)
        => BackButtonClick?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// <see cref="RelationDetailModule"/> 的基类.
/// </summary>
public abstract class RelationDetailModuleBase : ReactiveUserControl<RelationDetailViewModel>
{
}
