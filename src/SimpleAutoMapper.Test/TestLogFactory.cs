using Microsoft.Extensions.Logging;

namespace SimpleAutoMapper.Test;

internal static class TestLogFactory
{
    private static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    {
        builder
            .AddFilter("Microsoft", LogLevel.Warning)
            .AddFilter("System", LogLevel.Warning)
            .AddFilter("SimpleAutoMapper", LogLevel.Debug)
            .AddConsole()
            .AddDebug();
    });

    public static ILogger<T> CreateLog<T>() => loggerFactory.CreateLogger<T>();
}
