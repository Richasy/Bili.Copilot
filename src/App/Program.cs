// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace Bili.Copilot.App;

/// <summary>
/// Single instance mode:
/// https://blogs.windows.com/windowsdeveloper/2022/01/28/making-the-app-single-instanced-part-3/.
/// </summary>
public static class Program
{
    // Note that [STAThread] doesn't work with "async Task Main(string[] args)"
    // https://github.com/dotnet/roslyn/issues/22112
    [STAThread]
    private static void Main(string[] args)
    {
        var actArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        var mainAppInstance = AppInstance.FindOrRegisterForKey(App.Id);
        if (!mainAppInstance.IsCurrent)
        {
            mainAppInstance.RedirectActivationToAsync(actArgs).AsTask().Wait();
            return;
        }

        WinRT.ComWrappersSupport.InitializeComWrappers();

        Application.Start(p =>
        {
            var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
            SynchronizationContext.SetSynchronizationContext(context);
            new App();
        });
    }
}
