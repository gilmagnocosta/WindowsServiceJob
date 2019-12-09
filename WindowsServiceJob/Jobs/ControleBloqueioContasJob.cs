using log4net;
using Quartz;
using System.Threading.Tasks;

namespace WindowsServiceJob.Jobs
{
    public class MyJob : IJob
    {
        private ILog _log;

        public MyJob(ILog log) =>
            _log = log;

        async Task IJob.Execute(IJobExecutionContext context)
        {
            await Task.Run(() => _log.Info("Hi from MyJob"));
        }
    }
}
