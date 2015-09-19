using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecording.Model;

namespace TimeRecording.TimeCalculation
{
    public class WorkingTimeCalculator
    {
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
    }
}
