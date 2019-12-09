using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsServiceJob.Jobs;
using Quartz;

namespace WindowsServiceJob
{
    public class Service
    {
        private IScheduler _scheduler { get; }

        public Service(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void OnStart()
        {
            _scheduler.Start();

            IJobDetail job = JobBuilder
                    .Create<MyJob>()
                    .WithIdentity(typeof(MyJob).Name, SchedulerConstants.DefaultGroup)
                    .Build();

            ITrigger trigger = TriggerBuilder
                                .Create()
                                .WithIdentity("Trigger", SchedulerConstants.DefaultGroup)
                                .ForJob(job)
                                .StartNow()
                                .WithSimpleSchedule(x=>x.WithIntervalInSeconds(10).RepeatForever())
                                .Build();

            _scheduler.ScheduleJob(job, trigger);
        }

        public void OnPaused() => _scheduler.PauseAll();

        public void OnContinue() => _scheduler.ResumeAll();

        public void OnStop() => _scheduler.Shutdown();
    }
}
