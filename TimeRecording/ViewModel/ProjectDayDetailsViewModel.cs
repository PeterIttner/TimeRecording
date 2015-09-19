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
            TotalWorkingTime = FormatTotalDuration(mCalculator.CalculateTotalDuration(project));
            WorkingTimes = new ObservableCollection<WorkTime>(mCalculator.GetWorkingTimesOfProjectPerDay(project));
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

        private string FormatTotalDuration(TimeSpan totalTime)
        {
            var totalManDays = totalTime.TotalDays * 3; // * 24 / 8
            var totalHours = totalTime.TotalHours;
            var totalMinutes = totalTime.TotalMinutes;
            var totalSeconds = totalTime.TotalSeconds;

            return string.Format("{0:0.00000} Manntage ≙ {1:0.00} Stunden ≙ {2:0.00} Minuten ≙ {3:0.00} Sekunden", totalManDays, totalHours, totalMinutes, totalSeconds);
        }

        #endregion
    }
}