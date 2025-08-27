using data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace model.Repositories
{
    public class InicializadorDeCidadeRepository
    {
        private readonly Repository<Cidade> CidadeRepository;
        private readonly Repository<Pais> PaisRepository;
        private readonly Repository<Estado> EstadoRepository;

        public InicializadorDeCidadeRepository(
            Repository<Pais> paisRepository,
            Repository<Estado> estadoRepository,
            Repository<Cidade> cidadeRepository) 
        {
            PaisRepository = paisRepository;
            EstadoRepository = estadoRepository;
            CidadeRepository = cidadeRepository;
        }

        public void AtualizarCidades()
        {
            if (PaisRepository.GetAll().Select(i=> i.Id).Count() == 0)
            {

                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.Resources.cidades.json"))
                {
                    if (stream != null)
                    {
                        StreamReader reader = new StreamReader(stream);
                        var jsonContent = reader.ReadToEnd();
                        Console.WriteLine("Importandos cidades...");
                        AtualizarCidadesFromJson(jsonContent);
                        Console.WriteLine("Cidades importadas com sucesso!");
                    }
                }
            }            
        }

        public void AtualizarCidadesFromJson(string jsonContent)
        {
            var brasilData = JsonConvert.DeserializeObject<BrasilJson>(jsonContent);

            // País
            var brasil = PaisRepository.Get(brasilData.Pais.Id, false);
            if (brasil == null)
            {
                brasil = new Pais
                {
                    Id = brasilData.Pais.Id,
                    Nome = brasilData.Pais.Nome,
                    Codigo = brasilData.Pais.Codigo
                };
                PaisRepository.Insert(brasil);
            }

            // Estados e cidades
            foreach (var estadoJson in brasilData.Estados)
            {
                var estado = EstadoRepository.Get(estadoJson.Id, false);
                if (estado == null)
                {
                    estado = new Estado
                    {
                        Id = estadoJson.Id,
                        Nome = estadoJson.Nome,
                        UF = estadoJson.Uf,
                        Pais = brasil
                    };
                    EstadoRepository.Insert(estado);
                }

                foreach (var cidadeJson in estadoJson.Cidades)
                {
                    var cidade = CidadeRepository.Get(cidadeJson.Id, false);
                    if (cidade == null)
                    {
                        cidade = new Cidade
                        {
                            Id = cidadeJson.Id,
                            Nome = cidadeJson.Nome,
                            Estado = estado
                        };
                        CidadeRepository.Insert(cidade);
                    }
                }
            }
        }
    }

    public class PaisJson
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }
    }

    public class EstadoJson
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Uf { get; set; }
        public List<CidadeJson> Cidades { get; set; }
    }

    public class CidadeJson
    {
        public long Id { get; set; }
        public string Nome { get; set; }
    }

    public class BrasilJson
    {
        public PaisJson Pais { get; set; }
        public List<EstadoJson> Estados { get; set; }
    }

}
