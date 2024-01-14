// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// WebDav分集视图.
/// </summary>
public sealed partial class WebDavPartView : WebDavPartViewBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPartView"/> class.
    /// </summary>
    public WebDavPartView() => InitializeComponent();

    private void OnItemClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as WebDavStorageItemViewModel;
        ViewModel.ChangeVideoCommand.Execute(data);
    }
}

/// <summary>
/// <see cref="WebDavPartView"/> 的基类.
/// </summary>
public abstract class WebDavPartViewBase : ReactiveUserControl<WebDavPlayerPageViewModel>
{
}
