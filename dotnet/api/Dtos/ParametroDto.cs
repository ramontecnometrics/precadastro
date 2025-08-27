using framework;

namespace api.Dtos
{
    public class ParametroDoSistemaDto
    {
        public long Id { get; set; }
        public int Grupo { get; set; }
        public int Ordem { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Valor { get; set; }
        public bool Protegido { get; set; }
        public bool Preenchido { get; set; }

        public static ParametroDoSistemaDto Build(model.ParametroDoSistema value)
        {
            if (value == null)
            {
                return null;
            }
            var result = new ParametroDoSistemaDto()
            {
                Id = value.Id,
                Descricao = value.Descricao,
                Grupo = value.Grupo,
                Nome = value.Nome,
                Ordem = value.Ordem,
                Protegido = value.Protegido,
                Valor = value.Protegido ? null : value.Valor.GetPlainText(),
                Preenchido = !string.IsNullOrEmpty(value.Valor.GetEncryptedText())
            };
            return result;
        }
    }
}
