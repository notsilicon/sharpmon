using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SharpMonitor.Entity
{
    public class LogEntry
    {
        public DateTime TimeStamp { get; set; }
        public double VoltageAC { get; set; }
        public double CurrentAC { get; set; }
        public uint PowerAC { get; set; }
        public double VoltageDC { get; set; }
        public double CurrentDC { get; set; }
        public uint PowerDC { get; set; }
        public ushort Choke { get; set; }
        public ushort D2D { get; set; }
        public ushort Heatsink { get; set; }
        public ushort Ambient { get; set; }
        public double WattHour { get; set; }
        public byte Online { get; set; }

        public override string ToString()
        {
            string[] list = typeof(LogEntry).GetProperties().Select(
                p => String.Format("{0}", p.GetValue(this, null).ToString())).ToArray();

            return String.Join(", ", list);
        }
    }
}