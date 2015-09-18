using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeRecording.Model;
using TimeRecording.ViewModel;

namespace TimeRecordingTest.ViewModel
{
    [TestClass]
    public class RecordDetailsViewModelTest
    {
        private RecordDetailsViewModel viewModel;
        private Activity mockActivity = new Activity();

        [TestInitialize]
        public void Initialize()
        {
            string description = "this is my activity";
            mockActivity.ActivityTimes = new System.Collections.ObjectModel.ObservableCollection<ActivityTime>();
            mockActivity.ActivityTimes.Add(new ActivityTime { StartTime = new DateTime(2015, 9, 25, 12, 0, 0), EndTime = new DateTime(2015, 9, 25, 13, 29, 0) });
            mockActivity.ActivityTimes.Add(new ActivityTime { StartTime = new DateTime(2015, 9, 25, 14, 0, 0), EndTime = new DateTime(2015, 9, 25, 15, 0, 0) });
            mockActivity.Description = description;
            viewModel = new RecordDetailsViewModel(mockActivity);
        }

        [TestMethod]
        public void TestThatPropertiesAreInitializedRight()
        {
            Assert.AreEqual(mockActivity.Description, viewModel.ActivityDescription);
            Assert.AreEqual(mockActivity.ActivityTimes, viewModel.ActivityTimes);
        }
    }
}
