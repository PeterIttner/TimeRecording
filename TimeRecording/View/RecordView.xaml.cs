using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TimeRecording.ViewModel;

namespace TimeRecording.View
{
    /// <summary>
    /// Interaction logic for RecordView.xaml
    /// </summary>
    public partial class RecordView : Window
    {
        private Storyboard mProgressAnimationBoard = new Storyboard();

        public RecordView()
        {
            InitializeComponent();
            this.Loaded += RecordView_Loaded;
        }

        void RecordView_Loaded(object sender, RoutedEventArgs e)
        {
            InitAnimations();
            var dataContext = DataContext as INotifyPropertyChanged;
            if (dataContext != null)
            {
                dataContext.PropertyChanged += ViewModelPropertyChangedHandler;
            }
        }

        private void ViewModelPropertyChangedHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("InProgress") || e.PropertyName.Equals("NotInProgress"))
            {
                var inProgress = sender.GetType().GetProperty("InProgress").GetValue(sender) as bool?;
                if (inProgress.HasValue && inProgress.Value == true)
                {
                    StartAnimation();
                }
                else
                {
                    StopAnimation();
                }
            }
        }

        private void StartAnimation()
        {
            WorkingProgressBar.BeginStoryboard(mProgressAnimationBoard, HandoffBehavior.SnapshotAndReplace, true);
        }

        private void StopAnimation()
        {
            mProgressAnimationBoard.Stop(WorkingProgressBar);
        }

        private void InitAnimations()
        {
            Duration duration = new Duration(TimeSpan.FromSeconds(1));
            DoubleAnimation fillAnimation = new DoubleAnimation(100, duration);
            DoubleAnimation clearAnimation = new DoubleAnimation(0, duration);
            clearAnimation.BeginTime = TimeSpan.FromSeconds(1);

            Storyboard.SetTargetName(fillAnimation, WorkingProgressBar.Name);
            Storyboard.SetTargetProperty(fillAnimation, new PropertyPath(ProgressBar.ValueProperty));
            Storyboard.SetTargetName(clearAnimation, WorkingProgressBar.Name);
            Storyboard.SetTargetProperty(clearAnimation, new PropertyPath(ProgressBar.ValueProperty));

            mProgressAnimationBoard.RepeatBehavior = RepeatBehavior.Forever;
            mProgressAnimationBoard.Children.Add(fillAnimation);
            mProgressAnimationBoard.Children.Add(clearAnimation);
        }
    }
}
