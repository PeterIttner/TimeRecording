using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeRecording.Model
{
    public class RepositoryFactory
    {
        private static IRepository RepositoryInstance;
        public static IRepository CurrentRepository
        {
            get
            {
                if (RepositoryInstance == null)
                {
                    RepositoryInstance = new FileRepository();
                }
                return RepositoryInstance;
            }
            set
            {
                RepositoryInstance = value;
            }
        }
    }
}
