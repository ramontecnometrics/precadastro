using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace api.BackgroundServices
{
    public interface IScopedService
    {
        void Execute();
        bool Stopped { get; set; }
        int Delay { get; }
    }

    public interface IScopedServiceAsync : IScopedService
    {
        Task ExecuteAsync();
    }

    internal class ScopedProcessingService<T> : BackgroundService
        where T : IScopedService
    {
        private readonly IServiceProvider Services;
        private IScopedService scopedService;

        public ScopedProcessingService(IServiceProvider services)
        {
            this.Services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!Cfg.RunServices)
            {
                return;
            }

            var delay = 0;
            var isAsync = false;
            using (var scope = Services.CreateScope())
            {
                scopedService = scope.ServiceProvider.GetRequiredService<T>();
                delay = scopedService.Delay;
                if (scopedService is IScopedServiceAsync)
                {
                    isAsync = true;
                }
            };

            if (isAsync)
            {
                using (var scope = Services.CreateScope())
                {
                    scopedService = scope.ServiceProvider.GetRequiredService<T>();
                    delay = scopedService.Delay;
                    await ((IScopedServiceAsync)scopedService).ExecuteAsync();
                }
            }
            else
            {
                var task = Task<bool>.Factory.StartNew(() =>
                {
                    var stopped = false;
                    while (!stopped)
                    {
                        using (var scope = Services.CreateScope())
                        {
                            scopedService = scope.ServiceProvider.GetRequiredService<T>();                            
                            delay = scopedService.Delay;
//                            var startTime = DateTime.Now;
//                            Console.WriteLine(
//                                @"
//========================================================================
//Serviço: {0}
//Início: {1}", typeof(T).Name, startTime);
                            scopedService.Execute();
                            //var endTime = DateTime.Now;

                            //var elapsedTime = (endTime - startTime).TotalSeconds;

//                            if (elapsedTime > 30)
//                            {
//                                Console.WriteLine(
//                                    @"Fim: {0}

//LENTIDÃO!
//Tempo total (s): {1}

//========================================================================
//", endTime, elapsedTime.ToString("0.0"));
//                            }
//                            else {
//                                Console.WriteLine(
//                                    @"Fim: {0}
//Tempo total (s): {1}
//========================================================================
//", endTime, elapsedTime.ToString("0.0"));

//                            }

                            stopped = scopedService.Stopped;
                        }
                        Thread.Sleep(delay);
                    }
                    return true;

                });
                await task;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (scopedService != null)
            {
                scopedService.Stopped = true;
            }
            await base.StopAsync(cancellationToken);
        }
    }
}
