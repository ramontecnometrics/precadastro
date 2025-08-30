using ClosedXML;
using data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace api
{
    internal static class AppInit
    {
        public static void Init(IServiceProvider services)
        {
            SincronizarRelogioComOBanco(services);
            GerarRotinasDoSistema(services);
            GerarCidades(services);
            RefazerSearchables(services);
        }


        private static void SincronizarRelogioComOBanco(IServiceProvider provider)
        {
            var repository = provider.GetService<model.Repositories.DateTimeRepository>();
            repository.Sincronizar();
        }

        private static void GerarRotinasDoSistema(IServiceProvider provider)
        {
            var repository = provider.GetService<model.Repositories.RotinaDoSistemaRepository>();
            repository.AtualizarRotinas();
        }

        public static void GerarCidades(IServiceProvider provider)
        {
            var cidadeRepository = provider.GetService<model.Repositories.InicializadorDeCidadeRepository>();
            try
            {
                cidadeRepository.AtualizarCidades();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void RefazerSearchables(IServiceProvider services)
        {
            var config = services.GetRequiredService<IConfiguration>();

            var rebuildSearchable = config.GetValue<string>("RebuildSearchable") == "True";
            var rebuildSearchableEntityes = config.GetValue<string>("RebuildSearchableOfEntities");

            var rebuildSearchableEntityesArray = (rebuildSearchableEntityes == null ? "" : rebuildSearchableEntityes).Split(",");

            if (rebuildSearchable)
            {
                Console.WriteLine("Atualizando a propriedade \"Searchable\"...");
                try
                {
                    SearchableUpdater.Update(services, rebuildSearchableEntityesArray);
                    Console.WriteLine("Atualizado com sucesso!");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ocorreu um erro durante a atualização da propriedade \"Searchable\": \n\n{e.Message}");
                }
            }
        }
    }



    public class SearchableUpdater
    {
        public static void Update(IServiceProvider provider, string[] rebuildSearchableEntityes)
        {            
            using var scope = provider.CreateScope();
            var sp = scope.ServiceProvider;

            foreach (var entityType in TypeScanner.GetImplementations<ISearchableEntity>())
            {
                if (rebuildSearchableEntityes != null &&                    
                    !rebuildSearchableEntityes.Contains(entityType.Name))
                {
                    continue;
                }

                var repoType = typeof(Repository<>).MakeGenericType(entityType);
                var repo = sp.GetService(repoType) ?? Activator.CreateInstance(repoType);
                var getAll = repoType.GetMethod("GetAll", BindingFlags.Instance | BindingFlags.Public);
                var items = getAll.Invoke(repo, null) as IEnumerable<ISearchableEntity>;
                foreach(var item in items)
                {
                    var update = repoType.GetMethod("Update", new[] { entityType });
                    update.Invoke(repo, new object[] { item });
                }
            }
        }
    }

    public static class TypeScanner
    {
        public static IEnumerable<Type> GetImplementations<TInterface>()
        {
            var iface = typeof(TInterface);

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(SafeGetTypes) // evita ReflectionTypeLoadException
                .Where(t => t.IsClass && !t.IsAbstract && iface.IsAssignableFrom(t));
        }

        private static IEnumerable<Type> SafeGetTypes(Assembly a)
        {
            try { return a.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null)!; }
        }
    }
}
