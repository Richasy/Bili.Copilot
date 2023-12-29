// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.Items;
using static Bilibili.Api.Ticket.V1.GetTicketResponse.Types;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 直播导航列表模块.
/// </summary>
public sealed partial class LiveNavListModule : LiveNavListModuleBase
{
    private readonly LiveRecommendDetailViewModel _recommend;
    private readonly LivePartitionIndexViewModel _partition;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveNavListModule"/> class.
    /// </summary>
    public LiveNavListModule()
    {
        InitializeComponent();
        ViewModel = LivePageViewModel.Instance;
        _recommend = LiveRecommendDetailViewModel.Instance;
        _partition = LivePartitionIndexViewModel.Instance;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => LiveTypeSelection.SelectedIndex = (int)ViewModel.CurrentType;

    private void OnLiveTypeSegmentedSelectionChanged(object sender, SelectionChangedEventArgs e)
        => ViewModel.CurrentType = (LiveDisplayType)LiveTypeSelection.SelectedIndex;

    private void OnPartitionItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
    {
        _ = this;
        var vm = args.InvokedItem as PartitionItemViewModel;
        if (vm.Children != null)
        {
            vm.IsExpanded = !vm.IsExpanded;
        }
        else
        {
            _partition.OpenPartitionCommand.Execute(vm.Data);
        }
    }
}

/// <summary>
/// 直播导航列表模块基类.
/// </summary>
public abstract class LiveNavListModuleBase : ReactiveUserControl<LivePageViewModel>
{
}
