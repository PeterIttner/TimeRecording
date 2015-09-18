using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecording.Common;

namespace TimeRecording.Model
{
    public class Project : ModelBase
    {
        private DateTime mCreationTime;
        public DateTime CreationTime
        {
            get
            {
                return mCreationTime;
            }
            set
            {
                mCreationTime = value;
                NotifyPropertyChanged("CreationTime");
            }
        }

        private string mName;
        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
                NotifyPropertyChanged("Name");
            }
        }

        private ObservableCollection<Activity> mActivities = new ObservableCollection<Activity>();
        public ObservableCollection<Activity> Activities
        {
            get
            {
                return mActivities;
            }
            set
            {
                mActivities = value;
            }
        }
    }
}
