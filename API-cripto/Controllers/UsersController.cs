using Microsoft.AspNetCore.Mvc;
using API_cripto.Data;
using API_cripto.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using API_cripto.DTOs;

namespace API_cripto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar si el nombre de usuario ya existe
            if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
            {
                return BadRequest(new { message = "El nombre de usuario ya está en uso." });
            }

            // Crear nuevo usuario basado en el DTO
            var newUser = new User
            {
                Username = userDto.Username,
                PasswordHash = HashPassword(userDto.Password), // Hashea la contraseña aquí
                Favorites = new List<Favorite>()
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado con éxito." });
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Nombre de usuario o contraseña incorrectos." });
            }
          
            return Ok(new { message = "Inicio de sesión exitoso.", userId = user.Id });
        }

        // Método para hashear la contraseña
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // Método para verificar la contraseña
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return HashPassword(enteredPassword) == storedHash;
        }

    }
}

