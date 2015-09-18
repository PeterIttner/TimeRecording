using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeRecording.Common.Logging
{
    public interface ILogger
    {
        void Info(string message);
        void Info(string message, params string[] parameter);

        void Fatal(Exception e);
        void Fatal(string message);
        void Fatal(string message, Exception e);
        void Fatal(string message, params string[] parameter);

        void Warn(Exception e);
        void Warn(string message);
        void Warn(string message, Exception e);
        void Warn(string message, params string[] parameter);
    }
}
