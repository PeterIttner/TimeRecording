using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecording.Common.Logging;

namespace TimeRecording.Model
{
    public class FileRepository : IRepository
    {
        private static ILogger Logger = LoggerFactory.CurrentLogger;

        private const string EXTENSION = "work";

        private string mStoragePath;
        private ObservableCollection<Project> mProjects;

        public FileRepository()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);
            mStoragePath = Path.Combine(appDataPath, Properties.Settings.Default.StorageFolder);
            if (!Directory.Exists(mStoragePath))
            {
                Directory.CreateDirectory(mStoragePath);
            }
            mProjects = ReadProjects();
        }

        #region Implementation of Repository

        public Project CreateEmptyProject(string name)
        {
            var project = new Project { Name = name };
            mProjects.Add(project);
            Persist();
            return project;
        }

        public ObservableCollection<Project> GetProjects()
        {
            return mProjects;
        }

        public void Persist()
        {
            foreach (var project in mProjects)
            {
                PersistProject(project);
            }
        }

        public bool IsProjectNameValid(string name)
        {
            var dummyProject = new Project { Name = name };
            if(name == null || name.Length < 2 || name.Any(c => Path.GetInvalidFileNameChars().Contains(c))) {
                return false;
            }
            if(File.Exists(GetFilenameForProject(dummyProject))) {
                return false;
            }
            return true;        
        }

        #endregion

        #region Private Helpers

        private void PersistProject(Project project)
        {
            var projectFile = GetFilenameForProject(project);
            if (!File.Exists(projectFile))
            {
                File.Create(projectFile).Close();
            }
            var serializedProject = JsonConvert.SerializeObject(project, Formatting.Indented);
            File.WriteAllText(projectFile, serializedProject);
        }

        private string GetFilenameForProject(Project project)
        {
            var filename = project.Name + "." + EXTENSION;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c, '_');
            }

            var projectFile = mStoragePath + "\\" + filename;
            return projectFile;
        }

        private ObservableCollection<Project> ReadProjects()
        {
            var projects = new ObservableCollection<Project>();
            var files = Directory.GetFiles(mStoragePath, "*." + EXTENSION);
            foreach (var file in files)
            {
                try
                {
                    var projectContent = File.ReadAllText(file);
                    var project = JsonConvert.DeserializeObject<Project>(projectContent);
                    projects.Add(project);
                }
                catch (Exception e)
                {
                    Logger.Warn("The following project file had an incompatible content: " + file, e);
                }
            }
            return projects;
        }

        #endregion
    }
}
