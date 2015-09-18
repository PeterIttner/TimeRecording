using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeRecording.ViewModel;
using System.Collections.ObjectModel;
using TimeRecording.Model;

namespace TimeRecordingTest.ViewModel
{
    [TestClass]
    public class CreateProjectViewModelTest
    {
        private CreateProjectViewModel viewModel;
        private ObservableCollection<Project> mockProjects = new ObservableCollection<Project>();

        [TestInitialize]
        public void Initialize()
        {
            viewModel = new CreateProjectViewModel(mockProjects);
        }

        [TestMethod]
        public void TestThatProjectNameMustLongerThanTwoCharacters()
        {
            viewModel.ProjectName = string.Empty;
            Assert.IsFalse(viewModel.CreateProjectCommand.CanExecute(null));
            viewModel.ProjectName = "1";
            Assert.IsFalse(viewModel.CreateProjectCommand.CanExecute(null));
            viewModel.ProjectName = "12";
            Assert.IsTrue(viewModel.CreateProjectCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestThatProjectNameMustAValidFilename()
        {
            viewModel.ProjectName = "~~ 23 . /";
            Assert.IsFalse(viewModel.CreateProjectCommand.CanExecute(null));
            viewModel.ProjectName = "a_valid_filename";
            Assert.IsTrue(viewModel.CreateProjectCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestThatCreateProjectCommandAddsProject()
        {
            var countBefore = mockProjects.Count;
            var projectName = "a_valid_filename";
            viewModel.ProjectName = projectName;
            viewModel.CreateProjectCommand.Execute(null);
            Assert.AreEqual(countBefore + 1, mockProjects.Count);
            Assert.IsTrue(mockProjects.Any(p => p.Name.Equals(projectName)));
        }
    }
}
