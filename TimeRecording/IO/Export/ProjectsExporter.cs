using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TimeRecording.Model;

namespace TimeRecording.IO.Export
{
    public class ProjectsExporter
    {
        private List<Project> mProjects;

        public ProjectsExporter(List<Project> projects)
        {
            mProjects = projects;
        }

        public void Export(string filename)
        {
            var projects = JsonConvert.SerializeObject(mProjects);
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(projects));
            File.WriteAllText(filename, base64Encoded);
        }
    }
}
