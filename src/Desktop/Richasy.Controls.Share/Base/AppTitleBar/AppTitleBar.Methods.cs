// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.Graphics;

namespace Richasy.WinUI.Share.Base;

/// <summary>
/// 应用标题栏.
/// </summary>
public sealed partial class AppTitleBar
{
    private void UpdateIcon()
    {
        if (IconElement is not null)
        {
            VisualStateManager.GoToState(this, IconVisibleVisualStateName, false);
        }
        else
        {
            VisualStateManager.GoToState(this, IconCollapsedVisualStateName, false);
        }
    }

    private void UpdateBackButton()
    {
        if (IsBackButtonVisible)
        {
            if (_backButton is null)
            {
                LoadBackButton();
            }

            VisualStateManager.GoToState(this, BackButtonVisibleVisualStateName, false);
        }
        else
        {
            VisualStateManager.GoToState(this, BackButtonCollapsedVisualStateName, false);
        }

        UpdateInteractableElementsList();
    }

    private void UpdatePaneToggleButton()
    {
        if (IsPaneToggleButtonVisible)
        {
            if (_paneToggleButton is null)
            {
                LoadPaneToggleButton();
            }

            VisualStateManager.GoToState(this, PaneToggleButtonVisibleVisualStateName, false);
        }
        else
        {
            VisualStateManager.GoToState(this, PaneToggleButtonCollapsedVisualStateName, false);
        }

        UpdateInteractableElementsList();
    }

    private void UpdateHeight()
    {
        var stateName = Content is null && Header is null && Footer is null ? CompactHeightVisualStateName : ExpandedHeightVisualStateName;
        VisualStateManager.GoToState(this, stateName, false);
    }

    private void UpdatePadding()
    {
        var islandEnv = XamlRoot?.ContentIslandEnvironment;
        if (islandEnv is not null)
        {
            var appWindowId = islandEnv.AppWindowId;
            var appWindow = AppWindow.GetFromWindowId(appWindowId);
            var titleBar = appWindow.TitleBar;
            var scale = XamlRoot.RasterizationScale;
            if (_leftPaddingColumn is not null)
            {
                _leftPaddingColumn.Width = new GridLength(titleBar.LeftInset / scale, GridUnitType.Pixel);
            }

            if (_rightPaddingColumn is not null)
            {
                _rightPaddingColumn.Width = new GridLength(titleBar.RightInset / scale, GridUnitType.Pixel);
            }
        }
    }

    private void UpdateTheme()
    {
        var islandEnv = XamlRoot?.ContentIslandEnvironment;
        if (islandEnv is not null)
        {
            var appWindowId = islandEnv.AppWindowId;
            var appWindow = AppWindow.GetFromWindowId(appWindowId);
            var titleBar = appWindow.TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }
    }

    private void UpdateTitle()
    {
        if (!string.IsNullOrEmpty(Title))
        {
            VisualStateManager.GoToState(this, TitleTextVisibleVisualStateName, false);
        }
        else
        {
            VisualStateManager.GoToState(this, TitleTextCollapsedVisualStateName, false);
        }
    }

    private void UpdateSubtitle()
    {
        if (!string.IsNullOrEmpty(Subtitle))
        {
            VisualStateManager.GoToState(this, SubtitleTextVisibleVisualStateName, false);
        }
        else
        {
            VisualStateManager.GoToState(this, SubtitleTextCollapsedVisualStateName, false);
        }
    }

    private void UpdateHeader()
    {
        if (Header is null)
        {
            VisualStateManager.GoToState(this, HeaderCollapsedVisualStateName, false);
        }
        else
        {
            _headerArea ??= GetTemplateChild(HeaderContentPresenterPartName) as FrameworkElement;
            VisualStateManager.GoToState(this, HeaderVisibleVisualStateName, false);
        }

        UpdateHeight();
        UpdateInteractableElementsList();
    }

    private void UpdateContent()
    {
        if (Content is null)
        {
            VisualStateManager.GoToState(this, ContentCollapsedVisualStateName, false);
        }
        else
        {
            if (_contentArea is null)
            {
                _contentAreaGrid = GetTemplateChild(ContentPresenterGridPartName) as Grid;
                _contentArea = GetTemplateChild(ContentPresenterPartName) as FrameworkElement;
            }

            VisualStateManager.GoToState(this, ContentVisibleVisualStateName, false);
        }

        UpdateHeight();
        UpdateInteractableElementsList();
    }

    private void UpdateFooter()
    {
        if (Footer is null)
        {
            VisualStateManager.GoToState(this, FooterCollapsedVisualStateName, false);
        }
        else
        {
            _footerArea ??= GetTemplateChild(FooterContentPresenterPartName) as FrameworkElement;
            VisualStateManager.GoToState(this, FooterVisibleVisualStateName, false);
        }

        UpdateHeight();
        UpdateInteractableElementsList();
    }

    private void UpdateDragRegion()
    {
        var islandEnv = XamlRoot?.ContentIslandEnvironment;
        if (islandEnv is not null)
        {
            var appWindowId = islandEnv.AppWindowId;
            var nonClientPointerSource = InputNonClientPointerSource.GetForWindowId(appWindowId);

            if (_interactableElementList.Count > 0)
            {
                var passthroughRects = new List<RectInt32>();
                foreach (var element in _interactableElementList)
                {
                    var transformBounds = element.TransformToVisual(default);
                    var width = element.ActualWidth;
                    var height = element.ActualHeight;
                    var bounds = transformBounds.TransformBounds(new Rect(0, 0, width, height));

                    if (bounds.X < 0 || bounds.Y < 0)
                    {
                        continue;
                    }

                    var scale = XamlRoot.RasterizationScale;
                    var transparentRect = new RectInt32(
                        Convert.ToInt32(bounds.X * scale),
                        Convert.ToInt32(bounds.Y * scale),
                        Convert.ToInt32(bounds.Width * scale),
                        Convert.ToInt32(bounds.Height * scale));
                    passthroughRects.Add(transparentRect);
                }

                nonClientPointerSource.SetRegionRects(NonClientRegionKind.Passthrough, passthroughRects.ToArray());
            }
            else
            {
                nonClientPointerSource.ClearRegionRects(NonClientRegionKind.Passthrough);
            }
        }
    }

    private void UpdateInteractableElementsList()
    {
        _interactableElementList.Clear();
        if (IsBackButtonVisible && IsBackEnabled && _backButton is not null)
        {
            _interactableElementList.Add(_backButton);
        }

        if (IsPaneToggleButtonVisible && _paneToggleButton is not null)
        {
            _interactableElementList.Add(_paneToggleButton);
        }

        if (Header is not null && _headerArea is not null)
        {
            _interactableElementList.Add(_headerArea);
        }

        if (Content is not null && _contentArea is not null)
        {
            _interactableElementList.Add(_contentArea);
        }

        if (Footer is not null && _footerArea is not null)
        {
            _interactableElementList.Add(_footerArea);
        }
    }

    private void LoadBackButton()
    {
        _backButton = GetTemplateChild(BackButtonPartName) as Button;
        if (_backButton is not null)
        {
            _backButton.Click += OnBackButtonClick;
        }
    }

    private void LoadPaneToggleButton()
    {
        _paneToggleButton = GetTemplateChild(PaneToggleButtonPartName) as Button;
        if (_paneToggleButton is not null)
        {
            _paneToggleButton.Click += OnPaneToggleButtonClick;
        }
    }
}
