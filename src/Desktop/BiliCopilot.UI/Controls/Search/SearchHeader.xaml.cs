// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Search;

/// <summary>
/// 搜索页头部.
/// </summary>
public sealed partial class SearchHeader : SearchPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchHeader"/> class.
    /// </summary>
    public SearchHeader() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);
}
