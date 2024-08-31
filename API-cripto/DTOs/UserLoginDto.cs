﻿using System.ComponentModel.DataAnnotations;

namespace API_cripto.DTOs
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; }
    }

}
