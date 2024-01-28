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

    [ObservableProperty]
    private bool _hasUnread;

    [ObservableProperty]
    private string _accessKey;

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
                AccessKey = "3";
                break;
            case PageType.Popular:
                DefaultIcon = FluentSymbol.Rocket;
                SelectedIcon = FluentSymbol.RocketFilled;
                AccessKey = "1";
                break;
            case PageType.Live:
                DefaultIcon = FluentSymbol.Video;
                SelectedIcon = FluentSymbol.VideoFilled;
                AccessKey = "4";
                break;
            case PageType.Dynamic:
                DefaultIcon = FluentSymbol.DesignIdeas;
                SelectedIcon = FluentSymbol.DesignIdeasFilled;
                AccessKey = "2";
                break;
            case PageType.Anime:
                DefaultIcon = FluentSymbol.Dust;
                SelectedIcon = FluentSymbol.DustFilled;
                AccessKey = "5";
                break;
            case PageType.Film:
                DefaultIcon = FluentSymbol.FilmstripPlay;
                SelectedIcon = FluentSymbol.FilmstripPlayFilled;
                AccessKey = "6";
                break;
            case PageType.Article:
                DefaultIcon = FluentSymbol.DocumentBulletList;
                SelectedIcon = FluentSymbol.DocumentBulletListFilled;
                AccessKey = "7";
                break;
            case PageType.Settings:
                DefaultIcon = FluentSymbol.Settings;
                SelectedIcon = FluentSymbol.SettingsFilled;
                AccessKey = "S";
                break;
            case PageType.WebDav:
                DefaultIcon = FluentSymbol.CloudDatabase;
                SelectedIcon = FluentSymbol.CloudDatabaseFilled;
                AccessKey = "W";
                break;
            case PageType.Message:
                DefaultIcon = FluentSymbol.Mail;
                SelectedIcon = FluentSymbol.MailFilled;
                AccessKey = "M";
                break;
            default:
                break;
        }
    }
}
