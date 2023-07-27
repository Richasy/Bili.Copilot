// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Models.Data.Player;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 互动视频模块视图模型.
/// </summary>
public sealed partial class InteractionModuleViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionModuleViewModel"/> class.
    /// </summary>
    public InteractionModuleViewModel()
    {
        Choices = new ObservableCollection<InteractionInformation>();
        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand);
        AttachExceptionHandlerToAsyncCommand(LogException, ReloadCommand);
    }

    /// <summary>
    /// 设置数据.
    /// </summary>
    /// <param name="partId">分Id.</param>
    /// <param name="choiceId">选项Id.</param>
    /// <param name="graphVersion">版本.</param>
    public void SetData(string partId, string choiceId, string graphVersion)
    {
        _partId = partId;
        _choiceId = string.IsNullOrEmpty(choiceId) ? "0" : choiceId;
        _graphVersion = graphVersion;
        ReloadCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private void Reset() => TryClear(Choices);

    [RelayCommand]
    private async Task ReloadAsync()
    {
        if (IsReloading)
        {
            return;
        }

        Reset();
        var infos = await PlayerProvider.GetInteractionInformationsAsync(_partId, _graphVersion, _choiceId);
        infos?.Where(p => p.IsValid).ToList().ForEach(Choices.Add);

        if (Choices.Count == 0)
        {
            NoMoreChoices?.Invoke(this, EventArgs.Empty);
        }
    }
}
