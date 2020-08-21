using Newtonsoft.Json;
using System;

namespace TFTS.Models
{
    public struct Lap
    {
        public float Length { get; set; }
        public TimeSpan Time { get; set; }
        [JsonIgnore]
        public string TimeStr { get => Utils.getStringFromTimeSpan(Time); }
        [JsonIgnore]
        public double Speed { get => Length / Time.TotalSeconds; }
        public int Position { get; set; }
    }
}
