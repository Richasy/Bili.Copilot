using Bili.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
var builder = Host.CreateApplicationBuilder(args);
builder.Environment.ContentRootPath = AppDomain.CurrentDomain.BaseDirectory;

builder.Services.AddHostedService<BiliService>();
using var host = builder.Build();
await host.RunAsync().ConfigureAwait(false);
