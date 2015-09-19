using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeRecording.Common;
using TimeRecording.Common.Navigation;
using TimeRecording.IO.Repository;
using TimeRecording.Model;

namespace TimeRecording.ViewModel
{
    public class EditProjectViewModel : INotifyPropertyChanged
    {
        #region Member

        private IRepository CurrentRepository = RepositoryFactory.CurrentRepository;
        private Project mProject;

        #endregion

        #region Construction

        public EditProjectViewModel(Project project)
        {
            mProject = project;
            ProjectName = project.Name;

            if (project.TimeBudget.HasValue)
            {
                TimeBudget = project.TimeBudget.Value.TotalHours.ToString("0.00");
            }
            else
            {
                TimeBudget = string.Empty;
            }
            TimeBudgetUnits = new ObservableCollection<string>();
            TimeBudgetUnits.Add(TimeBudgetUnit.Hours.ToReadableString());
            TimeBudgetUnits.Add(TimeBudgetUnit.ManDays.ToReadableString());
            TimeBudgetUnits.Add(TimeBudgetUnit.ManMonths.ToReadableString());
            SelectedTimeBudgetUnit = TimeBudgetUnits.First();

            InitCommands();
        }


        #endregion

        #region Model

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

        private ObservableCollection<string> mTimeBudgetUnits;
        public ObservableCollection<string> TimeBudgetUnits
        {
            get
            {
                return mTimeBudgetUnits;
            }
            set
            {
                mTimeBudgetUnits = value;
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

        private string mSelectedTimeBudgetUnit;
        public string SelectedTimeBudgetUnit
        {
            get
            {
                return mSelectedTimeBudgetUnit;
            }
            set
            {
                mSelectedTimeBudgetUnit = value;
                NotifyPropertyChanged("SelectedTimeBudgetUnit");
            }
        }

        #endregion

        #region Commands

        public ICommand EditProjectCommand { get; set; }
        public ICommand DeleteProjectCommand { get; set; }

        private void InitCommands()
        {
            this.EditProjectCommand = new RelayCommand(o => EditProjectHandler(), o => EditProjectCondition());
            this.DeleteProjectCommand = new RelayCommand(o => DeleteProjectHandler(), o => DeleteProjectCondition());
        }
       
        #region Delete Project

        private void DeleteProjectHandler()
        {
            RepositoryFactory.CurrentRepository.DeleteProject(mProject);
            NavigatorFactory.MyNavigator.NavigateBack();
        }

        private bool DeleteProjectCondition()
        {
            return mProject != null;
        }

        #endregion

        #region Edit Project

        private void EditProjectHandler()
        {
            mProject.Name = mProjectName;
            mProject.TimeBudget = GetTimeBudget();
            NavigatorFactory.MyNavigator.NavigateBack();
        }

        private bool EditProjectCondition()
        {
            return CurrentRepository.IsProjectNameValid(ProjectName);
        }

        #endregion

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

        #region Private Helper

        private TimeSpan? GetTimeBudget()
        {
            TimeSpan? budget = null;
            double timeNumber = 0.0;
            var correctedTimeBudget = TimeBudget.Replace('.', ',');
            if (double.TryParse(correctedTimeBudget, out timeNumber))
            {
                var selectedUnit = SelectedTimeBudgetUnit.FromReadableString();
                if (selectedUnit == TimeBudgetUnit.Hours)
                {
                    budget = TimeSpan.FromHours(timeNumber);
                }
                else if (selectedUnit == TimeBudgetUnit.ManDays)
                {
                    budget = TimeSpan.FromHours(timeNumber * 8);
                }
                else if (selectedUnit == TimeBudgetUnit.ManMonths)
                {
                    budget = TimeSpan.FromHours(timeNumber * 8 * 20);
                }
            }
            return budget;
        }

        #endregion

    }
}

