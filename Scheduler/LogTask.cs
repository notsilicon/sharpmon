using System;
using System.IO.Ports;
using System.Threading;
using Quartz;
using SharpMonitor.IO;
using log4net;
using System.Net;

namespace SharpMonitor.Scheduler
{
    public class LogTask : IStatefulJob
    {
        private ILog log;
        private JobExecutionContext context;
        private const string REQUEST_URL = "http://pvoutput.org/service/r1/addstatus.jsp?key={0}&sid={1}&d={2}&t={3}&v1={4}&v2={5}";

        #region IJob Members

        public void Execute(JobExecutionContext context)
        {
            this.log = LogManager.GetLogger(typeof(LogTask));
            this.context = context;

            var data = new SharpData();

            try
            {
                using (var port = new SerialPort(SharpMonitor.Properties.Settings.Default.PortName))
                {
                    // configure serial port
                    port.BaudRate = 9600;
                    port.DataBits = 8;
                    port.Parity = Parity.None;
                    port.StopBits = StopBits.One;
                    port.Open();

                    // create modbus master
                    DataReader dr = new DataReader(port);

                    do
                    {
                        data.SetPacket(dr.Read());
                        Thread.Sleep(250);
                    }
                    while (!data.IsValid);
                }
            }
            catch (Exception ex)
            {
                JobExecutionException jobEx = new JobExecutionException(ex);
                jobEx.RefireImmediately = true;
                throw jobEx;
            }

            Entity.LogEntry e = data.GetEntry();

            try
            {
                string key = SharpMonitor.Properties.Settings.Default.APIKey;
                string sid = SharpMonitor.Properties.Settings.Default.SystemId;

                if (!String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(sid))
                {
                    if (e.TimeStamp.Minute % 10 == 0)
                    {
                        double wh = e.Online == 1 ? e.WattHour : 0;
                        WebRequest request = WebRequest.Create(String.Format(REQUEST_URL, key, sid,
                            e.TimeStamp.ToString("yyyyMMdd"), e.TimeStamp.ToString("HH:mm"), wh, e.PowerAC));

                        // Send the 'WebRequest' and wait for response.
                        WebResponse response = request.GetResponse();
                    }
                }
            }
            catch (Exception ex)
            {
                JobExecutionException jobEx = new JobExecutionException(ex);
                throw jobEx;
            }
            finally
            {
                log.Info(e.ToString());
            }
        }

        private class SharpData
        {
            private DataPacket AC;
            private DataPacket DC;
            private DateTime time;

            public uint TodayWh
            {
                get
                {
                    return (IsValid ? AC.TodayWh : 0);
                }
            }

            public uint MinutesToday
            {
                get
                {
                    return (IsValid ? AC.MinutesToday : 0);
                }
            }

            public SharpData()
            {
                time = DateTime.Now;
            }

            public bool IsValid
            {
                get
                {
                    return (AC != null && DC != null);
                }
            }

            public void SetPacket(DataPacket packet)
            {
                if (packet.ACFlag == 0)
                {
                    DC = packet;
                }
                else
                {
                    AC = packet;
                }
            }

            public Entity.LogEntry GetEntry()
            {
                if (IsValid)
                {
                    return new Entity.LogEntry
                    {
                        TimeStamp = time,
                        VoltageAC = AC.Voltage,
                        CurrentAC = AC.Current,
                        PowerAC = AC.Power,
                        VoltageDC = DC.Voltage,
                        CurrentDC = DC.Current,
                        PowerDC = DC.Power,
                        Choke = AC.ChokeTemp,
                        D2D = AC.D2DHeatsinkTemp,
                        Heatsink = AC.InverterHeatsinkTemp,
                        Ambient = AC.AmbientTemp,
                        WattHour = AC.TodayWh,
                        Online = Convert.ToByte((AC.Online + DC.Online == 0) ? 0 : 1)
                    };
                }

                return new Entity.LogEntry { TimeStamp = time };
            }
        }

        #endregion
    }
}
