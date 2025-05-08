using Infra.Core.Abstract;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Infra.Core.System.Extensions;

namespace Infra.IdGenerater.Yitter
{
    public sealed class WorkerNodeHostedService(ILogger<WorkerNodeHostedService> logger
           , WorkerNode workerNode
           , IServiceInfo serviceInfo) : BackgroundService
    {
        private readonly ILogger<WorkerNodeHostedService> logger = logger;
        private readonly string serviceName = serviceInfo.ShortName;
        private readonly WorkerNode workerNode = workerNode;
        private readonly int millisecondsDelay = 1000 * 60;

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await workerNode.InitWorkerNodesAsync(serviceName);
            var workerId = await workerNode.GetWorkerIdAsync(serviceName);

            IdGenerater.SetWorkerId((ushort)workerId);

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            logger.LogInformation("stopping service {serviceName}", serviceName);

            var subtractionMilliseconds = 0 - (millisecondsDelay * 1.5);
            var score = DateTime.Now.AddMilliseconds(subtractionMilliseconds).GetTotalMilliseconds();
            await workerNode.RefreshWorkerIdScoreAsync(serviceName, IdGenerater.CurrentWorkerId, score);

            logger.LogInformation("stopped service {serviceName}:{score}", serviceName, score);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(millisecondsDelay, stoppingToken);

                    if (stoppingToken.IsCancellationRequested) break;

                    await workerNode.RefreshWorkerIdScoreAsync(serviceName, IdGenerater.CurrentWorkerId);
                }
                catch (Exception ex)
                {
                    logger.LogError("异常:{ex}", [ex]);
                    await Task.Delay(millisecondsDelay / 3, stoppingToken);
                }
            }
        }
    }
}
