using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.TimerService
{
    public class TitanbotTimer
    {
        private static TitanbotTimer _instance;
        public static TitanbotTimer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TitanbotTimer();
                return _instance;
            }
        }

        private TitanbotTimer()
        {

        }
    }
}
