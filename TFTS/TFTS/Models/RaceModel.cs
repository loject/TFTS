using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SQLite;

namespace TFTS.Models
{
    public class RaceModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        //public string Name { get; set; } /* TODO */
        public DateTime StartTime { get; set; } = DateTime.Now;
        public float Distance { get; set; }
        public float LapLength { get; set; }
        [TextBlob("RunnersSerizlized ")]
        public List<RunnerModel> Runners { get; set; }

        #region Properties
        public float LapsCount { get => Distance / LapLength; }
        #endregion
        #region misc
        public string RunnersSerizlized 
        {
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
                Runners = (List<RunnerModel>)tmp.Deserialize(sr, typeof(List<RunnerModel>));
            }
        }
        #endregion
    }
}
