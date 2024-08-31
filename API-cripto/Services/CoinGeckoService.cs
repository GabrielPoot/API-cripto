using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic; 
using API_cripto.Models; 
using Newtonsoft.Json; 

namespace API_cripto.Services
{
    public class CoinGeckoService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public CoinGeckoService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("API-cripto", "1.0"));
            _cache = cache;
        }

        public async Task<List<Cryptocurrency>> GetTopCryptocurrenciesAsync(int count)
        {
            string cacheKey = $"TopCryptos-{count}";

            if (_cache.TryGetValue(cacheKey, out List<Cryptocurrency> cachedData))
            {
                return cachedData;
            }

            try
            {
                string url = $"https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page={count}&page=1&sparkline=false";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al obtener datos de CoinGecko API. Status code: {response.StatusCode}, Content: {errorContent}");
                    throw new HttpRequestException($"Error al obtener datos de CoinGecko API. Status code: {response.StatusCode}, Content: {errorContent}");
                }

                string data = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta JSON a la lista de criptomonedas
                var cryptocurrencies = JsonConvert.DeserializeObject<List<Cryptocurrency>>(data);

                // Guardar en caché los datos obtenidos de la API con una duración de 60 segundos
                _cache.Set(cacheKey, cryptocurrencies, TimeSpan.FromMinutes(1));

                return cryptocurrencies;
            }
            catch (HttpRequestException httpRequestException)
            {
                Console.WriteLine($"Error HTTP: {httpRequestException.Message}");
                throw new Exception("Ocurrió un error al comunicarse con la API de CoinGecko.", httpRequestException);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error General: {ex.Message} \nStackTrace: {ex.StackTrace}");
                throw new Exception("Ocurrió un error inesperado al manejar la solicitud.", ex);
            }
        }


    }
}
