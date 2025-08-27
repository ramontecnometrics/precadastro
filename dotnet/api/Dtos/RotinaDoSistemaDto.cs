using model;

namespace api.Dtos
{
    public class RotinaDoSistemaDto
    {
        public long Id { get; set; }
        public string Descricao { get; set; }

        public static RotinaDoSistemaDto Build(RotinaDoSistema value)
        {
            if (value == null)
            {
                return null;
            }
            var result = new RotinaDoSistemaDto()
            {
                Id = value.Id,
                Descricao = value.Descricao,
            };
            return result;
        }
    }
}