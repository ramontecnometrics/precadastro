using model;
using framework;

namespace api.Dtos
{
    public class UnidadeDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Uuid { get; set; }
        public string UnoSecretKey { get; set; }
        public string UnoAccessToken { get; set; }

        public static UnidadeDto Build(Unidade entity)
        {
            if (entity == null) return null;

            return new UnidadeDto()
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Uuid = entity.Uuid,
                UnoSecretKey = entity.UnoSecretKey.GetPlainText(),
                UnoAccessToken = entity.UnoAccessToken?.GetPlainText()
            };
        }
    }
}
