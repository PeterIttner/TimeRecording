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
    public class RecordDetailsViewModel : INotifyPropertyChanged
    {
        #region Member

        private Activity mActivity;

        #endregion

        #region Creation

        public RecordDetailsViewModel(Activity activity)
        {
            mActivity = activity;
            ActivityDescription = activity.Description;
            ActivityTimes = activity.ActivityTimes;
        }

        #endregion

        #region View Bound Properties

        private string mActivityDescription;
        public string ActivityDescription
        {
            get
            {
                return mActivityDescription;
            }
            set
            {
                mActivityDescription = value;
                NotifyPropertyChanged("ActivityDescription");
            }
        }

        private ObservableCollection<ActivityTime> mActivityTimes;
        public ObservableCollection<ActivityTime> ActivityTimes
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
