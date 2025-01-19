// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;
using System.Windows.Input;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// UP 主区域.
/// </summary>
public sealed partial class UpSectionControl : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="UserName"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty UserNameProperty =
        DependencyProperty.Register(nameof(UserName), typeof(string), typeof(UpSectionControl), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Avatar"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty AvatarProperty =
        DependencyProperty.Register(nameof(Avatar), typeof(Uri), typeof(UpSectionControl), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="IsFollow"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsFollowProperty =
        DependencyProperty.Register(nameof(IsFollow), typeof(bool), typeof(UpSectionControl), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="ActiveCommand"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ActiveCommandProperty =
        DependencyProperty.Register(nameof(ActiveCommand), typeof(ICommand), typeof(UpSectionControl), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="ToggleFollowCommand"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ToggleFollowCommandProperty =
        DependencyProperty.Register(nameof(ToggleFollowCommand), typeof(ICommand), typeof(UpSectionControl), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Subtitle"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(UpSectionControl), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="IsFollowButtonShown"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsFollowButtonShownProperty =
        DependencyProperty.Register(nameof(IsFollowButtonShown), typeof(bool), typeof(UpSectionControl), new PropertyMetadata(true));

    /// <summary>
    /// Initializes a new instance of the <see cref="UpSectionControl"/> class.
    /// </summary>
    public UpSectionControl() => InitializeComponent();

    /// <summary>
    /// 用户名.
    /// </summary>
    public string UserName
    {
        get => (string)GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }

    /// <summary>
    /// 头像.
    /// </summary>
    public Uri Avatar
    {
        get => (Uri)GetValue(AvatarProperty);
        set => SetValue(AvatarProperty, value);
    }

    /// <summary>
    /// 是否正在关注.
    /// </summary>
    public bool IsFollow
    {
        get => (bool)GetValue(IsFollowProperty);
        set => SetValue(IsFollowProperty, value);
    }

    /// <summary>
    /// 副标题.
    /// </summary>
    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    /// <summary>
    /// 是否显示关注按钮.
    /// </summary>
    public bool IsFollowButtonShown
    {
        get => (bool)GetValue(IsFollowButtonShownProperty);
        set => SetValue(IsFollowButtonShownProperty, value);
    }

    /// <summary>
    /// 切换关注命令.
    /// </summary>
    public ICommand ToggleFollowCommand
    {
        get => (ICommand)GetValue(ToggleFollowCommandProperty);
        set => SetValue(ToggleFollowCommandProperty, value);
    }

    /// <summary>
    /// 激活命令.
    /// </summary>
    public ICommand ActiveCommand
    {
        get => (ICommand)GetValue(ActiveCommandProperty);
        set => SetValue(ActiveCommandProperty, value);
    }

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);
}
