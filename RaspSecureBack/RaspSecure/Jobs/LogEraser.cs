using Microsoft.Extensions.DependencyInjection;
using Quartz;
using RaspSecure.Context;
using System;
using System.Linq;

namespace RaspSecure.Jobs
{
    public class LogEraser: IJob
    {
        private readonly IServiceProvider _provider;
        public LogEraser(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void Execute(IJobExecutionContext context)
        {
            using (var scope = _provider.CreateScope())
            {
                var lastWeek = DateTime.Now.AddDays(-6);
                var _logs = scope.ServiceProvider.GetService<LogsDbContext>();
                var logs = _logs.LogEntitys.Where(l => l.AccessTime < lastWeek);
                _logs.LogEntitys.RemoveRange(logs);
                _logs.SaveChanges();
            }
        }
    }
}
