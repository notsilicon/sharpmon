using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMonitor.Entity
{
    public class DailyEntry
    {
        public DateTime Date { get; set; }
        public double Generated { get; set; }
        public double RunTime { get; set; }
        public string Forecast { get; set; }

        public override string ToString()
        {
            string[] list = typeof(LogEntry).GetProperties().Select(
                p => String.Format("{0}", p.GetValue(this, null).ToString())).ToArray();

            return String.Join(", ", list);
        }
    }
}
