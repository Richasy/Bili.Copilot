// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 消息详情模块.
/// </summary>
public sealed partial class MessageDetailModule : MessageDetailModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDetailModule"/> class.
    /// </summary>
    public MessageDetailModule() => InitializeComponent();

    private void OnIncrementalTriggered(object sender, System.EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);

    private void OnNavItemInvokedAsync(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        ContentScrollViewer.ChangeView(0, 0, 1, true);
        var data = args.InvokedItem as MessageHeaderViewModel;

        if (data != ViewModel.CurrentType)
        {
            ViewModel.SelectTypeCommand.Execute(data);
        }
    }
}

/// <summary>
/// <see cref="MessageDetailModule"/> 的基类.
/// </summary>
public abstract class MessageDetailModuleBase : ReactiveUserControl<MessageDetailViewModel>
{
}
