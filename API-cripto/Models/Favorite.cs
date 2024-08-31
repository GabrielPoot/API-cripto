using API_cripto.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Favorite
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El ID de la criptomoneda es obligatorio.")]
    public int CryptocurrencyId { get; set; }

    [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    [JsonIgnore]
    public User User { get; set; }
    public Cryptocurrency Cryptocurrency { get; set; }
}
