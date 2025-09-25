// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Pages;
using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls;

public sealed partial class OverlayFrame : LayoutUserControlBase
{
    public OverlayFrame() => InitializeComponent();

    public List<object> GetBackStack()
        => [.. RootGrid.Children];

    public void ClearBackStack()
    {
        RootGrid.Children.Clear();
    }

    public void GoBack()
    {
        if (RootGrid.Children.Count > 0)
        {
            RootGrid.Children.RemoveAt(RootGrid.Children.Count - 1);
        }

        // 如果还有元素，则将最后一个元素设置为可见
        if (RootGrid.Children.Count > 0)
        {
            foreach (var child in RootGrid.Children)
            {
                child.Visibility = Visibility.Collapsed;
            }

            RootGrid.Children[^1].Visibility = Visibility.Visible;
        }
    }

    public void NavigateTo(Type pageType, object? parameter = null)
    {
        if (pageType is null)
        {
            return;
        }

        UIElement? targetElement = null;

        // 查找是否已存在该类型的控件
        foreach (var item in RootGrid.Children)
        {
            if (item.GetType() == pageType)
            {
                targetElement = item;
                // 如果实现了 IMainFramePage，则调用 SetParameter
                if (item is IParameterPage mainFramePage)
                {
                    mainFramePage.SetParameter(parameter);
                }
            }
        }

        if (targetElement == null)
        {
            // 未找到，创建新实例
            var instance = Activator.CreateInstance(pageType);
            if (instance is UIElement element)
            {
                targetElement = element;
                // 如果实现了 IMainFramePage，则调用 SetParameter
                if (element is IParameterPage mainFramePage)
                {
                    mainFramePage.SetParameter(parameter);
                }

                RootGrid.Children.Add(element);
            }
            else
            {
                // 类型不是 UIElement，直接返回
                return;
            }
        }
        else if (RootGrid.Children.Count > 1)
        {
            // 将 targetElement 移动到最后
            RootGrid.Children.Move(Convert.ToUInt32(RootGrid.Children.IndexOf(targetElement)), Convert.ToUInt32(RootGrid.Children.Count - 1));
        }

        // 设置可见性
        foreach (var child in RootGrid.Children)
        {
            child.Visibility = child == targetElement ? Visibility.Visible : Visibility.Collapsed;
            if (child.Visibility == Visibility.Collapsed && child is ICancelPageViewModel cancelPageViewModel)
            {
                cancelPageViewModel.CancelLoading();
            }
        }
    }

    public object? GetCurrentContent()
    {
        if (RootGrid.Children.Count > 0)
        {
            return RootGrid.Children[^1];
        }

        return default;
    }
}
