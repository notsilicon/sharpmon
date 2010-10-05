using System;
using System.Linq;
using System.Xml.Linq;
using Quartz;
using SharpMonitor.IO;

namespace SharpMonitor.Scheduler
{
    public class DailyTask : IStatefulJob
    {
        #region IJob Members

        public void Execute(JobExecutionContext context)
        {
            try
            {
                // Get sunrise time, sunset time and weather condition
                XElement root = XElement.Load(SharpMonitor.Properties.Settings.Default.WeatherServiceUrl);
                XNamespace yweather = "http://xml.weather.yahoo.com/ns/rss/1.0";

                XElement astronomy = root.Descendants(yweather + "astronomy").First();
                string sunrise = astronomy.Attribute("sunrise").Value;
                string sunset = astronomy.Attribute("sunset").Value;

                DateTime ts = DateTime.ParseExact(
                    String.Format("{0:d/M/yyyy} {1}", DateTime.Today, sunrise),
                    "d/M/yyyy h:mm tt", null);
                DateTime te = DateTime.ParseExact(
                    String.Format("{0:d/M/yyyy} {1}", DateTime.Today, sunset),
                    "d/M/yyyy h:mm tt", null);

                if (DateTime.Now < te)
                {
                    int period = SharpMonitor.Properties.Settings.Default.RepeatIntervalInSeconds;

                    Trigger trigger = new SimpleTrigger("SimpleTrigger",
                        DateTime.Now > ts ? TriggerUtils.GetNextGivenSecondDate(null, period % 60) : ts.ToUniversalTime(),
                        te.ToUniversalTime(),
                        SimpleTrigger.RepeatIndefinitely,
                        TimeSpan.FromSeconds(period));

                    Type jobType = typeof(LogTask);
                    JobDetail job = new JobDetail(jobType.Name, null, jobType);
                    context.Scheduler.ScheduleJob(job, trigger);
                }
            }
            catch (Exception ex)
            {
                throw new JobExecutionException(ex);
            }
        }

        #endregion
    }
}
