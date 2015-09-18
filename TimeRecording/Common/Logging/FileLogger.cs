using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeRecording.Common.Logging
{
    public class FileLogger : ILogger
    {
        private const double MAX_LOGFILE_SIZE_MB = 1.0;
        private const string FILENAME = "log";
        private const string EXTENSION = "txt";

        #region ILogger Implementation

        public void Info(string message)
        {
            var text = string.Format("{0}{1}", GetPrefix(Severity.Info), message);
            WriteToFile(text);
        }

        public void Info(string message, params string[] parameter)
        {
            var messageWithParams = string.Format(message, parameter);
            var text = string.Format("{0}{1}", GetPrefix(Severity.Info), messageWithParams);
            WriteToFile(text);
        }

        public void Fatal(Exception e)
        {
            var text = string.Format("{0}{1}:{2}", GetPrefix(Severity.Fatal), e.Message, e.StackTrace);
            WriteToFile(text);
        }

        public void Fatal(string message)
        {
            var text = string.Format("{0}{1}", GetPrefix(Severity.Fatal), message);
            WriteToFile(text);
        }

        public void Fatal(string message, Exception e)
        {
            var text = string.Format("{0}{1}:{2}:{3}", GetPrefix(Severity.Fatal), message, e.Message, e.StackTrace);
            WriteToFile(text);
        }

        public void Fatal(string message, params string[] parameter)
        {
            var messageWithParams = string.Format(message, parameter);
            var text = string.Format("{0}{1}", GetPrefix(Severity.Fatal), messageWithParams);
            WriteToFile(text);
        }

        public void Warn(Exception e)
        {
            var text = string.Format("{0}{1}:{2}", GetPrefix(Severity.Warn), e.Message, e.StackTrace);
            WriteToFile(text);
        }

        public void Warn(string message)
        {
            var text = string.Format("{0}{1}", GetPrefix(Severity.Warn), message);
            WriteToFile(text);
        }

        public void Warn(string message, Exception e)
        {
            var text = string.Format("{0}{1}:{2}:{3}", GetPrefix(Severity.Warn), message, e.Message, e.StackTrace);
            WriteToFile(text);
        }

        public void Warn(string message, params string[] parameter)
        {
            var messageWithParams = string.Format(message, parameter);
            var text = string.Format("{0}{1}", GetPrefix(Severity.Warn), messageWithParams);
            WriteToFile(text);
        }

        #endregion

        #region Private Helpers

        private string GetPrefix(Severity severity)
        {
            var time = DateTime.Now;
            return string.Format("{0,-6} {1} {2} ~> ", "[" + severity.ToString() + "]", time.ToShortDateString(), time.ToShortTimeString());
        }

        private void EnsureMaximumFileSize(string assemblyDirectory, string currentLogfile)
        {
            var fileinfo = new FileInfo(currentLogfile);
            var kilobytes = fileinfo.Length / 1024.0;
            var megabytes = kilobytes / 1024.0;
            if (megabytes > MAX_LOGFILE_SIZE_MB)
            {
                string backupLogfile = null;
                int appendix = 1;
                do
                {
                    var possibleLogfile = Path.Combine(assemblyDirectory, string.Format("{0}_{1:0000}.{2}", FILENAME, appendix, EXTENSION));
                    if (!File.Exists(possibleLogfile))
                    {
                         backupLogfile = possibleLogfile;
                    }
                    appendix++;
                } while (backupLogfile == null);
                File.Move(currentLogfile, backupLogfile);
                File.Create(currentLogfile).Close();
            }
        }

        private void WriteToFile(string text)
        {
            var assemblyDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var currentLogfile = Path.Combine(assemblyDirectory, FILENAME + "." + EXTENSION);

            if (!File.Exists(currentLogfile))
            {
                File.Create(currentLogfile).Close();
            }
            else
            {
                EnsureMaximumFileSize(assemblyDirectory, currentLogfile);
            }
            lock (this)
            {
                text += Environment.NewLine;
                File.AppendAllText(currentLogfile, text);
            }
        }

        #endregion   
    }

    public enum Severity
    {
        Info,
        Warn,
        Fatal
    }

    
}
