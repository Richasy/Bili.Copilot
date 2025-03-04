// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.BiliKernel.Models.User;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Users;

/// <summary>
/// 关注页面侧边栏.
/// </summary>
public sealed partial class FollowsSideBody : FollowsPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FollowsSideBody"/> class.
    /// </summary>
    public FollowsSideBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.GroupInitialized += OnGroupInitialized;
        GroupView.SelectionChanged += OnGroupSelectionChanged;
        CheckGroupSelection();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        GroupView.SelectionChanged -= OnGroupSelectionChanged;
        ViewModel.GroupInitialized -= OnGroupInitialized;
    }

    private void OnGroupInitialized(object? sender, EventArgs e)
        => CheckGroupSelection();

    private void OnGroupSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var item = sender.SelectedItem as UserGroup;
        ViewModel.SelectGroupCommand.Execute(item);
    }

    private void CheckGroupSelection()
    {
        if (ViewModel.SelectedGroup is not null)
        {
            GroupView.Select(ViewModel.Groups.IndexOf(ViewModel.SelectedGroup));
        }
    }
}
