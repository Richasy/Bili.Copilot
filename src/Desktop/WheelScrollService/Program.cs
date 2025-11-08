namespace WheelScrollService;

internal static class Program
{
    private const string MutexName = "BiliCopilot.WheelScrollService";
#pragma warning disable CA1303 // 请不要将文本作为本地化参数传递
    static void Main(string[] args)
    {
        // 解析命令行参数
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: WheelScrollService <originalSpeed> <expectedSpeed>");
            Console.WriteLine("Example: WheelScrollService 3 12");
            return;
        }

        if (!int.TryParse(args[0], out var originalSpeed) || !int.TryParse(args[1], out var expectedSpeed))
        {
            Console.WriteLine("Error: Invalid arguments. Both originalSpeed and expectedSpeed must be integers.");
            return;
        }

        // 确保只有一个实例运行
        using var singleInstance = new SingleInstanceLock(MutexName);
        if (!singleInstance.IsFirstInstance)
        {
            Console.WriteLine("Another instance is already running. Exiting.");
            return;
        }

        Console.WriteLine($"WheelScrollService started.");
        Console.WriteLine($"Original Speed: {originalSpeed}");
        Console.WriteLine($"Expected Speed: {expectedSpeed}");

        // 启动服务
        var manager = new WheelScrollManager(originalSpeed, expectedSpeed);

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
    }
}
#pragma warning restore CA1303 // 请不要将文本作为本地化参数传递