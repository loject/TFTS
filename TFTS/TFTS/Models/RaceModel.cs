using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using TFTS.misc;

using System.Runtime;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TFTS.Models
{
    [Serializable]
    public class RaceModel
    {
        //public string Name { get; set; } /* TODO */
        public DateTime StartTime { get; set; } = DateTime.Now;
        public float Distance { get; set; }
        public float LapLength { get; set; }
        [TextBlob("RunnersSerizlized ")]
        public SortableObservableCollection<RunnerModel> Runners { get; set; }

        public string RunnersSerizlized {
            get
            {
                try
                {
                    StringWriter sw = new StringWriter(new StringBuilder());
                    var tmp = JsonSerializer.Create();
                    tmp.Serialize(sw, Runners.ToList());
                    return sw.GetStringBuilder().ToString();
                }
                catch (Exception e)
                {
                    Console.Write("Error - " + e.Message);
                }
                return "";
            }
            set
            {
                StringReader sr = new StringReader(value);
                var tmp = JsonSerializer.Create();
                List<RunnerModel> list = (List<RunnerModel>)tmp.Deserialize(sr, typeof(List<RunnerModel>));
                Runners = new SortableObservableCollection<RunnerModel>(list ?? new List<RunnerModel>());
            }
        }
    }
}
