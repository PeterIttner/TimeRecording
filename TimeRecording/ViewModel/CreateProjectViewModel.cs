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
using TimeRecording.Common.Navigation;
using TimeRecording.Model;

namespace TimeRecording.ViewModel
{
    public class CreateProjectViewModel : INotifyPropertyChanged
    {
        #region Member

        private IRepository CurrentRepository =  RepositoryFactory.CurrentRepository;
        private ObservableCollection<Project> mProjects;

        #endregion

        #region C'tor
        
        public CreateProjectViewModel(ObservableCollection<Project> projects)
        {
            this.CreateProjectCommand = new RelayCommand(o => CreateProjectHandler(), o => CurrentRepository.IsProjectNameValid(ProjectName));
            mProjects = projects; 
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

        #endregion

        #region Commands

        public ICommand CreateProjectCommand { get; set; }

        private void CreateProjectHandler()
        {
            mProjects.Add(new Project { Name = ProjectName });
            NavigatorFactory.MyNavigator.NavigateBack();
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
