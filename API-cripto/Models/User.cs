using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_cripto.Models
{

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede tener más de 50 caracteres.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string PasswordHash { get; set; } 

        [JsonIgnore]
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }

}
