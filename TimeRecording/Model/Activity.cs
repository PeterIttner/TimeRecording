using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecording.Common;

namespace TimeRecording.Model
{
    public class Activity : ModelBase
    {
        private ObservableCollection<ActivityTime> mActivityTimes = new ObservableCollection<ActivityTime>();
        public ObservableCollection<ActivityTime> ActivityTimes
        {
            get
            {
                return mActivityTimes;
            }
            set
            {
                mActivityTimes = value;
            }
        }

        private DateTime mLatestWork;
        public DateTime LatestWork
        {
            get
            {
                return mLatestWork;
            }
            set
            {
                mLatestWork = value;
                NotifyPropertyChanged("LatestWork");
            }
        }


        private string mDescription;
        public string Description
        {
            get { return mDescription; }
            set
            {
                mDescription = value;
                NotifyPropertyChanged("Description");
            }
        }

        private TimeSpan mDuration;
        public TimeSpan Duration
        {
            get
            {
                return mDuration;
            }
            set
            {
                mDuration = value;
                NotifyPropertyChanged("Duration");
            }
        }
    }
}
