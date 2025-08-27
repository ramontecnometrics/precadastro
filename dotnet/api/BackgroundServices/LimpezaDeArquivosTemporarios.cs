using framework;
using framework.Extensions;
using System;

namespace api.BackgroundServices
{
    internal class LimpezaDeArquivosTemporarios : IScopedService
    {
        public bool Stopped { get; set; }
        public int Delay => 1000 * 60 * 30;  // 30 minutos

        private readonly FileManager FileManager;

        public LimpezaDeArquivosTemporarios(FileManager fileManager)
        {
            FileManager = fileManager;
        }

        public void Execute()
        {
            SafeExecute(() => FileManager.LimparArquivos());
        }

        private void SafeExecute(Action action)
        {
            var startTime = DateTimeSync.Now;
            try
            {
                action();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{this.GetType().Name}: {e.ToString()}");
                try
                {
                    log.Logger.Add(
                        Log.Modulo.WebApi.Description(),
                        this.GetType().Name,
                        null,
                        e.ToString(),
                        Log.Categoria.Geral.ToInt(),
                        Log.SubCategoria.Erro.ToInt(),
                        null,
                        null,
                        "127.0.0.1",
                        startTime,
                        null,
                        null);
                }
                catch (System.Exception e2)
                {
                    Console.WriteLine($"Erro ao gravar log: {e2.ToString()}");
                }
            }
        }

    }
}
