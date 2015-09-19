using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Shapes;
using TimeRecording.Common;
using TimeRecording.Model;

namespace TimeRecording.ViewModel
{
    public class AdvancedActivityTime
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
    }

    public class ProjectDetailsViewModel : INotifyPropertyChanged
    {
        #region Member

        private Project mProject;

        #endregion

        #region Creation

        public ProjectDetailsViewModel(Project project)
        {
            mProject = project;
            ProjectName = project.Name;
            var activityTimes = new ObservableCollection<AdvancedActivityTime>();
            foreach(var activity in project.Activities) {
                foreach (var activityTime in activity.ActivityTimes)
                {
                    var time = new AdvancedActivityTime { StartTime = activityTime.StartTime, EndTime = activityTime.EndTime, Duration = activityTime.Duration, Description = activity.Description };
                    activityTimes.Add(time);
                }
            }
            ActivityTimes = new ObservableCollection<AdvancedActivityTime>(activityTimes.OrderBy(time => time.StartTime).ThenBy(time => time.Duration));
        }

        #endregion

        #region View Bound Properties

        private string mProjectName;
        public string ProjectName
        {
            get
            {
                return mProjectName;
            }
            set
            {
                mProjectName = value;
                NotifyPropertyChanged("ProjectName");
            }
        }

        private ObservableCollection<AdvancedActivityTime> mActivityTimes;
        public ObservableCollection<AdvancedActivityTime> ActivityTimes
        {
            get { return mActivityTimes; }
            set { mActivityTimes = value; }
        }


        #endregion

        #region Common

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

    }
}
