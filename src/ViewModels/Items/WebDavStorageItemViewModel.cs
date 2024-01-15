// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using WebDav;

namespace Bili.Copilot.ViewModels.Items;

/// <summary>
/// WebDav 存储项视图模型.
/// </summary>
public sealed partial class WebDavStorageItemViewModel : SelectableViewModel<WebDavResource>
{
    [ObservableProperty]
    private bool _isFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavStorageItemViewModel"/> class.
    /// </summary>
    public WebDavStorageItemViewModel(WebDavResource item)
        : base(item)
    {
        IsFolder = item.IsCollection;
    }
}
