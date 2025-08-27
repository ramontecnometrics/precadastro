using model;

namespace api.Dtos
{
    public class ArquivoDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }

        public static ArquivoDto Build(Arquivo item)
        {
            var result = default(ArquivoDto);
            if (item != null)
            {
                result = new ArquivoDto()
                {
                    Id = item.Id,
                    Nome = item.Nome,
                    Tipo = item.Tipo,
                    Descricao = item.Descricao,
                };
            }
            return result;
        }
    }
}
