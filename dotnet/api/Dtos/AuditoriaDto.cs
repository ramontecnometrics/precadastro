using System;
using framework;
using model;

namespace api.Dtos
{
    public class AuditoriaFastDto
    {
        public long Id { get; set; }
        public long IdDoUsuario { get; set; }
        public string NomeDoUsuario { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public Tipo<UserAction> Acao { get; set; }

        public static AuditoriaFastDto Build(AuditoriaFast item)
        {
            if (item == null)
            {
                return null;
            }
            var result = new AuditoriaFastDto()
            {
                Id = item.Id,
                Descricao = item.Descricao.GetPlainText(),
                IdDoUsuario = item.IdDoUsuario,
                NomeDoUsuario = item.NomeDoUsuario,
                Acao = item.Acao,
            };
            return result;
        }
    }
}

