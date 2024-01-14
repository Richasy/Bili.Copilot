// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels.Items;

/// <summary>
/// 导航条目.
/// </summary>
public sealed partial class NavigateItemViewModel : SelectableViewModel<NavigateItem>
{
    [ObservableProperty]
    private FluentSymbol _defaultIcon;

    [ObservableProperty]
    private FluentSymbol _selectedIcon;

    [ObservableProperty]
    private bool _isVisible;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigateItemViewModel"/> class.
    /// </summary>
    public NavigateItemViewModel(NavigateItem data)
        : base(data)
    {
        switch (data.Id)
        {
            case PageType.Partition:
                DefaultIcon = FluentSymbol.Apps;
                SelectedIcon = FluentSymbol.AppsFilled;
                break;
            case PageType.Popular:
                DefaultIcon = FluentSymbol.Rocket;
                SelectedIcon = FluentSymbol.RocketFilled;
                break;
            case PageType.Live:
                DefaultIcon = FluentSymbol.Video;
                SelectedIcon = FluentSymbol.VideoFilled;
                break;
            case PageType.Dynamic:
                DefaultIcon = FluentSymbol.DesignIdeas;
                SelectedIcon = FluentSymbol.DesignIdeasFilled;
                break;
            case PageType.Anime:
                DefaultIcon = FluentSymbol.Dust;
                SelectedIcon = FluentSymbol.DustFilled;
                break;
            case PageType.Film:
                DefaultIcon = FluentSymbol.FilmstripPlay;
                SelectedIcon = FluentSymbol.FilmstripPlayFilled;
                break;
            case PageType.Article:
                DefaultIcon = FluentSymbol.DocumentBulletList;
                SelectedIcon = FluentSymbol.DocumentBulletListFilled;
                break;
            case PageType.Settings:
                DefaultIcon = FluentSymbol.Settings;
                SelectedIcon = FluentSymbol.SettingsFilled;
                break;
            case PageType.WebDav:
                DefaultIcon = FluentSymbol.CloudDatabase;
                SelectedIcon = FluentSymbol.CloudDatabaseFilled;
                break;
            default:
                break;
        }
    }
}
