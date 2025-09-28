using BiliCopilot.UI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

public sealed partial class PlaylistMediaViewModel : ViewModelBase<MediaSnapshot>
{
    private readonly Action<PlaylistMediaViewModel> _activeAction;

    public PlaylistMediaViewModel(MediaSnapshot data, Action<PlaylistMediaViewModel> activeAction) : base(data)
    {
        _activeAction = activeAction;
        if (data.Type == Models.Constants.BiliMediaType.Video)
        {
            Name = data.Video.Identifier.Title;
        }
    }

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial bool IsPlaying { get; set; }

    [ObservableProperty]
    public partial bool IsPlayed { get; set; }

    [RelayCommand]
    private void Active()
        => _activeAction.Invoke(this);
}
