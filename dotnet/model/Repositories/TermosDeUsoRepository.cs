using System;
using System.Dynamic;
using data;
using Newtonsoft.Json.Converters;

namespace model.Repositories
{
    public class TermosDeUsoRepository : Repository<TermoDeUso>
    {
        private readonly ParametroDoSistemaRepository ParametroDoSistemaRepository;

        public TermosDeUsoRepository(IUnitOfWork<TermoDeUso> unitOfWork, 
            ParametroDoSistemaRepository parametroDoSistemaRepository
        )
            : base(unitOfWork)
        {
            ParametroDoSistemaRepository = parametroDoSistemaRepository;
        }    

        public virtual TermoDeUso GetTermoDeUsoAtivo()
        {
            var parametroJson = ParametroDoSistemaRepository.GetString("TermoDeUsoAtivo");
            if(string.IsNullOrWhiteSpace(parametroJson))
            {
                return null;
            }
            var converter = new ExpandoObjectConverter();
            dynamic parametro = Newtonsoft.Json.JsonConvert
                .DeserializeObject<ExpandoObject>(parametroJson, converter);                    
            var result = Get(parametro.id);
            return result;
        }
    }
}
