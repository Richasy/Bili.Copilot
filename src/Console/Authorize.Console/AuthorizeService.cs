using Microsoft.Extensions.Hosting;
using Richasy.BiliKernel;
using Richasy.BiliKernel.Authorizers.TV;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models.Authorization;
using Spectre.Console;

namespace Authorize.Console;

/// <summary>
/// 授权服务.
/// </summary>
public sealed class AuthorizeService : IHostedService
{
    private readonly Kernel _kernel;

    /// <summary>
    /// 初始化 <see cref="AuthorizeService"/> 类的新实例.
    /// </summary>
    public AuthorizeService()
    {
        _kernel = Kernel.CreateBuilder()
            .AddNativeTokenResolver()
            .AddNativeQRCodeResolver()
            .AddNativeCookiesResolver()
            .AddHttpClient()
            .AddBasicAuthenticator()
            .AddTVAuthentication()
            .Build();
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var confirm = AnsiConsole.Confirm("是否需要授权?");
            var tokenResolver = _kernel.GetRequiredService<IBiliTokenResolver>();
            if (confirm)
            {
                AnsiConsole.MarkupLine("[bold yellow]请在弹出的窗口中扫码[/]");
                var service = _kernel.GetRequiredService<IAuthenticationService>(nameof(TVAuthenticationService));
                await service.SignInAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                var token = tokenResolver.GetToken();
                if (token != null)
                {
                    DisplayLocalToken(token);
                    await AskSignOutAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    AnsiConsole.MarkupLine("[bold red]授权失败[/]");
                }
            }
            else
            {
                var localToken = tokenResolver.GetToken();
                if (localToken != null)
                {
                    AnsiConsole.MarkupLine("[bold yellow]检测到已授权的 Token[/]");
                    DisplayLocalToken(localToken);
                    await AskSignOutAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    AnsiConsole.MarkupLine("[bold red]未授权[/]");
                    Environment.Exit(0);
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            Environment.Exit(1);
        }
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void DisplayLocalToken(BiliToken token)
    {
        AnsiConsole.MarkupLine("[bold green]授权成功[/]");
        AnsiConsole.MarkupLine($"[bold green]Token: {token.AccessToken}[/]");
        AnsiConsole.MarkupLine($"[bold yellow]过期时间: {DateTimeOffset.FromUnixTimeSeconds(token.ExpiresIn)}[/]");
        AnsiConsole.MarkupLine($"[bold blue]当前用户 ID: {token.UserId}[/]");

        var cookiesResolver = _kernel.GetRequiredService<IBiliCookiesResolver>();
        var cookies = cookiesResolver.GetCookieString();
        if (!string.IsNullOrEmpty(cookies))
        {
            AnsiConsole.MarkupLine($"[bold white]Cookies: {cookies}[/]");
        }
    }

    private async Task AskSignOutAsync(CancellationToken cancellationToken)
    {
        var shouldSignOut = AnsiConsole.Confirm("是否退出账户?", true);
        if (shouldSignOut)
        {
            await _kernel.GetRequiredService<IAuthenticationService>(nameof(TVAuthenticationService))
                .SignOutAsync(cancellationToken).ConfigureAwait(false);
            Environment.Exit(0);
        }
    }
}
