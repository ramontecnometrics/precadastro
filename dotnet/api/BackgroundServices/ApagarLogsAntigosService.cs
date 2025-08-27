using framework.Extensions;
using model;
using model.Repositories;
using System;
using framework;

namespace api.BackgroundServices
{
    internal class ApagarLogsAntigosService : IScopedService
    {
        public bool Stopped { get; set; }
        public int Delay => 1000 * 60 * 60 * 5;

        public ApagarLogsAntigosService()
        {            
        }

        public void Execute()
        {
            Apagar();
        }

        protected virtual void Apagar()
        {
            ExceptionHandler.SafeExecute(this.GetType().Name, () =>
            {
                log.Logger.Delete($"action = '/receiver/moko' and date < now() - interval '3' day");
                log.Logger.Delete($"date < now() - interval '30' day ");                
            });
        }
         
    }
}
