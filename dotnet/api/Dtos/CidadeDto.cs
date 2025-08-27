using model;

namespace api.Dtos
{
    public class CidadeDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public EstadoDto Estado { get; set; }

        public static CidadeDto Build(Cidade item)
        {
            var result = default(CidadeDto);
            if (item != null)
            {
                result = new CidadeDto()
                {
                    Id = item.Id,
                    Nome = item.Nome,
                    Estado = EstadoDto.FromModel(item.Estado),
                };
            }
            return result;
        }
    }
}
