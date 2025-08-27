using model;

namespace api.Dtos
{
    public class PaisDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Codigo { get;  set; }

        public static PaisDto Build(Pais item)
        {
            if (item == null)
            {
                return null;
            }
            var result = new PaisDto()
            {
                Id = item.Id,
                Nome = item.Nome,
                Codigo = item.Codigo,
            };
            return result;
        }
    }
}
