using System;
using Quartz;
using Quartz.Impl;
using SharpMonitor.IO;
using SharpMonitor.Scheduler;
using System.ServiceProcess;

namespace SharpMonitor
{
    class MonitorService : ServiceBase
    {
        static IScheduler sched;

        public MonitorService()
        {
            this.ServiceName = "Sharp Monitoring Service";

            // create the job to be scheduled
            Type jobType = typeof(DailyTask);
            JobDetail job = new JobDetail(jobType.Name, null, jobType);

            ISchedulerFactory schedFact = new StdSchedulerFactory();
            sched = schedFact.GetScheduler();
            sched.Start();

            // schedule daily cron job
            Trigger trigger = new CronTrigger("CronTrigger", null, jobType.Name, null, SharpMonitor.Properties.Settings.Default.CornExpression);
            sched.AddJob(job, true);

            DateTime ft = sched.ScheduleJob(trigger);

            // start job immediately if next schedule job is tomorrow
            if (ft > DateTime.UtcNow)
            { 
                sched.TriggerJob(jobType.Name, null);         
            }
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            base.OnStop();

            if (sched != null)
            {
                sched.Shutdown();
            }
        }
    }
}
