using System.Diagnostics;
using Richasy.BiliKernel;
using Richasy.BiliKernel.Authorizers.TV;
using Richasy.BiliKernel.Bili.Authorization;
using Spectre.Console;

namespace Bili.Console;

internal sealed class AuthorizeModule : IFeatureModule
{
    private readonly Kernel _kernel;
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;

    public AuthorizeModule(
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
        var isSignedIn = await CheckAuthorizeStatusAsync().ConfigureAwait(false);
        var lastTip = string.Empty;
        if (isSignedIn)
        {
            var shouldSignOut = AnsiConsole.Confirm("已登录，是否注销?", false);
            if (shouldSignOut)
            {
                await _kernel.GetRequiredService<IAuthenticationService>().SignOutAsync().ConfigureAwait(false);
                lastTip = "[bold green]注销成功[/]";
            }
        }
        else
        {
            var shouldSignIn = AnsiConsole.Confirm("您还没有登录，是否登录?", true);
            if (shouldSignIn)
            {
                AnsiConsole.MarkupLine("[bold yellow]请在弹出的窗口中扫码[/]");
                var service = _kernel.GetRequiredService<IAuthenticationService>(nameof(TVAuthenticationService));
                await service.SignInAsync(cancellationToken: _cancellationToken).ConfigureAwait(false);
                isSignedIn = await CheckAuthorizeStatusAsync().ConfigureAwait(false);
                lastTip = isSignedIn ? "[bold green]登录成功[/]" : "[bold red]登录失败[/]";
            }
        }

        await _backFunc(lastTip).ConfigureAwait(false);
    }

    private async Task<bool> CheckAuthorizeStatusAsync()
    {
        var tokenResolver = _kernel.GetRequiredService<IAuthenticationService>();
        try
        {
            await tokenResolver.EnsureTokenAsync(_cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        return false;
    }
}
