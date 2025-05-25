// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

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
    protected override void OnControlLoaded()
    {
        ViewModel.SectionInitialized += OnSectionInitialized;
        ViewModel.ChatSessionsUpdated += OnChatSessionsUpdatedAsync;
        SectionView.SelectionChanged += OnSectionSelectionChanged;
        SectionView.ScrollView.ViewChanged += OnViewChanged;
        CheckSectionSelection();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.SectionInitialized -= OnSectionInitialized;
        ViewModel.ChatSessionsUpdated -= OnChatSessionsUpdatedAsync;
        SectionView.SelectionChanged -= OnSectionSelectionChanged;
        SectionView.ScrollView.ViewChanged -= OnViewChanged;
    }

    private void OnSectionInitialized(object? sender, EventArgs e)
        => CheckSectionSelection();

    private void OnSectionSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var item = sender.SelectedItem as IMessageSectionDetailViewModel;
        ViewModel.SelectSectionCommand.Execute(item);
    }

    private void OnViewChanged(ScrollView sender, object args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (SectionView.ScrollView.ExtentHeight - SectionView.ScrollView.ViewportHeight - SectionView.ScrollView.VerticalOffset <= 240)
            {
                ViewModel.LoadChatSessionsCommand.Execute(default);
            }
        });
    }

    private async void OnChatSessionsUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckChatSessionCount();
    }

    private void CheckSectionSelection()
    {
        if (ViewModel.CurrentSection is not null)
        {
            SectionView.Select(ViewModel.Sections.IndexOf(ViewModel.CurrentSection));
        }
    }

    private void CheckChatSessionCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (SectionView.ScrollView.ScrollableHeight <= 240 && ViewModel is not null)
            {
                ViewModel.LoadChatSessionsCommand.Execute(default);
            }
        });
    }
}
