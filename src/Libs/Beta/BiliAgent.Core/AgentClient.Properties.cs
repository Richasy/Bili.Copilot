// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using Microsoft.Extensions.Logging;

namespace BiliAgent.Core;

/// <summary>
/// 助理客户端.
/// </summary>
public sealed partial class AgentClient
{
    private readonly IAgentProviderFactory _providerFactory;
    private readonly ILogger<AgentClient> _logger;
    private bool _disposedValue;
}
