using framework.Extensions;
using model;
using model.Repositories;
using System;
using framework;

namespace api.BackgroundServices
{
    internal class ClearIdleSessionsService : IScopedService
    {
        public bool Stopped { get; set; }
        public int Delay => 1000 * 60 * 5;  // 5 minutos
        
        private readonly ClearIdleSessionsRepository ClearIdleSessionsRepository;

        public ClearIdleSessionsService(ClearIdleSessionsRepository clearIdleSessionsRepository)
        {
            ClearIdleSessionsRepository = clearIdleSessionsRepository;
        }

        public void Execute()
        {
            ExceptionHandler.SafeExecute(this.GetType().Name, () =>
            {
                ClearIdleSessionsRepository.Execute();
            });
        }
    }
}
