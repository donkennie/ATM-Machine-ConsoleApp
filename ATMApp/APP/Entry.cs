using ATMApp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMApp.APP
{
    public class Entry
    {

        static void Main(string[] args)
        {
  
                ATMApp atmApp = new ATMApp();
                atmApp.InitializeData();

                atmApp.Run();          
        }

    }
}
