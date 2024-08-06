// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Search;

/// <summary>
/// PGC搜索分区详情控件.
/// </summary>
public sealed partial class PgcSectionDetailControl : PgcSectionDetailControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcSectionDetailControl"/> class.
    /// </summary>
    public PgcSectionDetailControl()
    {
        InitializeComponent();
    }
}

/// <summary>
/// PGC搜索分区详情控件基类.
/// </summary>
public abstract class PgcSectionDetailControlBase : LayoutUserControlBase<PgcSearchSectionDetailViewModel>
{
}
