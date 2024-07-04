
using System.Diagnostics;
using Richasy.BiliKernel;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;
using Spectre.Console;

namespace Bili.Console;

internal sealed class MyProfileModule : IFeatureModule
{
    private readonly Kernel _kernel;
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;

    private UserDetailProfile? _myInfo;

    public MyProfileModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _kernel = kernel;
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
    }

    public async Task RunAsync()
    {
        var profileService = _kernel.GetRequiredService<IUserProfileService>();

        try
        {
            await DisplayUserInformationAsync().ConfigureAwait(false);

            if (AnsiConsole.Confirm("是否返回？"))
            {
                await _backFunc(string.Empty).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await _backFunc("[bold red]用户数据模块出现异常[/]").ConfigureAwait(false);
        }
    }

    public void Exit()
    {
        // Do nothing.
    }

    private async Task DisplayUserInformationAsync()
    {
        if (_myInfo == null)
        {
            var profileService = _kernel.GetRequiredService<IUserProfileService>();
            _myInfo = await profileService.GetMyProfileAsync(default, _cancellationToken).ConfigureAwait(false);
        }

        AnsiConsole.MarkupLine($"你好，[bold yellow]{_myInfo.User.Name}[/]");
    }
}
