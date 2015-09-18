using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using TimeRecording.Common;
using TimeRecording.Common.Logging;
using TimeRecording.Common.Navigation;
using TimeRecording.Model;
using TimeRecording.View;

namespace TimeRecording.ViewModel
{
    public class RecordViewModel : INotifyPropertyChanged
    {
        #region Member

        private ILogger Logger = LoggerFactory.CurrentLogger;
        private Stopwatch mStopwatch = new Stopwatch();
        public IRepository MyRepository = RepositoryFactory.CurrentRepository;

        #endregion

        #region Creation / Destcruction

        public RecordViewModel()
        {
            Logger.Info("Application started");
            InitCommands();

            Projects = MyRepository.GetProjects();
            if (Projects.Count > 0)
            {
                SelectedProject = Projects.First();
                Activities = new ObservableCollection<Activity>(SelectedProject.Activities);
                if (Activities.Count > 0)
                {
                    SelectedActivity = Activities.First();
                }
                TotalDuration = FormatTotalDuration(CalculateTotalDuration(SelectedProject));
            }
        }

        ~RecordViewModel()
        {
            if (StopWorkCommand.CanExecute(null))
            {
                StopWorkCommand.Execute(null);
            }
            MyRepository.Persist();
            Logger.Info("All data persisted before closing the application");
        }

        #endregion

        #region View-Bound Properties

        private ObservableCollection<Project> mProjects;
        public ObservableCollection<Project> Projects
        {
            get
            {
                return mProjects;
            }
            set
            {
                mProjects = value;
                NotifyPropertyChanged("Projects");
            }
        }

        private Project mSelectedProject;
        public Project SelectedProject
        {
            get
            {
                return mSelectedProject;
            }
            set
            {
                mSelectedProject = value;
                TotalDuration = FormatTotalDuration(CalculateTotalDuration(value));
                Activities = value == null ? null : value.Activities;
                SelectedActivity = null;

                NotifyPropertyChanged("EditActivityCondition");
                NotifyPropertyChanged("SelectActivityCondition");
                NotifyPropertyChanged("ShowDetailsCondition");
                NotifyPropertyChanged("SelectedProject");
            }
        }

        private ObservableCollection<Activity> mActivities;
        public ObservableCollection<Activity> Activities
        {
            get
            {
                return mActivities;
            }
            set
            {
                mActivities = value;
                NotifyPropertyChanged("Activities");
            }
        }

        private Activity mSelectedActivity;
        public Activity SelectedActivity
        {
            get
            {
                return mSelectedActivity;
            }
            set
            {
                mSelectedActivity = value;
                NotifyPropertyChanged("SelectedActivity");
                NotifyPropertyChanged("EditActivityCondition");
                NotifyPropertyChanged("ShowDetailsCondition");
            }
        }

        private bool mInProgress;
        public bool InProgress
        {
            get
            {
                return this.mInProgress;
            }
            set
            {
                this.mInProgress = value;
                NotifyPropertyChanged("InProgress");
                NotifyPropertyChanged("NotInProgress");
            }
        }

        public bool NotInProgress
        {
            get
            {
                return !InProgress;
            }
        }

        private string mTotalDuration;
        public string TotalDuration
        {
            get
            {
                return mTotalDuration;
            }
            set
            {
                mTotalDuration = value;
                NotifyPropertyChanged("TotalDuration");
            }
        }

        #endregion

        #region Commands

        public ICommand StartWorkCommand { get; set; }
        public ICommand StopWorkCommand { get; set; }
        public ICommand CreateActivityCommand { get; set; }
        public ICommand CreateProjectCommand { get; set; }
        public ICommand DeleteActivityCommand { get; set; }
        public ICommand ShowDetailsCommand { get; set; }

        private void InitCommands()
        {
            StartWorkCommand = new RelayCommand(o => StartWork(), o => StartWorkCondition());
            StopWorkCommand = new RelayCommand(o => StopWorkHandler(), o => StopWorkCondition());
            CreateActivityCommand = new RelayCommand(o => CreateActivityHandler(), o => CreateActivityCondition());
            CreateProjectCommand = new RelayCommand(o => CreateProjectHandler(), o => CreateProjectCondition());
            DeleteActivityCommand = new RelayCommand(o => DeleteActivityHandler(), o => DeleteActivityCondition());
            ShowDetailsCommand = new RelayCommand(o => ShowDetailsHandler(), o => ShowDetailsCondition());
        }

        #region Create Project

        private bool CreateProjectCondition()
        {
            return NotInProgress;
        }

        private void CreateProjectHandler()
        {           
            var craeteProjectViewModel = new CreateProjectViewModel(Projects);
            NavigatorFactory.MyNavigator.NavigateTo(craeteProjectViewModel);
            var newProject = Projects.FirstOrDefault(p => p.Name.Equals(craeteProjectViewModel.ProjectName));
            if(newProject != null) {
                SelectedProject = newProject;
            }
        }

        #endregion

        #region Select Project

        public bool SelectProjectCondition
        {
            get
            {
                return this.Projects != null && this.Projects.Count > 0 && NotInProgress;
            }
        }

        #endregion

        #region Create Activity

        public bool CreateActivityCondition()
        {
            return Projects.Count > 0 && SelectedProject != null && NotInProgress;
        }

        private void CreateActivityHandler()
        {
            var activity = new Activity { Description = "Neue Beschreibung ..." };
            SelectedProject.Activities.Add(activity);
            Activities = SelectedProject.Activities;
            SelectedActivity = activity;
            NotifyPropertyChanged("EditActivityCondition");
            NotifyPropertyChanged("SelectActivityCondition");
        }

        #endregion

        #region Delete Activity

        public bool DeleteActivityCondition()
        {
            return NotInProgress && Projects.Count > 0 && SelectedProject != null && SelectedActivity != null;
        }

        private void DeleteActivityHandler()
        {
            Activities.Remove(SelectedActivity);
            SelectedProject.Activities = Activities;
            SelectedActivity = Activities.OrderByDescending(a => a.LatestWork).FirstOrDefault();
            TotalDuration = FormatTotalDuration(CalculateTotalDuration(SelectedProject));
            NotifyPropertyChanged("EditActivityCondition");
            NotifyPropertyChanged("SelectActivityCondition");
        }

        #endregion

        #region Select Activity

        public bool SelectActivityCondition
        {
            get
            {
                return Projects.Count > 0 && SelectedProject != null && Activities.Count > 0 && NotInProgress;
            }
        }

        #endregion

        #region Edit Activity

        public bool EditActivityCondition
        {
            get
            {
                return this.SelectedActivity != null && NotInProgress;
            }
        }

        #endregion

        #region Show Details for Activity

        private void ShowDetailsHandler()
        {
            var detailsViewModel = new RecordDetailsViewModel(SelectedActivity);
            NavigatorFactory.MyNavigator.NavigateTo(detailsViewModel);
        }

        private bool ShowDetailsCondition()
        {
            return Projects.Count > 0 && SelectedProject != null && Activities.Count > 0 && SelectedActivity != null && SelectedActivity.ActivityTimes.Count > 0 && NotInProgress;
        }

        #endregion

        #region Stop Work

        private bool StopWorkCondition()
        {
            return InProgress;
        }

        private void StopWorkHandler()
        {
            mStopwatch.Stop();
            var endTime = DateTime.Now;
            var startTime = endTime - mStopwatch.Elapsed;

            SelectedActivity.ActivityTimes.Add(new ActivityTime { StartTime = startTime, EndTime = endTime });
            SelectedActivity.Duration = CalculateDuration(SelectedActivity);
            SelectedActivity.LatestWork = endTime;
            TotalDuration = FormatTotalDuration(CalculateTotalDuration(SelectedProject));
            InProgress = false;
            NotifyPropertyChanged("SelectProjectCondition");
            NotifyPropertyChanged("EditActivityCondition");
            NotifyPropertyChanged("SelectActivityCondition");
            NotifyPropertyChanged("ShowDetailsCondition");
        }

        private TimeSpan CalculateDuration(Activity activity)
        {
            TimeSpan totalTime = new TimeSpan(0);
            foreach (var activeTime in activity.ActivityTimes)
            {
                totalTime += activeTime.Duration;
            }
            return totalTime;
        }

        private TimeSpan CalculateTotalDuration(Project project)
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

        #endregion

        #region Start Work

        private bool StartWorkCondition()
        {
            return NotInProgress && Projects.Count > 0 && SelectedProject != null && SelectedActivity != null;
        }

        private void StartWork()
        {
            mStopwatch.Reset();
            mStopwatch.Start();
            InProgress = mStopwatch.IsRunning;
            NotifyPropertyChanged("SelectProjectCondition");
            NotifyPropertyChanged("EditActivityCondition");
            NotifyPropertyChanged("SelectActivityCondition");
            NotifyPropertyChanged("ShowDetailsCondition");
        }

        #endregion

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region Private Helper

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
