using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecording.Model;
using TimeRecording.ViewModel;

namespace TimeRecording.TimeCalculation
{
    public class WorkingTimeCalculator
    {
        #region Public Interface
        
        public TimeSpan CalculateDuration(Activity activity)
        {
            TimeSpan totalTime = new TimeSpan(0);
            foreach (var activeTime in activity.ActivityTimes)
            {
                totalTime += activeTime.Duration;
            }
            return totalTime;
        }

        public TimeSpan CalculateTotalDuration(Project project)
        {
            TimeSpan totalTime = new TimeSpan(0);
            if (project != null)
            {
                foreach (var activity in project.Activities)
                {
                    totalTime += activity.Duration;
                }
            }
            return totalTime;
        }

        public List<WorkTime> GetWorkingTimesOfProjectPerDay(Project project)
        {
            var splittedWorkingTimes = new List<WorkTime>();
            foreach (var activity in project.Activities)
            {
                foreach (var activityTime in activity.ActivityTimes)
                {
                    splittedWorkingTimes.AddRange(SplitIntoDays(activityTime, activity.Description));
                }
            }
            var aggregatedWorkingTimes = AggregateDays(splittedWorkingTimes);

            return aggregatedWorkingTimes.OrderBy(workingTime => workingTime.Date).ThenBy(workingTime => workingTime.WorkingTime).ToList();
        }

        #endregion

        #region Private Helpers

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
    }
}
