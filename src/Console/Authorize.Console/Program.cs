using System.Text;
using Authorize.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.OutputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);
builder.Environment.ContentRootPath = AppDomain.CurrentDomain.BaseDirectory;

builder.Services.AddHostedService<AuthorizeService>();
using var host = builder.Build();
await host.RunAsync().ConfigureAwait(false);
