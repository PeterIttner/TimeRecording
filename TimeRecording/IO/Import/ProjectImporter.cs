using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecording.Common.Logging;
using TimeRecording.Model;

namespace TimeRecording.IO.Import
{
    public class ProjectsImporter
    {
        private static ILogger Logger = LoggerFactory.CurrentLogger;

        private string mFilename;
        public ProjectsImporter(string filename)
        {
            mFilename = filename;
        }

        public List<Project> Import()
        {
            try
            {
                var projectsBase64 = File.ReadAllText(mFilename, Encoding.UTF8);
                var decodedBytes = Convert.FromBase64String(projectsBase64);
                var projectsJson = Encoding.UTF8.GetString(decodedBytes);
                var projects = JsonConvert.DeserializeObject<List<Project>>(projectsJson);
                return projects;
            }
            catch (Exception ex)
            {
                Logger.Warn("Tried to import invalid projecstfile", ex);
                return new List<Project>();
            }
        }
    }
}
