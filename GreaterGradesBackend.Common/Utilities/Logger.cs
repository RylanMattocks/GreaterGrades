using Microsoft.Extensions.Logging;

namespace GreaterGradesBackend.Common.Utilities
{
    public class Logger : ILogger
    {
        private readonly ILogger _logger;

        public Logger(ILogger<Logger> logger)
        {
            _logger = logger;
        }

        public IDisposable BeginScope<TState>(TState state) => _logger.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
