// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.WinUIKernel.Share.Toolkits;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels;

/// <summary>
/// 布局页面视图模型基类.
/// </summary>
public abstract partial class LayoutPageViewModelBase : ViewModelBase
{
    [ObservableProperty]
    private double _navColumnWidth;

    [ObservableProperty]
    private bool _isNavColumnManualHide;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutPageViewModelBase"/> class.
    /// </summary>
    protected LayoutPageViewModelBase()
    {
#pragma warning disable CA2214
        NavColumnWidth = this.Get<ISettingsToolkit>().ReadLocalSetting($"{GetPageKey()}NavColumnWidth", GetDefaultNavColumnWidth());
        IsNavColumnManualHide = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{GetPageKey()}NavColumnManualHide", false);
#pragma warning restore CA2214
    }

    /// <summary>
    /// 获取页面名称.
    /// </summary>
    /// <returns>页面名称.</returns>
    protected abstract string GetPageKey();

    /// <summary>
    /// 获取默认导航栏宽度.
    /// </summary>
    /// <returns>宽度.</returns>
    protected virtual double GetDefaultNavColumnWidth() => 240d;

    /// <summary>
    /// 导航栏手动关闭时的行为.
    /// </summary>
    protected virtual void IsNavManualHideChanged(bool value)
        => NavColumnWidth = value ? 0 : this.Get<ISettingsToolkit>().ReadLocalSetting($"{GetPageKey()}NavColumnWidth", 240d);

    partial void OnNavColumnWidthChanged(double value)
    {
        if (value > 0)
        {
            this.Get<ISettingsToolkit>().WriteLocalSetting($"{GetPageKey()}NavColumnWidth", value);
        }
    }

    partial void OnIsNavColumnManualHideChanged(bool value)
    {
        this.Get<ISettingsToolkit>().WriteLocalSetting($"Is{GetPageKey()}NavColumnManualHide", value);
        IsNavManualHideChanged(value);
    }
}
