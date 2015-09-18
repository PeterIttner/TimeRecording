using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeRecording.Common.Navigation
{
    public static class NavigatorFactory
    {
        private static INavigator mMyNavigator = new Navigator();
        public static INavigator MyNavigator
        {
            get
            {
                return mMyNavigator;
            }
            set
            {
                mMyNavigator = value;
            }
        }

    }
}
