using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFactorySdk
{
    class Program
    {
        static void Main(string[] args)
        {
            DataFactoryProcessEngine oEngine = new DataFactoryProcessEngine();
            oEngine.initateProcess();
        }
    }
}
