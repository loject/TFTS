using System;

namespace TFTS.Models
{
    public struct Lap
    {
        public float Length { get; set; }
        public TimeSpan Time { get; set; }
        public string TimeStr { get => Utils.getStringFromTimeSpan(Time); }
        public double Speed { get => Length / Time.TotalSeconds; }
        public int Position { get; set; }
    }
}
