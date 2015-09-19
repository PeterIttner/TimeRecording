using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using TimeRecording.Common;
using TimeRecording.Model;

namespace TimeRecording.IO.Repository
{
    public interface IRepository
    {
        void Persist();
        void DeleteProject(Project project);
        Project CreateEmptyProject(string name);
        ObservableCollection<Project> GetProjects();
        bool IsProjectNameValid(string name);
        bool IsProjectExisting(string name);
    }
}
