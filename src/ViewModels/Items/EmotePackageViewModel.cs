// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Appearance;

namespace Bili.Copilot.ViewModels.Items;

/// <summary>
/// 表情包视图模型.
/// </summary>
public sealed partial class EmotePackageViewModel : SelectableViewModel<EmotePackage>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmotePackageViewModel"/> class.
    /// </summary>
    public EmotePackageViewModel(EmotePackage data)
        : base(data)
    {
    }
}
