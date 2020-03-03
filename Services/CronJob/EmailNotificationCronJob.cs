using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Services.CronJob
{
    public class EmailNotificationCronJob : CronJobService
    {
        private readonly ILogger<EmailNotificationCronJob> _logger;

        public EmailNotificationCronJob(IScheduleConfig<EmailNotificationCronJob> config, ILogger<EmailNotificationCronJob> logger)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob 3 starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} CronJob 3 is working.");
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob 3 is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}