using Microsoft.AspNetCore.Mvc;
using API_cripto.Services;
using API_cripto.Data;
using API_cripto.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace API_cripto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptocurrenciesController : ControllerBase
    {
        private readonly CoinGeckoService _coinGeckoService;
        private readonly ApplicationDbContext _context;

        public CryptocurrenciesController(CoinGeckoService coinGeckoService, ApplicationDbContext context)
        {
            _coinGeckoService = coinGeckoService;
            _context = context;
        }

        // GET: api/cryptocurrencies/popular
        [HttpGet("popular")]
        public async Task<IActionResult> GetTopCryptocurrencies()
        {
            try
            {
                var cryptocurrencies = await _coinGeckoService.GetTopCryptocurrenciesAsync(10);
                return Ok(cryptocurrencies);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new { message = "No se puede conectar con la API.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", details = ex.Message });
            }
        }



        // POST: api/cryptocurrencies
        [HttpPost]
        public async Task<ActionResult<Cryptocurrency>> AddCryptocurrency([FromBody] Cryptocurrency cryptocurrency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (await _context.Cryptocurrencies.AnyAsync(c => c.Symbol == cryptocurrency.Symbol))
                {
                    return BadRequest(new { message = "La criptomoneda ya existe en la base de datos." });
                }

                _context.Cryptocurrencies.Add(cryptocurrency);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTopCryptocurrencies), new { id = cryptocurrency.Id }, cryptocurrency);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al guardar la criptomoneda en la base de datos.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", details = ex.Message });
            }
        }
    }
}
