using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeRecording.Common.Navigation
{
    public interface INavigator
    {
        void NavigateTo(INotifyPropertyChanged viewModel);
        void NavigateBack();
        void Register(Type viewModelType, Type viewType);
    }
}
