using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TimeRecording.View;
using TimeRecording.ViewModel;

namespace TimeRecording.Common.Navigation
{
    public static class Bootstrapper
    {
        public static void Init()
        {
            NavigatorFactory.MyNavigator.Register(typeof(CreateProjectViewModel), typeof(CreateProjectView));
            NavigatorFactory.MyNavigator.Register(typeof(RecordDetailsViewModel), typeof(RecordDetailsView));
            NavigatorFactory.MyNavigator.Register(typeof(RecordViewModel), typeof(RecordView));

            NavigatorFactory.MyNavigator.NavigateTo(new RecordViewModel());
        }
    }
}
