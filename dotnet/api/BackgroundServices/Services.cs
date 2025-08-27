using Microsoft.Extensions.DependencyInjection;

namespace api.BackgroundServices
{
    public class Services
    {
        public static void Install(IServiceCollection services)
        {   
            services.AddHostedService<ScopedProcessingService<ClearIdleSessionsService>>();
            services.AddScoped<ClearIdleSessionsService>();

            services.AddHostedService<ScopedProcessingService<ApagarLogsAntigosService>>();
            services.AddScoped<ApagarLogsAntigosService>();

            services.AddHostedService<ScopedProcessingService<LimpezaDeArquivosTemporarios>>();
            services.AddScoped<LimpezaDeArquivosTemporarios>();
        }
    }
}
