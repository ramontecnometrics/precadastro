using System.Linq;
using model;
using api.Dtos;
using Microsoft.AspNetCore.Components;
using data;
using framework;
using System;
using System.Collections.Generic;

namespace api.Controllers
{
    [Route("auditoria/[action]")]
    [Route("auditoria")]
    public class AuditoriaController :
        ReadOnlyEntityController<
            AuditoriaFast,
            AuditoriaFastDto,
            AuditoriaGetParams>
    {
        public AuditoriaController(
            Repository<AuditoriaFast> repository, 
            IAppContext appContext
        ) : base(repository, appContext)
        {

        }

        protected override IQueryable<AuditoriaFast> Get(AuditoriaGetParams getParams)
        {
 
            var result = Repository.GetAll();

            var dataObrigatoria = !getParams.Id.HasValue;

            DateTime? dataInicial = null;
            DateTime? dataFinal = null;

            if (dataObrigatoria)
            {
                dataInicial = getParams.DataInicial ?? throw new Exception("Informe a data inicial.");
                dataFinal = getParams.DataFinal ?? throw new Exception("Informe a data final.");
            }

            var horaInicial = getParams.HoraInicial;
            var horaFinal = getParams.HoraFinal;

            if (dataInicial.HasValue)
            {
                if (!string.IsNullOrEmpty(horaInicial))
                {
                    dataInicial = dataInicial.Value.AddMinutes(TimeSpan.Parse(horaInicial).TotalMinutes);
                }

                result = result.Where(i => i.Data >= dataInicial);
            }

            if (dataFinal.HasValue)
            {
                if (string.IsNullOrEmpty(horaFinal))
                {
                    horaFinal = "23:59:59.999";
                }
                else
                {
                    horaFinal = $"{horaFinal}:59.999";
                }

                dataFinal = dataFinal.Value.AddMinutes(TimeSpan.Parse(horaFinal).TotalMinutes);

                result = result.Where(i => i.Data <= dataFinal);
            }         

            if (getParams.Id.HasValue)
            {
                result = result.Where(i => i.Id == getParams.Id.Value);
            }

            if (getParams.Usuario.HasValue)
            {
                result = result.Where(i => i.IdDoUsuario == getParams.Usuario.Value);
            }

            if (!string.IsNullOrEmpty(getParams.Descricao))
            {
                result = result.Where(i => i.Descricao.Contains(getParams.Descricao));
            }

            if (getParams.Acao != null)
            {
                result = result.Where(i => i.Acao == getParams.Acao.Id);
            }

            return result;
        }

        protected override AuditoriaFastDto Convert(AuditoriaFast entity)
        {
            var result = new AuditoriaFastDto()
            {
                Id = entity.Id,
                Descricao = entity.Descricao,
                Acao = entity.Acao,
                Data = entity.Data, 
                IdDoUsuario = entity.IdDoUsuario, 
                NomeDoUsuario = entity.NomeDoUsuario,
            };
            return result;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("[controller]/acoes")]
        public virtual Tipo<UserAction>[] GetAcoes()
        {
            var result = new List<Tipo<UserAction>>();

            foreach (var value in Enum.GetValues(typeof(UserAction)))
            {
                result.Add(new Tipo<UserAction>((int)(value)));
            }

            return result.ToArray();
        }
    }

    public class AuditoriaGetParams : IId
    {
        public long? Id { get; set; }
        public long? Usuario { get; set; }
        public string Descricao { get; set; }
        public DateTime? DataInicial { get; set; }
        public string HoraInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public string HoraFinal { get; set; }
        public Tipo<UserAction> Acao { get; set; }
    }

}
