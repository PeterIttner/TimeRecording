using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecording.Model;
using TimeRecording.TimeCalculation;

namespace TimeRecording.ViewModel
{
    public class WorkTime
    {
        public DateTime Date { get; set; }
        public TimeSpan WorkingTime { get; set; }
        public string Activities { get; set; }
    }

    public class ProjectDayDetailsViewModel : INotifyPropertyChanged
    {
        #region Member

        WorkingTimeCalculator mCalculator = new WorkingTimeCalculator();

        #endregion

        #region Creation

        public ProjectDayDetailsViewModel(Project project)
        {
            ProjectName = project.Name;
            TotalWorkingTime = FormatDuration(mCalculator.CalculateTotalDuration(project));
            WorkingTimes = new ObservableCollection<WorkTime>(mCalculator.GetWorkingTimesOfProjectPerDay(project));
            TimeBudget = project.TimeBudget.HasValue ? FormatDuration(project.TimeBudget.Value) : "Unbegrenzt";
            ProjectStatus = GetProjectStatus(project);
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

        private string mTotalWorkingTime;
        public string TotalWorkingTime
        {
            get
            {
                return mTotalWorkingTime;
            }
            set
            {
                mTotalWorkingTime = value;
                NotifyPropertyChanged("TotalWorkingTime");
            }
        }

        private string mProjectStatus;
        public string ProjectStatus
        {
            get
            {
                return mProjectStatus;
            }
            set
            {
                mProjectStatus = value;
                NotifyPropertyChanged("ProjectStatus");
            }
        }

        private string mTimeBudget;
        public string TimeBudget
        {
            get
            {
                return mTimeBudget;
            }
            set
            {
                mTimeBudget = value;
                NotifyPropertyChanged("TimeBudget");
            }
        }

        private bool mProjectStatusFlag;
        public bool ProjectStatusFlag
        {
            get
            {
                return mProjectStatusFlag;
            }
            set
            {
                mProjectStatusFlag = value;
                NotifyPropertyChanged("ProjectStatusFlag");
            }
        }


        private ObservableCollection<WorkTime> mWorkingTimes;
        public ObservableCollection<WorkTime> WorkingTimes
        {
            get { return mWorkingTimes; }
            set { mWorkingTimes = value; }
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

        #region Private Helpers

        private string FormatDuration(TimeSpan totalTime)
        {
            var totalManDays = totalTime.TotalDays * 3; // * 24 / 8
            var totalHours = totalTime.TotalHours;
            var totalMinutes = totalTime.TotalMinutes;
            var totalSeconds = totalTime.TotalSeconds;

            return string.Format("{0:0.##} Manntage ≙ {1:0.##} Stunden ≙ {2} Minuten ≙ {3} Sekunden", totalManDays, totalHours, totalMinutes, totalSeconds);
        }

        private string GetProjectStatus(Project project)
        {
            var currentProjectDuration = mCalculator.CalculateTotalDuration(project);
            if (project.TimeBudget.HasValue == false)
            {
                ProjectStatusFlag = true;
                return "Im Zeitplan - Rest Budget: Unbegrenzt";
            }
            else if (project.TimeBudget.Value > currentProjectDuration)
            {
                var remainingHours = (project.TimeBudget.Value - currentProjectDuration).TotalHours;
                ProjectStatusFlag = true;
                return string.Format("Im Zeitplan - Rest Budget: {0:0.##} Stunden", remainingHours);
            }
            else
            {
                var tooMuchHours = (currentProjectDuration - project.TimeBudget.Value).TotalHours;
                ProjectStatusFlag = false;
                return string.Format("Zeit-Budget um {0:0.##} Stunden überzogen", tooMuchHours);
            }
        }

        #endregion
    }
}