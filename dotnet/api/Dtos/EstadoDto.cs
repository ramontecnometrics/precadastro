using model;

namespace api.Dtos
{
    public class EstadoDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string UF { get; set; }
        public PaisDto Pais { get; set; }

        public static EstadoDto FromModel(Estado item)
        {
            if (item == null)
            {
                return null;
            }
            var result = new EstadoDto()
            {
                Id = item.Id,
                Nome = item.Nome,
                UF = item.UF,
                Pais = PaisDto.Build(item.Pais),
            };
            return result;
        }
    }
}
