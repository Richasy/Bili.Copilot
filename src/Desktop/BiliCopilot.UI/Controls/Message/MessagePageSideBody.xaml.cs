﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Message;

/// <summary>
/// 消息页面侧边栏主体.
/// </summary>
public sealed partial class MessagePageSideBody : MessagePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePageSideBody"/> class.
    /// </summary>
    public MessagePageSideBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.SectionInitialized += OnSectionInitialized;
        SectionView.SelectionChanged += OnSectionSelectionChanged;
        CheckSectionSelection();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.SectionInitialized -= OnSectionInitialized;
        SectionView.SelectionChanged -= OnSectionSelectionChanged;
    }

    private void OnSectionInitialized(object? sender, EventArgs e)
        => CheckSectionSelection();

    private void OnSectionSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var item = sender.SelectedItem as IMessageSectionDetailViewModel;
        ViewModel.SelectSectionCommand.Execute(item);
    }

    private void CheckSectionSelection()
    {
        if (ViewModel.CurrentSection is not null)
        {
            SectionView.Select(ViewModel.Sections.IndexOf(ViewModel.CurrentSection));
        }
    }
}
