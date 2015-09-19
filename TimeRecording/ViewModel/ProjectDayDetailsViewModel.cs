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

            var splittedWorkingTimes = new List<WorkTime>();
            foreach (var activity in project.Activities)
            {
                foreach (var activityTime in activity.ActivityTimes)
                {
                    splittedWorkingTimes.AddRange(SplitIntoDays(activityTime, activity.Description));
                }
            }
            var aggregatedWorkingTimes = AggregateDays(splittedWorkingTimes);

            WorkingTimes = new ObservableCollection<WorkTime>(aggregatedWorkingTimes.OrderBy(workingTime => workingTime.Date).ThenBy(workingTime => workingTime.WorkingTime).ToList());
        }

        private List<WorkTime> AggregateDays(List<WorkTime> workingTimes)
        {
            var aggregatedWorkingTimes = new List<WorkTime>();
            var groupedByDay = workingTimes.GroupBy(worktime => worktime.Date);
            foreach (var dayGroup in groupedByDay)
            {
                var day = dayGroup.Key;
                var description = string.Join(", ", dayGroup.Select(d => d.Activities).Distinct().ToList());
                var workingTimeThatDay = new TimeSpan(0);

                foreach (var partialWorkingTime in dayGroup)
                {
                    workingTimeThatDay += partialWorkingTime.WorkingTime;
                }
                var aggregatedByOneDay = new WorkTime { Date = day, WorkingTime = workingTimeThatDay, Activities = description };
                aggregatedWorkingTimes.Add(aggregatedByOneDay);
            }

            return aggregatedWorkingTimes;
        }

        private List<WorkTime> SplitIntoDays(ActivityTime activityTime, String description)
        {
            var splittedTimes = new List<WorkTime>();
            var dayDifference = (activityTime.EndTime.Date - activityTime.StartTime.Date).TotalDays;

            if (dayDifference == 0)
            {
                splittedTimes.Add(new WorkTime { Date = activityTime.StartTime.Date, WorkingTime = activityTime.Duration, Activities = description });
            }
            else if (dayDifference == 1)
            {
                var workTimeDay1 = activityTime.EndTime.Date - activityTime.StartTime;
                var workTimeDay2 = activityTime.EndTime - activityTime.EndTime.Date;
                splittedTimes.Add(new WorkTime { Date = activityTime.StartTime.Date, WorkingTime = workTimeDay1, Activities = description });
                splittedTimes.Add(new WorkTime { Date = activityTime.EndTime.Date, WorkingTime = workTimeDay2, Activities = description });
            }
            else
            {
                var workTimeFirstDay = activityTime.StartTime.AddDays(1).Date - activityTime.StartTime;
                splittedTimes.Add(new WorkTime { Date = activityTime.StartTime.Date, WorkingTime = workTimeFirstDay, Activities = description });

                // TODO: check if the substracted amount is correct - Maybe we can merge this branch with the == 1 branch, to reduce the amount of code
                for (int dayCount = 1; dayCount < dayDifference - 2; dayCount++)
                {
                    var morning = activityTime.StartTime.Date.AddDays(dayCount).Date;
                    var midnight = activityTime.StartTime.Date.AddDays(dayCount + 1).Date;
                    var workingTime = midnight - morning;
                    splittedTimes.Add(new WorkTime { Date = morning.Date, WorkingTime = workingTime, Activities = description });
                }

                var workTimeLastDay = activityTime.EndTime - activityTime.EndTime.Date;
                splittedTimes.Add(new WorkTime { Date = activityTime.EndTime.Date, WorkingTime = workTimeLastDay, Activities = description });
            }
            return splittedTimes;
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