// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.Visor.Models;
using System.IO.Pipes;
using System.Text.Json;

namespace BiliCopilot.UI.Extensions;

/// <summary>
/// Visor 客户端，用于与 Visor 进程通信
/// </summary>
public sealed class VisorClient
{
    private const string PipeName = "BiliCopilot.Visor.Pipe";
    private const int TimeoutMs = 3000;

    /// <summary>
    /// 发送命令到 Visor
    /// </summary>
    public static async Task<VisorResponse?> SendCommandAsync(VisorCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var pipeClient = new NamedPipeClientStream(
                ".",
                PipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeoutMs);

            await pipeClient.ConnectAsync(cts.Token);

            // 发送命令
            var commandJson = JsonSerializer.Serialize(command, GlobalSerializeContext.Default.VisorCommand);
            var commandBytes = Encoding.UTF8.GetBytes(commandJson);
            await pipeClient.WriteAsync(commandBytes, cts.Token);
            await pipeClient.FlushAsync(cts.Token);

            // 读取响应 - 读取所有可用数据
            await using var memoryStream = new MemoryStream();
            var buffer = new byte[4096];

            // 第一次读取
            int bytesRead = await pipeClient.ReadAsync(buffer, cts.Token);
            if (bytesRead > 0)
            {
                await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead), cts.Token);
            }

            var responseJson = Encoding.UTF8.GetString(memoryStream.ToArray());
            return JsonSerializer.Deserialize(responseJson, GlobalSerializeContext.Default.VisorResponse);
        }
        catch (TimeoutException)
        {
            return new VisorResponse { Success = false, Message = "Connection timeout" };
        }
        catch (Exception ex)
        {
            return new VisorResponse { Success = false, Message = $"Error: {ex.Message}" };
        }
    }

    /// <summary>
    /// 初始化 Visor
    /// </summary>
    public static Task<VisorResponse?> InitializeAsync(int originalSpeed, int expectedSpeed, bool enabled = true)
    {
        var command = new VisorCommand
        {
            Type = VisorCommandType.Initialize,
            OriginalSpeed = originalSpeed,
            ExpectedSpeed = expectedSpeed,
            Enabled = enabled,
        };
        return SendCommandAsync(command);
    }

    /// <summary>
    /// 更新滚动速度
    /// </summary>
    public static Task<VisorResponse?> UpdateScrollSpeedAsync(int originalSpeed, int expectedSpeed)
    {
        var command = new VisorCommand
        {
            Type = VisorCommandType.UpdateScrollSpeed,
            OriginalSpeed = originalSpeed,
            ExpectedSpeed = expectedSpeed,
        };
        return SendCommandAsync(command);
    }

    /// <summary>
    /// 启用滚动加速
    /// </summary>
    public static Task<VisorResponse?> EnableScrollAccelerateAsync()
    {
        var command = new VisorCommand
        {
            Type = VisorCommandType.EnableScrollAccelerate,
        };
        return SendCommandAsync(command);
    }

    /// <summary>
    /// 禁用滚动加速
    /// </summary>
    public static Task<VisorResponse?> DisableScrollAccelerateAsync()
    {
        var command = new VisorCommand
        {
            Type = VisorCommandType.DisableScrollAccelerate,
        };
        return SendCommandAsync(command);
    }

    /// <summary>
    /// 关闭 Visor 服务
    /// </summary>
    public static Task<VisorResponse?> ShutdownAsync()
    {
        var command = new VisorCommand
        {
            Type = VisorCommandType.Shutdown,
        };
        return SendCommandAsync(command);
    }
}
