// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.Toolkits;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 视频卡片控件.
/// 这个控件用于在列表/网格中显示视频项卡片，并处理与卡片交互相关的逻辑（播放、用户空间、右键菜单、稍后再看等）。
/// </summary>
public sealed partial class VideoCardControl : LayoutControlBase<VideoItemViewModel>
{
    // 根卡片元素（ControlTemplate 内的命名元素），用于监听指针进入/移出以显示操作按钮
    private UIElement _rootCard;
    // 用户空间按钮（通常在卡片上显示作者/UP 主头像或按钮）
    private Button _userButton;
    // “稍后再看”按钮（显示在封面上，悬停时可见）
    private Button _addViewLaterButton;

    /// <summary>
    /// 构造函数：设置默认样式键。
    /// </summary>
    public VideoCardControl() => DefaultStyleKey = typeof(VideoCardControl);

    /// <inheritdoc/>
    /// <remarks>
    /// 在模板应用时查找命名的子元素并绑定命令与事件。
    /// 每次模板重新应用前先取消上一次绑定，避免重复订阅。
    /// </remarks>
    protected override void OnApplyTemplate()
    {
        // 取消之前订阅的事件，防止重复绑定
        if (_rootCard is not null)
        {
            _rootCard.PointerEntered -= OnRootPointerEntered;
            _rootCard.PointerExited -= OnRootPointerExited;
            if (_rootCard is Grid grid)
            {
                // 如果根元素是 Grid（例如 Moment 风格），也移除 Tapped 处理器
                grid.Tapped -= OnRootGridTapped;
            }
        }

        // 从 ControlTemplate 获取命名元素引用（允许为 null，需要做空检查）
        _rootCard = GetTemplateChild("RootCard") as UIElement;
        _userButton = GetTemplateChild("UserButton") as Button;
        _addViewLaterButton = GetTemplateChild("AddViewLaterButton") as Button;

        // 如果当前绑定了 ViewModel，则完成命令与事件的绑定
        if (ViewModel is not null)
        {
            if (_rootCard is not null)
            {
                // 当指针进入/离开根卡片时，切换“稍后再看”按钮的可见性
                _rootCard.PointerEntered += OnRootPointerEntered;
                _rootCard.PointerExited += OnRootPointerExited;

                // 根据不同模板类型决定如何处理点击或点击命令
                if (_rootCard is ButtonBase buttonBase)
                {
                    // 若根元素是 ButtonBase（大部分卡片模板），则将播放命令绑定给它
                    buttonBase.Command = ViewModel.PlayCommand;
                }
                else if (_rootCard is Grid momentGrid)
                {
                    // 对于 Moment 风格，根元素可能是 Grid，需要监听 Tapped 事件以触发播放
                    momentGrid.Tapped += OnRootGridTapped;
                }
            }

            // 将用户按钮绑定到 ViewModel 的显示用户空间命令
            if (_userButton is not null)
            {
                _userButton.Command = ViewModel.ShowUserSpaceCommand;
            }

            // 将稍后再看按钮绑定到 ViewModel 的对应命令，初始为隐藏
            if (_addViewLaterButton is not null)
            {
                _addViewLaterButton.Command = ViewModel.AddToViewLaterCommand;
                // 默认隐藏，靠指针进入/离开来控制可见性
                _addViewLaterButton.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            }
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// 控件加载时注册上下文菜单请求事件
    /// </remarks>
    protected override void OnControlLoaded()
        => ContextRequested += OnContextRequested;

    /// <inheritdoc/>
    /// <remarks>
    /// 控件卸载时移除事件订阅
    /// </remarks>
    protected override void OnControlUnloaded()
    {
        ContextRequested -= OnContextRequested;

        if (_rootCard is not null)
        {
            _rootCard.PointerEntered -= OnRootPointerEntered;
            _rootCard.PointerExited -= OnRootPointerExited;
            if (_rootCard is Grid grid)
            {
                grid.Tapped -= OnRootGridTapped;
            }
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// 当 ViewModel 变更时，重新绑定命令到模板元素（如果存在）。
    /// 这样可以保证在复用控件或数据上下文切换时命令正确工作。
    /// </remarks>
    protected override void OnViewModelChanged(VideoItemViewModel? oldValue, VideoItemViewModel? newValue)
    {
        if (_rootCard is ButtonBase buttonBase)
        {
            // 将根按钮的 Command 更新为新的 ViewModel 的播放命令（如果存在）
            buttonBase.Command = newValue?.PlayCommand;
        }

        if (_userButton is not null)
        {
            // 更新用户按钮的命令
            _userButton.Command = newValue?.ShowUserSpaceCommand;
        }

        if (_addViewLaterButton is not null)
        {
            // 更新稍后再看按钮的命令
            _addViewLaterButton.Command = newValue?.AddToViewLaterCommand;
        }
    }

    /// <summary>
    /// 创建“私密播放”菜单项（右键菜单）
    /// </summary>
    private static MenuFlyoutItem CreatePrivatePlayItem()
    {
        return new MenuFlyoutItem()
        {
            // 文本从本地化资源获取
            Text = ResourceToolkit.GetLocalizedString(StringNames.PlayInPrivate),
            // 使用 FluentIcons 显示图标
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.EyeOff },
            // 使用 Tag 来标记应绑定到 ViewModel 的命令名，后续会根据 Tag 映射命令
            Tag = nameof(ViewModel.PlayInPrivateCommand),
        };
    }

    /// <summary>
    /// 创建“进入用户空间”菜单项
    /// </summary>
    private static MenuFlyoutItem CreateUserSpaceItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.EnterUserSpace),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Person },
            Tag = nameof(ViewModel.ShowUserSpaceCommand),
        };
    }

    /// <summary>
    /// 创建“添加到稍后再看”菜单项
    /// </summary>
    private static MenuFlyoutItem CreateAddViewLaterItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.AddToViewLater),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.CalendarAdd },
            Tag = nameof(ViewModel.AddToViewLaterCommand),
        };
    }

    /// <summary>
    /// 创建“从稍后再看移除”菜单项（多用于已在稍后再看的列表中）
    /// </summary>
    private MenuFlyoutItem CreateRemoveViewLaterItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.Remove),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Delete, Foreground = this.Get<IResourceToolkit>().GetThemeBrush("SystemFillColorCriticalBrush") },
            Tag = nameof(ViewModel.RemoveViewLaterCommand),
        };
    }

    /// <summary>
    /// 创建“从历史中移除”菜单项
    /// </summary>
    private MenuFlyoutItem CreateRemoveHistoryItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.Remove),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Delete, Foreground = this.Get<IResourceToolkit>().GetThemeBrush("SystemFillColorCriticalBrush") },
            Tag = nameof(ViewModel.RemoveHistoryCommand),
        };
    }

    /// <summary>
    /// 创建“从收藏中移除”菜单项
    /// </summary>
    private MenuFlyoutItem CreateRemoveFavoriteItem()
    {
        return new MenuFlyoutItem() { Text = ResourceToolkit.GetLocalizedString(StringNames.Remove), Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Delete, Foreground = this.Get<IResourceToolkit>().GetThemeBrush("SystemFillColorCriticalBrush") }, Tag = nameof(ViewModel.RemoveFavoriteCommand) };
    }

    /// <summary>
    /// 创建“在浏览器中打开”菜单项
    /// </summary>
    private static MenuFlyoutItem CreateOpenInBroswerItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.OpenInBrowser),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Globe },
            Tag = nameof(ViewModel.OpenInBroswerCommand),
        };
    }

    /// <summary>
    /// 创建“复制视频链接”菜单项
    /// </summary>
    private static MenuFlyoutItem CreateCopyUriItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.CopyVideoUrl),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Copy },
            Tag = nameof(ViewModel.CopyUriCommand),
        };
    }

    /// <summary>
    /// 创建“固定内容/置顶”菜单项
    /// </summary>
    private static MenuFlyoutItem CreatePinItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.FixContent),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Pin },
            Tag = nameof(ViewModel.PinCommand),
        };
    }

    /// <summary>
    /// 右键/触摸长按弹出上下文菜单的事件处理器
    /// </summary>
    private void OnContextRequested(UIElement sender, ContextRequestedEventArgs args)
    {
        if (ContextFlyout is null)
        {
            // 首次请求时创建上下文菜单
            CreateContextFlyout();
            args.TryGetPosition(this, out var point);
            // 在控件上显示菜单，指定位置
            ContextFlyout.ShowAt(this, new Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowOptions { Position = point });
        }

        // 将菜单项的 Tag 映射到 ViewModel 的命令
        RelocateCommands();
        args.Handled = true;
    }

    /// <summary>
    /// 创建完整的上下文菜单并根据当前 ViewModel 状态添加相应的菜单项
    /// </summary>
    private void CreateContextFlyout()
    {
        var menuFlyout = new MenuFlyout() { ShouldConstrainToRootBounds = false };
        menuFlyout.Items.Add(CreatePrivatePlayItem());
        // 若不是 Moment 风格且用户有效，则添加进入用户空间菜单项
        if (ViewModel.Style != VideoCardStyle.Moment && ViewModel.IsUserValid)
        {
            menuFlyout.Items.Add(CreateUserSpaceItem());
        }

        // 若不是在“稍后再看”列表中，则提供添加选项
        if (ViewModel.Style != VideoCardStyle.ViewLater)
        {
            menuFlyout.Items.Add(CreateAddViewLaterItem());
        }

        menuFlyout.Items.Add(CreateOpenInBroswerItem());
        menuFlyout.Items.Add(CreateCopyUriItem());
        menuFlyout.Items.Add(CreatePinItem());

        // 根据不同的列表风格，添加移除命令
        if (ViewModel.Style == VideoCardStyle.ViewLater)
        {
            menuFlyout.Items.Add(CreateRemoveViewLaterItem());
        }
        else if (ViewModel.Style == VideoCardStyle.History)
        {
            menuFlyout.Items.Add(CreateRemoveHistoryItem());
        }
        else if (ViewModel.Style == VideoCardStyle.Favorite)
        {
            menuFlyout.Items.Add(CreateRemoveFavoriteItem());
        }

        ContextFlyout = menuFlyout;
    }

    /// <summary>
    /// 将菜单项的 Tag 映射到实际的 ViewModel 命令
    /// </summary>
    private void RelocateCommands()
    {
        if (ContextFlyout is not MenuFlyout flyout)
        {
            return;
        }

        foreach (var item in flyout.Items.OfType<MenuFlyoutItem>())
        {
            // 通过 Tag 的值来设置对应的 Command
            switch (item.Tag.ToString())
            {
                case nameof(ViewModel.PlayInPrivateCommand):
                    item.Command = ViewModel.PlayInPrivateCommand;
                    break;
                case nameof(ViewModel.ShowUserSpaceCommand):
                    item.Command = ViewModel.ShowUserSpaceCommand;
                    break;
                case nameof(ViewModel.AddToViewLaterCommand):
                    item.Command = ViewModel.AddToViewLaterCommand;
                    break;
                case nameof(ViewModel.RemoveViewLaterCommand):
                    item.Command = ViewModel.RemoveViewLaterCommand;
                    break;
                case nameof(ViewModel.RemoveHistoryCommand):
                    item.Command = ViewModel.RemoveHistoryCommand;
                    break;
                case nameof(ViewModel.RemoveFavoriteCommand):
                    item.Command = ViewModel.RemoveFavoriteCommand;
                    break;
                case nameof(ViewModel.OpenInBroswerCommand):
                    item.Command = ViewModel.OpenInBroswerCommand;
                    break;
                case nameof(ViewModel.CopyUriCommand):
                    item.Command = ViewModel.CopyUriCommand;
                    break;
                case nameof(ViewModel.PinCommand):
                    item.Command = ViewModel.PinCommand;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 指针进入根卡片时的处理：显示“稍后再看”按钮
    /// </summary>
    private void OnRootPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (_addViewLaterButton is not null)
        {
            _addViewLaterButton.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
    }

    /// <summary>
    /// 指针离开根卡片时的处理：隐藏“稍后再看”按钮
    /// </summary>
    private void OnRootPointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (_addViewLaterButton is not null)
        {
            _addViewLaterButton.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }

    /// <summary>
    /// 针对 Moment 风格（根元素为 Grid）处理 Tapped 事件的逻辑
    /// 需要在点击发生在“稍后再看”按钮上时阻止播放逻辑。
    /// </summary>
    private void OnRootGridTapped(object sender, TappedRoutedEventArgs e)
    {
        // 如果 OriginalSource 在视觉树上属于 _addViewLaterButton，则不触发播放
        if (e.OriginalSource is DependencyObject source)
        {
            var element = source;
            while (element != null)
            {
                if (element == _addViewLaterButton)
                {
                    // 点击命中在“稍后再看”按钮，直接返回
                    return;
                }

                element = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(element);
            }
        }

        // 触发播放命令（如果可以执行）
        if (ViewModel?.PlayCommand?.CanExecute(default) == true)
        {
            ViewModel.PlayCommand.Execute(default);
        }
    }
}
