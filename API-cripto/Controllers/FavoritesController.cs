using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_cripto.Data;
using API_cripto.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using API_cripto.DTOs;

namespace API_cripto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/favorites/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetFavorites(int userId)
        {
            try
            {
                // Verificar si el usuario existe
                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    return NotFound(new { message = "El usuario no existe." });
                }

                var favorites = await _context.Favorites
                    .Include(f => f.Cryptocurrency)
                    .Where(f => f.UserId == userId)
                    .ToListAsync();

                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al obtener los favoritos.", details = ex.Message });
            }
        }


        // POST: api/favorites
        [HttpPost]
        public async Task<IActionResult> PostFavorite([FromBody] AddFavoriteDto favoriteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _context.Users.FindAsync(favoriteDto.UserId);
                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado." });
                }

                var cryptocurrency = await _context.Cryptocurrencies
                    .FirstOrDefaultAsync(c => c.ApiId == favoriteDto.Cryptocurrency.ApiId);

                if (cryptocurrency == null)
                {
                    cryptocurrency = new Cryptocurrency
                    {
                        ApiId = favoriteDto.Cryptocurrency.ApiId,
                        Symbol = favoriteDto.Cryptocurrency.Symbol,
                        Name = favoriteDto.Cryptocurrency.Name,
                        CurrentPrice = favoriteDto.Cryptocurrency.CurrentPrice,
                        MarketCapRank = favoriteDto.Cryptocurrency.MarketCapRank,
                        Volume24h = favoriteDto.Cryptocurrency.Volume24h,
                        Change24h = favoriteDto.Cryptocurrency.Change24h,
                        IconPath = favoriteDto.Cryptocurrency.IconPath
                    };
                    _context.Cryptocurrencies.Add(cryptocurrency);
                    await _context.SaveChangesAsync();
                }

                var existingFavorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == favoriteDto.UserId && f.CryptocurrencyId == cryptocurrency.Id);

                if (existingFavorite != null)
                {
                    return BadRequest(new { message = "La criptomoneda ya está en favoritos." });
                }

                var favorite = new Favorite
                {
                    UserId = favoriteDto.UserId,
                    CryptocurrencyId = cryptocurrency.Id
                };

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetFavorites), new { userId = favorite.UserId }, favorite);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", details = ex.Message });
            }
        }




        // DELETE: api/favorites/{userId}/{id}
        [HttpDelete("{userId}/{id}")]
        public async Task<IActionResult> DeleteFavorite(int userId, int id)
        {
            try
            {
                // Busca el favorito basado en el userId y el cryptocurrencyId (id).
                var favorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.CryptocurrencyId == id);

                if (favorite == null)
                {
                    return NotFound(new { message = "No se encontró el favorito especificado." });
                }

                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al eliminar el favorito de la base de datos.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", details = ex.Message });
            }
        }

    }
}
