using framework;
using framework.Extensions;
using System;
using System.Text;

namespace api.BackgroundServices
{
    internal class ExceptionHandler
    {
        public static void SafeExecute(string module, Action action)
        {
            var startTime = DateTimeSync.Now;
            try
            {
                action();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{module}: {e.ToString()}");

                try
                {
                    log.Logger.Add(
                        Log.Modulo.WebApi.Description(),
                        module,
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


        public static void SafeExecute(string module, Func<Exception[]> action)
        {            
            try
            {
                var exceptions = action();
                var messages = new StringBuilder();
                exceptions.ForEach(i =>
                {
                    messages.AppendLine(">>>>>");
                    messages.AppendLine(i.ToString());
                    messages.AppendLine("<<<<<");
                });

                if (!string.IsNullOrWhiteSpace(messages.ToString()))
                {
                    throw new Exception(messages.ToString());
                }
            }
            catch (Exception e)
            {                
                Console.WriteLine($"{module}: {e.ToString()}");

                try
                {
                    log.Logger.Add(
                        Log.Modulo.WebApi.Description(),
                        module,
                        null,
                        e.ToString(),
                        Log.Categoria.Geral.ToInt(),
                        Log.SubCategoria.Erro.ToInt(),
                        null,
                        null,
                        "127.0.0.1",
                        DateTimeSync.Now,
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
