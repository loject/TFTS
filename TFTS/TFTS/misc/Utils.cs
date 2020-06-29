using System;
using System.Collections.Generic;
using System.Text;

namespace TFTS
{
    public static class Utils
    {
        static public string getStringFromTimeSpan(TimeSpan timeSpan)
        {
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;
            int miliseconds = timeSpan.Milliseconds / 100;
            return $"{hours:00}:{minutes:00}:{seconds:00}.{miliseconds:0}";
        }
    }
}
