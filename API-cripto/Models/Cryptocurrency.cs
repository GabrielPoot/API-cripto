using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace API_cripto.Models
{

    public class Cryptocurrency
    {
        [Key]
        public int Id { get; set; }

        [JsonProperty("id")]
        public string ApiId { get; set; }

        [Required(ErrorMessage = "El símbolo de la criptomoneda es obligatorio.")]
        [StringLength(10, ErrorMessage = "El símbolo no puede tener más de 10 caracteres.")]

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [Required(ErrorMessage = "El nombre de la criptomoneda es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]

        [JsonProperty("name")]
        public string Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El rango de capitalización de mercado debe ser mayor a 0.")]

        [JsonProperty("market_cap_rank")]
        public int? MarketCapRank { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio actual debe ser un valor positivo.")]

        [JsonProperty("current_price")]
        public double? CurrentPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El volumen de 24 horas debe ser un valor positivo.")]

        [JsonProperty("total_volume")]
        public double? Volume24h { get; set; }


        [JsonProperty("price_change_percentage_24h")]
        public double? Change24h { get; set; }

        [StringLength(200, ErrorMessage = "La ruta del ícono no puede tener más de 200 caracteres.")]

        [JsonProperty("image")]
        public string IconPath { get; set; }
    }

}
