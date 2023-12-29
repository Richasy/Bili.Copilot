// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 固定模块.
/// </summary>
public sealed partial class FixModule : FixModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FixModule"/> class.
    /// </summary>
    public FixModule()
    {
        InitializeComponent();
        ViewModel = FixModuleViewModel.Instance;
        Loaded += OnLoaded;
    }

    /// <summary>
    /// 可见性改变事件.
    /// </summary>
    public event EventHandler VisibilityChanged;

    private void UnpinItemClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement)?.DataContext as FixedItem;
        FixModuleViewModel.Instance.RemoveFixedItemCommand.Execute(data?.Id);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.HasFixedItems))
        {
            if (!ViewModel.HasFixedItems && FixFlyout.IsOpen)
            {
                FixFlyout.Hide();
            }

            VisibilityChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnFixedItemClick(object sender, RoutedEventArgs e)
    {
        var item = (sender as FrameworkElement).DataContext as FixedItem;
        PlaySnapshot playRecord = null;
        switch (item.Type)
        {
            case FixedType.User:
                AppViewModel.Instance.ShowUserDetailCommand.Execute(new Models.Data.User.UserProfile(item.Id));
                break;
            case FixedType.Pgc:
                playRecord = new PlaySnapshot(default, item.Id, VideoType.Pgc)
                {
                    Title = item.Title,
                };
                break;
            case FixedType.Video:
                playRecord = new PlaySnapshot(item.Id, default, VideoType.Video);
                break;

            case FixedType.Live:
                playRecord = new PlaySnapshot(item.Id, default, VideoType.Live);
                break;
            default:
                break;
        }

        if (playRecord != null)
        {
            AppViewModel.Instance.OpenPlayerCommand.Execute(playRecord);
        }
    }
}

/// <summary>
/// <see cref="FixModule"/> 的基类.
/// </summary>
public abstract class FixModuleBase : ReactiveUserControl<FixModuleViewModel>
{
}
