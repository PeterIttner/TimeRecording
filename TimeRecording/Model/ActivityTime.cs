using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecording.Common;

namespace TimeRecording.Model
{
    public class ActivityTime : ModelBase
    {
        private DateTime mStartTime = DateTime.Now;
        public DateTime StartTime
        {
            get
            {
                return mStartTime;
            }
            set
            {
                mStartTime = value;
                NotifyPropertyChanged("StartTime");
                NotifyPropertyChanged("Duration");
            }
        }

        private DateTime mEndTime = DateTime.Now;
        public DateTime EndTime
        {
            get
            {
                return mEndTime;
            }
            set
            {
                mEndTime = value;
                NotifyPropertyChanged("EndTime");
                NotifyPropertyChanged("Duration");
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return EndTime - StartTime;
            }
        }
    }
}
