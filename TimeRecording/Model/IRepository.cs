using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using TimeRecording.Common;
using TimeRecording.Model;

namespace TimeRecording.Model
{
    public interface IRepository
    {
        void Persist();
        Project CreateEmptyProject(string name);
        ObservableCollection<Project> GetProjects();
        bool IsProjectNameValid(string name);
    }
}
