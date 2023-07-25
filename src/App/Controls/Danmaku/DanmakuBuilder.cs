// Copyright (c) Bili Copilot. All rights reserved.

using System;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Bili.Copilot.App.Controls.Danmaku;

/// <summary>
/// 弹幕组件构建器.
/// </summary>
public class DanmakuBuilder
{
    private double _sizeZoom = 1d;
    private bool _isBold = false;
    private string _fontFamily = "Segoe UI";
    private DanmakuModel _model = null;

    /// <summary>
    /// 设置缩放比例.
    /// </summary>
    /// <param name="sizeZoom">缩放比例.</param>
    /// <returns>构造器.</returns>
    public DanmakuBuilder WithSizeZoom(double sizeZoom)
    {
        _sizeZoom = sizeZoom;
        return this;
    }

    /// <summary>
    /// 设置字体是否加粗.
    /// </summary>
    /// <param name="isBold">是否加粗.</param>
    /// <returns>构造器.</returns>
    public DanmakuBuilder WithBold(bool isBold)
    {
        _isBold = isBold;
        return this;
    }

    /// <summary>
    /// 设置字体.
    /// </summary>
    /// <param name="fontFamily">字体名.</param>
    /// <returns>构造器.</returns>
    public DanmakuBuilder WithFontFamily(string fontFamily)
    {
        _fontFamily = fontFamily;
        return this;
    }

    /// <summary>
    /// 设置弹幕模型.
    /// </summary>
    /// <param name="model">模型.</param>
    /// <returns>构造器.</returns>
    public DanmakuBuilder WithDanmakuModel(DanmakuModel model)
    {
        _model = model;
        return this;
    }

    /// <summary>
    /// 创建重叠弹幕.
    /// </summary>
    /// <returns>弹幕容器.</returns>
    public Grid CreateOverlapDanamku()
    {
        if (_model == null)
        {
            throw new ArgumentNullException("未传入弹幕模型.");
        }

        // 创建基础控件
        var tx = new TextBlock();
        var tx2 = new TextBlock();
        var grid = new Grid();

        tx2.Text = _model.Text;
        tx.Text = _model.Text;

        if (_isBold)
        {
            tx.FontWeight = FontWeights.Bold;
            tx2.FontWeight = FontWeights.Bold;
        }

        if (!string.IsNullOrEmpty(_fontFamily))
        {
            tx.FontFamily = new FontFamily(_fontFamily);
            tx2.FontFamily = new FontFamily(_fontFamily);
        }

        tx2.Foreground = _model.Foreground;
        tx.Foreground = _model.Foreground;

        // 弹幕大小
        var size = _model.Size * _sizeZoom;

        tx2.FontSize = size;
        tx.FontSize = size;

        tx2.Margin = new Thickness(1);

        // Grid包含弹幕文本信息
        grid.Children.Add(tx2);
        grid.Children.Add(tx);
        grid.Tag = _model;
        return grid;
    }

    /// <summary>
    /// 创建无边框弹幕.
    /// </summary>
    /// <returns>弹幕容器.</returns>
    public Grid CreateNoStrokeDanmaku()
    {
        if (_model == null)
        {
            throw new ArgumentNullException("未传入弹幕模型.");
        }

        // 创建基础控件
        var tx = new TextBlock();
        var grid = new Grid();

        tx.Text = _model.Text;
        if (_isBold)
        {
            tx.FontWeight = FontWeights.Bold;
        }

        if (!string.IsNullOrEmpty(_fontFamily))
        {
            tx.FontFamily = new FontFamily(_fontFamily);
        }

        tx.Foreground = _model.Foreground;
        var size = _model.Size * _sizeZoom;
        tx.FontSize = size;
        grid.Children.Add(tx);
        grid.Tag = _model;
        return grid;
    }

    /// <summary>
    /// 创建阴影弹幕.
    /// </summary>
    /// <returns>弹幕容器.</returns>
    public Grid CreateShadowDanmaku()
    {
        if (_model == null)
        {
            throw new ArgumentNullException("未传入弹幕模型.");
        }

        // 创建基础控件
        var tx = new TextBlock();
        tx.Text = _model.Text;
        if (_isBold)
        {
            tx.FontWeight = FontWeights.Bold;
        }

        if (!string.IsNullOrEmpty(_fontFamily))
        {
            tx.FontFamily = new FontFamily(_fontFamily);
        }

        tx.Foreground = _model.Foreground;
        var size = _model.Size * _sizeZoom;
        tx.FontSize = size;

        var grid = new Grid();
        var hostGrid = new Grid();

        var shadow = new AttachedDropShadow();
        shadow.BlurRadius = 2;
        shadow.Opacity = 0.8;
        shadow.Offset = "1,1,0";
        shadow.Color = _model.Color.R <= 80 ? Colors.White : Colors.Black;
        grid.Children.Add(hostGrid);
        grid.Children.Add(tx);
        grid.Loaded += (s, e) =>
        {
            shadow.CastTo = hostGrid;
        };

        Effects.SetShadow(tx, shadow);
        grid.Tag = _model;
        return grid;
    }
}
