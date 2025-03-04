// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 动态头部.
/// </summary>
public sealed partial class MomentHeader : MomentHeaderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MomentHeader"/> class.
    /// </summary>
    public MomentHeader() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Selector.SelectionChanged += OnSelectorChanged;
        ViewModel.Initialized += OnViewModelInitialized;
        InitializeChildPartitions();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.Initialized -= OnViewModelInitialized;
        }

        Selector.SelectionChanged -= OnSelectorChanged;
    }

    private static string GetSectionName(MomentSectionType type)
        => type switch
        {
            MomentSectionType.Video => ResourceToolkit.GetLocalizedString(StringNames.Video),
            MomentSectionType.Comprehensive => ResourceToolkit.GetLocalizedString(StringNames.Comprehensive),
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

    private void OnViewModelInitialized(object? sender, EventArgs e)
        => InitializeChildPartitions();

    private void OnSelectorChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if (sender.SelectedItem is null)
        {
            return;
        }

        var item = sender.SelectedItem.Tag as IMomentSectionDetailViewModel;
        if (item is not null && item != ViewModel.SelectedSection)
        {
            ViewModel.SelectSectionCommand.Execute(item);
        }
    }

    private void InitializeChildPartitions()
    {
        Selector.Items.Clear();
        if (ViewModel.Sections is not null)
        {
            foreach (var item in ViewModel.Sections)
            {
                Selector.Items.Add(new SelectorBarItem()
                {
                    Text = GetSectionName(item.SectionType),
                    Tag = item,
                });
            }
        }

        Selector.SelectedItem = Selector.Items.FirstOrDefault(p => p.Tag == ViewModel.SelectedSection);
    }
}

/// <summary>
/// 动态头部基类.
/// </summary>
public abstract class MomentHeaderBase : LayoutUserControlBase<MomentPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MomentHeaderBase"/> class.
    /// </summary>
    protected MomentHeaderBase() => ViewModel = this.Get<MomentPageViewModel>();
}
