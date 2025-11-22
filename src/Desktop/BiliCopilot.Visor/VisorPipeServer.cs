using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using BiliCopilot.Visor.Models;

namespace BiliCopilot.Visor;

/// <summary>
/// Visor 命名管道服务器
/// </summary>
public sealed class VisorPipeServer : IDisposable
{
    private const string PipeName = "BiliCopilot.Visor.Pipe";
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly WheelScrollManager _scrollManager;
    private Task? _serverTask;
    private bool _isDisposed;

    public VisorPipeServer(WheelScrollManager scrollManager)
    {
        _scrollManager = scrollManager;
    }

    /// <summary>
    /// 启动管道服务器
    /// </summary>
    public void Start()
    {
        _serverTask = Task.Run(async () => await RunServerAsync(_cancellationTokenSource.Token));
    }

    /// <summary>
    /// 停止管道服务器
    /// </summary>
    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        _serverTask?.Wait(TimeSpan.FromSeconds(5));
    }

    private async Task RunServerAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var pipeServer = new NamedPipeServerStream(
                    PipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Message,
                    PipeOptions.Asynchronous);

                await pipeServer.WaitForConnectionAsync(cancellationToken);

                try
                {
                    var command = await ReadCommandAsync(pipeServer, cancellationToken);
                    if (command != null)
                    {
                        var response = HandleCommand(command);
                        await WriteResponseAsync(pipeServer, response, cancellationToken);
                    }
                }
                finally
                {
                    if (pipeServer.IsConnected)
                    {
                        pipeServer.Disconnect();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pipe server error: {ex.Message}");
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    private static async Task<VisorCommand?> ReadCommandAsync(NamedPipeServerStream pipeServer, CancellationToken cancellationToken)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            var buffer = new byte[4096];
            int bytesRead;

            do
            {
                bytesRead = await pipeServer.ReadAsync(buffer, cancellationToken);
                await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            }
            while (!pipeServer.IsMessageComplete);

            var json = Encoding.UTF8.GetString(memoryStream.ToArray());
            return JsonSerializer.Deserialize(json, JsonGenContext.Default.VisorCommand);
        }
        catch
        {
            return null;
        }
    }

    private static async Task WriteResponseAsync(NamedPipeServerStream pipeServer, VisorResponse response, CancellationToken cancellationToken)
    {
        try
        {
            var json = JsonSerializer.Serialize(response, JsonGenContext.Default.VisorResponse);
            var bytes = Encoding.UTF8.GetBytes(json);
            await pipeServer.WriteAsync(bytes, cancellationToken);
            await pipeServer.FlushAsync(cancellationToken);
        }
        catch
        {
            // Ignore write errors
        }
    }

    private VisorResponse HandleCommand(VisorCommand command)
    {
        try
        {
            switch (command.Type)
            {
                case VisorCommandType.Initialize:
                    if (command.OriginalSpeed.HasValue && command.ExpectedSpeed.HasValue)
                    {
                        _scrollManager.Initialize(command.OriginalSpeed.Value, command.ExpectedSpeed.Value);
                        _scrollManager.IsEnabled = command.Enabled ?? true;
                        return new VisorResponse { Success = true, Message = "Initialized successfully" };
                    }
                    return new VisorResponse { Success = false, Message = "Missing speed parameters" };

                case VisorCommandType.UpdateScrollSpeed:
                    if (command.OriginalSpeed.HasValue && command.ExpectedSpeed.HasValue)
                    {
                        _scrollManager.UpdateSpeed(command.OriginalSpeed.Value, command.ExpectedSpeed.Value);
                        return new VisorResponse { Success = true, Message = "Speed updated successfully" };
                    }
                    return new VisorResponse { Success = false, Message = "Missing speed parameters" };

                case VisorCommandType.EnableScrollAccelerate:
                    _scrollManager.IsEnabled = true;
                    return new VisorResponse { Success = true, Message = "Scroll accelerate enabled" };

                case VisorCommandType.DisableScrollAccelerate:
                    _scrollManager.IsEnabled = false;
                    return new VisorResponse { Success = true, Message = "Scroll accelerate disabled" };

                case VisorCommandType.Shutdown:
#pragma warning disable VSTHRD110 // Observe result of async calls
                    Task.Run(() =>
                    {
                        Thread.Sleep(100);
                        Environment.Exit(0);
                    });
#pragma warning restore VSTHRD110 // Observe result of async calls
                    return new VisorResponse { Success = true, Message = "Shutting down" };

                default:
                    return new VisorResponse { Success = false, Message = "Unknown command" };
            }
        }
        catch (Exception ex)
        {
            return new VisorResponse { Success = false, Message = $"Error: {ex.Message}" };
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        Stop();
        _cancellationTokenSource?.Dispose();
    }
}
