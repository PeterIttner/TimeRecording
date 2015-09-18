using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecording.Common.Navigation;
using TimeRecording.Model;
using TimeRecording.ViewModel;

namespace TimeRecordingTest.ViewModel
{
    [TestClass]
    public class RecordViewModelTest
    {
        private RecordViewModel viewModel;
        private Activity mockActivity = new Activity();

        [TestInitialize]
        public void Initialize()
        {
            var workSpan1 = new ActivityTime { StartTime = new DateTime(2015, 9, 30, 12, 0, 0), EndTime = new DateTime(2015, 9, 30, 13, 29, 12) };
            var workSpan2 = new ActivityTime { StartTime = new DateTime(2015, 9, 30, 14, 0, 0), EndTime = new DateTime(2015, 9, 30, 15, 0, 0) };
            var workSpans1 = new ObservableCollection<ActivityTime>();
            workSpans1.Add(workSpan1);
            workSpans1.Add(workSpan2);

            
            var activity1 = new Activity { ActivityTimes = workSpans1, Description = "this is the first activity" };
            activity1.Duration = workSpan1.Duration + workSpan2.Duration;

            var workSpan3 = new ActivityTime { StartTime = new DateTime(2015, 9, 28, 12, 0, 0), EndTime = new DateTime(2015, 9, 29, 13, 29, 12) };
            var workSpan4 = new ActivityTime { StartTime = new DateTime(2015, 9, 30, 14, 0, 0), EndTime = new DateTime(2015, 9, 30, 15, 0, 0) };
            var workSpans2 = new ObservableCollection<ActivityTime>();
            workSpans2.Add(workSpan3);
            workSpans2.Add(workSpan4);

            var activity2 = new Activity { ActivityTimes = workSpans2, Description = "this is the second activity" };
            activity2.Duration = workSpan3.Duration + workSpan4.Duration;

            var activities = new ObservableCollection<Activity>();
            activities.Add(activity1);
            activities.Add(activity2);

            var project1 = new Project { Activities = activities, Name = "this is the first mock project" };

            var projects = new ObservableCollection<Project>();
            projects.Add(project1);

            var repository = new Mock<IRepository>();
            repository.Setup(r => r.GetProjects()).Returns(projects);
            RepositoryFactory.CurrentRepository = repository.Object;
                        
            viewModel = new RecordViewModel();
        }

        [TestMethod]
        public void TestThatCreationInitsActivitesRight()
        {
            Assert.IsFalse(viewModel.InProgress);
            Assert.IsTrue(viewModel.NotInProgress);
            Assert.IsTrue(viewModel.Projects.Count > 0);
            Assert.IsTrue(viewModel.SelectActivityCondition);
            Assert.IsNotNull(viewModel.SelectedActivity);
            Assert.IsNotNull(viewModel.SelectedProject);
            Assert.IsTrue(viewModel.SelectProjectCondition);
            Assert.IsTrue(viewModel.ShowDetailsCommand.CanExecute(null));
            Assert.IsTrue(viewModel.StartWorkCommand.CanExecute(null));
            Assert.IsFalse(viewModel.StopWorkCommand.CanExecute(null));
            Assert.IsTrue(viewModel.CreateProjectCommand.CanExecute(null));
            Assert.IsTrue(viewModel.CreateActivityCommand.CanExecute(null));
            Assert.IsTrue(viewModel.DeleteActivityCommand.CanExecute(null));
            Assert.IsTrue(viewModel.EditActivityCondition);
            Assert.IsNotNull(viewModel.TotalDuration);
        }

        [TestMethod]
        public void TestThatTotalDurationIsCalculatedAndFormattedRight()
        {
            Assert.AreEqual("3,62167 Manntage ≙ 28,97 Stunden ≙ 1738,40 Minuten ≙ 104304,00 Sekunden", viewModel.TotalDuration);
        }

        [TestMethod]
        public void TestThatInProgressGetsChangedWhileRunning()
        {
            viewModel.StartWorkCommand.Execute(null);
            Assert.IsTrue(viewModel.InProgress);
            viewModel.StopWorkCommand.Execute(null);
            Assert.IsFalse(viewModel.InProgress);
        }

        [TestMethod]
        public void TestThatStartWorkCommandIsNotPossibleWhenNoProjectsAvailable()
        {
            viewModel.Projects = new ObservableCollection<Project>();
            Assert.IsFalse(viewModel.StartWorkCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestThatStartWorkCommandIsNotPossibleWhenNoProjectIsSelected()
        {
            viewModel.SelectedProject = null;
            Assert.IsFalse(viewModel.StartWorkCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestThatStopWorkCommandIsOnlyPossibleWhenStartedBefore()
        {
            viewModel.StartWorkCommand.Execute(null);
            Assert.IsTrue(viewModel.StopWorkCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestThatCreateProjectCommandIsNotPossibleWhenMeasuringTime()
        {
            viewModel.StartWorkCommand.Execute(null);
            Assert.IsFalse(viewModel.CreateProjectCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestThatCreateProjectCommandWantsToNavigateToOtherViewModel()
        {
            var navigator = new Mock<INavigator>();
            NavigatorFactory.MyNavigator = navigator.Object;
            
            viewModel.CreateProjectCommand.Execute(null);
            navigator.Verify(n => n.NavigateTo(It.IsAny<INotifyPropertyChanged>()));
        }

        [TestMethod]
        public void TestThatShowDetailsCommandWantsToNavigateToOtherViewModel()
        {
            var navigator = new Mock<INavigator>();
            NavigatorFactory.MyNavigator = navigator.Object;

            viewModel.ShowDetailsCommand.Execute(null);
            navigator.Verify(n => n.NavigateTo(It.IsAny<INotifyPropertyChanged>()));
        }

        [TestMethod]
        public void TestThatCreateActivityCommandAddsNewActivity()
        {
            var countBefore = viewModel.Activities.Count;
            viewModel.CreateActivityCommand.Execute(null);
            Assert.AreEqual(countBefore + 1, viewModel.Activities.Count);
        }

        [TestMethod]
        public void TestThatDeleteActivityCommandDeletesSelectedActivity()
        {
            var countBefore = viewModel.Activities.Count;
            viewModel.DeleteActivityCommand.Execute(null);
            Assert.AreEqual(countBefore - 1, viewModel.Activities.Count);
        }
    }
}
