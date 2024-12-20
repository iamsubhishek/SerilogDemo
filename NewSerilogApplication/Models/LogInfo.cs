using NewSerilogApplication.Interface;
using System;
using System.Runtime.CompilerServices;
using Serilog;

namespace NewSerilogApplication.Models
{
    public class LogInfo<T> : ILog<T>
    {
        private ILogger _logger;

        public LogInfo()
        {
            _logger = Log.ForContext<T>();
        }
        public void TraceError(Exception ex, [CallerMemberName] string memberName = "")
        {
            _logger.Error(ex, string.Empty);
            //Log.ForContext<T>().Error(ex, string.Empty);
            //logger.LogError(ex, string.Empty);
        }

        public void TraceMessage([CallerMemberName] string memberName = "")
        {
            _logger.Information($"Started executing {memberName} member");
            //Log.ForContext<T>().Information($"Started executing {memberName} member");
            //logger.LogInformation($"Started executing {memberName} member");
        }
    }
}
