using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TimeRecording.View;
using TimeRecording.ViewModel;

namespace TimeRecording.Common.Navigation
{
    public class Navigator : INavigator
    {
        private Dictionary<Type, Type> mBindings = new Dictionary<Type, Type>();
        private LinkedList<Window> mCurrentWindows = new LinkedList<Window>();

        public void NavigateTo(INotifyPropertyChanged viewModel)
        {
            var binding = mBindings.Where((b) => b.Key.Equals(viewModel.GetType())).FirstOrDefault();
            if (binding.Value != null)
            {
                var view = Activator.CreateInstance(binding.Value) as Window;
                if (view != null)
                {
                    view.Closed += (s, e) => mCurrentWindows.Remove(view);
                    if (mCurrentWindows.Count > 0)
                    {
                        var ownerWindow = mCurrentWindows.Last();
                        view.Owner = ownerWindow;
                    }
                    mCurrentWindows.AddLast(view);
                    view.DataContext = viewModel;
                    view.ShowDialog();
                }
            }
        }

        public void NavigateBack()
        {
            if (mCurrentWindows.Count > 0)
            {
                var currentWindow = mCurrentWindows.Last.Value;
                mCurrentWindows.RemoveLast();
                currentWindow.Close();
            }
        }

        public void Register(Type viewModelType, Type viewType)
        {
            mBindings.Add(viewModelType, viewType);
        }
    }
}
