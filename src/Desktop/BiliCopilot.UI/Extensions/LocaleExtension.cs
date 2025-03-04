// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Markup;

namespace BiliCopilot.UI.Extensions;

/// <summary>
/// Localized text extension.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed partial class LocaleExtension : MarkupExtension
{
    /// <summary>
    /// Language name.
    /// </summary>
    public StringNames Name { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
        => ResourceToolkit.GetLocalizedString(Name);
}
