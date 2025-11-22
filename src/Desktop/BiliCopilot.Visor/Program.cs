namespace BiliCopilot.Visor;

internal static class Program
{
    private const string MutexName = "BiliCopilot.Visor";
#pragma warning disable CA1303 // 请不要将文本作为本地化参数传递
    static void Main(string[] args)
    {
        // 确保只有一个实例运行
        using var singleInstance = new SingleInstanceLock(MutexName);
        if (!singleInstance.IsFirstInstance)
        {
            Console.WriteLine("Another instance is already running. Exiting.");
            return;
        }

        Console.WriteLine($"BiliCopilot.Visor started.");

        // 创建滚动管理器
        var manager = new WheelScrollManager();

        // 如果有命令行参数，使用旧的初始化方式（向后兼容）
        if (args.Length >= 2)
        {
            if (int.TryParse(args[0], out var originalSpeed) && int.TryParse(args[1], out var expectedSpeed))
            {
                Console.WriteLine($"Legacy mode - Original Speed: {originalSpeed}, Expected Speed: {expectedSpeed}");
                manager.Initialize(originalSpeed, expectedSpeed);
            }
        }

        // 启动管道服务器
        using var pipeServer = new VisorPipeServer(manager);
        pipeServer.Start();
        Console.WriteLine("Pipe server started. Listening for commands...");

        // 保持运行，直到进程被终止或自动退出
        var exitEvent = new ManualResetEvent(false);
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            exitEvent.Set();
        };

        Console.WriteLine("Service is running. Press Ctrl+C to stop.");
        exitEvent.WaitOne();

        Console.WriteLine("Service stopped.");
        pipeServer.Stop();
        manager.Dispose();
    }
#pragma warning restore CA1303 // 请不要将文本作为本地化参数传递
}
