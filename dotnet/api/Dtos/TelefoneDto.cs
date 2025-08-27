using framework;
using model;

namespace api.Dtos
{
    public class TelefoneDto
    {
        public long Id { get; set; }
        public int DDD { get; set; }
        public string Numero { get; set; }
        public bool TemWhatsApp { get; set; }
        public int Tipo { get; set; }
        public string Pais { get; set; }

        public string NumeroComDDD
        {
            get
            {
                return this.DDD > 0 && !string.IsNullOrWhiteSpace(Numero) ? string.Format(
                    "({0}){1}",
                    this.DDD.ToString("00"),
                    this.GetNumeroComDigito()) : null;
            }
        }

        private string GetNumeroComDigito()
        {
            if (this.Numero.Length == 8)
            {
                return $"{this.Numero.Substring(0, 4)}-{this.Numero.Substring(4, 4)}";
            }
            else
            if (this.Numero.Length == 9)
            {
                return $"{this.Numero.Substring(0, 5)}-{this.Numero.Substring(5, 4)}";
            }
            else
            {
                return this.Numero;
            }
        }

        internal static TelefoneDto Build(TelefoneDePessoa item)
        {
            var result = default(TelefoneDto);
            if (item != null)
            {
                result = new TelefoneDto()
                {
                    Id = item.Id,
                    Pais = item.Pais,
                    DDD = item.DDD,
                    Numero = item.Numero.GetPlainText(),
                    Tipo = item.Tipo,
                    TemWhatsApp = item.TemWhatsApp
                };
            }
            return result;
        }
    }
}