using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeRecording.Common.Logging
{
    public class LoggerFactory
    {
        private static ILogger LoggerInstance;
        public static ILogger CurrentLogger
        {
            get
            {
                if(LoggerInstance == null) {
                    LoggerInstance = new FileLogger();
                }
                return LoggerInstance;
            }
        }
    }
}
