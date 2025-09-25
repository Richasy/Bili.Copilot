// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Pages;
using BiliCopilot.UI.ViewModels.View;
using CommunityToolkit.WinUI.Animations;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls;

public sealed partial class MainFrame : LayoutUserControlBase
{
    public MainFrame() => InitializeComponent();

    public object GetCurrentContent()
    {
        // ���ص�ǰ�ɼ�����Ԫ��
        foreach (var child in RootGrid.Children)
        {
            if (child.Visibility == Visibility.Visible)
            {
                return child;
            }
        }

        return null!;
    }

    public void NavigateTo(Type pageType, object? parameter = null)
    {
        if (pageType is null)
        {
            return;
        }

        UIElement? targetElement = null;

        // �����Ƿ��Ѵ��ڸ����͵Ŀؼ�
        foreach (var item in RootGrid.Children)
        {
            if (item.GetType() == pageType)
            {
                targetElement = item;
                // ���ʵ���� IMainFramePage������� SetParameter
                if (item is IParameterPage mainFramePage)
                {
                    mainFramePage.SetParameter(parameter);
                }

                if (item is IInitializePage initPage)
                {
                    initPage.Initialize();
                }
            }
        }

        if (targetElement == null)
        {
            // δ�ҵ���������ʵ��
            var instance = Activator.CreateInstance(pageType);
            if (instance is UIElement element)
            {
                targetElement = element;
                // ���ʵ���� IMainFramePage������� SetParameter
                if (element is IParameterPage mainFramePage)
                {
                    mainFramePage.SetParameter(parameter);
                }

                var showOpacityAnimation = new OpacityAnimation
                {
                    Duration = TimeSpan.FromMilliseconds(200),
                    From = 0,
                    To = 1,
                };
                var hideOpacityAnimation = new OpacityAnimation
                {
                    Duration = TimeSpan.FromMilliseconds(100),
                    From = 1,
                    To = 0,
                };
                var showSet = new ImplicitAnimationSet
                {
                    showOpacityAnimation
                };
                var hideSet = new ImplicitAnimationSet
                {
                    hideOpacityAnimation
                };
                Implicit.SetShowAnimations(element, showSet);
                Implicit.SetHideAnimations(element, hideSet);
                RootGrid.Children.Add(element);
            }
            else
            {
                // ���Ͳ��� UIElement��ֱ�ӷ���
                return;
            }
        }

        // ���ÿɼ���
        foreach (var child in RootGrid.Children)
        {
            child.Visibility = child == targetElement ? Visibility.Visible : Visibility.Collapsed;
            if (child.Visibility == Visibility.Collapsed && child is ICancelPageViewModel cancelPageViewModel)
            {
                cancelPageViewModel.CancelLoading();
            }
        }
    }
}
