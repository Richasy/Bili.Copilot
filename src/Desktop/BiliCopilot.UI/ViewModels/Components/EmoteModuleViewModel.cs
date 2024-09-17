// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Comment;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 表情模块视图模型.
/// </summary>
public sealed partial class EmoteModuleViewModel : ViewModelBase
{
    private readonly ICommentService _service;
    private readonly ILogger<EmoteModuleViewModel> _logger;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private List<EmotePackage>? _packages;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmoteModuleViewModel"/> class.
    /// </summary>
    public EmoteModuleViewModel(
        ICommentService service,
        ILogger<EmoteModuleViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (IsLoading || Packages is not null)
        {
            return;
        }

        try
        {
            IsLoading = true;
            Packages = [.. await _service.GetEmotePackagesAsync()];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取表情包时出现错误.");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
