using System;
using System.Collections.Generic;
using System.Text;

namespace doorfail.zethos.UI.parsing.scanner
{
    interface IScanner
    {
        int Status { get; set; }
        string errorMsg { get; set; }
        detectionString detection { get; set; }
        detectionString cur { get; set; }

        public detectionString GetDetection(int level,bool clearStatus);
        public int Next(String str);


        //enum Status
        // IDLE
        // ERROR
        // FOUND
        // FINISHED
        // other custom statuses considered Active
        public bool isFinished() => Status == 3;
        public bool isActive() => Status >2 && !isFinished();
        public bool isFound() => Status == 2;
    }
}
