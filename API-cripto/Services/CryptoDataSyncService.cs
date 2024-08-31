using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using API_cripto.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using API_cripto.Models;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection; 

public class CryptoDataSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;  

    public CryptoDataSyncService(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Iniciando la tarea de sincronización de datos de criptomonedas.");

            try
            {
                await UpdateCryptoDataFromApi();
                Console.WriteLine("Datos de criptomonedas sincronizados exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al sincronizar los datos de criptomonedas: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Ejecuta cada minuto
        }
    }

    private async Task UpdateCryptoDataFromApi()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var client = _httpClientFactory.CreateClient();

            try
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("API-cripto", "1.0"));

                // Hacer la solicitud GET a la API pública de CoinGecko
                var response = await client.GetAsync("https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page=10&page=1");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                // Deserializar JSON en una lista de objetos Cryptocurrency
                var cryptoDataList = JsonConvert.DeserializeObject<List<Cryptocurrency>>(content);

                foreach (var cryptoData in cryptoDataList)
                {
                    string apiId = cryptoData.ApiId;
                    var existingCrypto = await context.Cryptocurrencies.FirstOrDefaultAsync(c => c.ApiId == apiId);

                    if (existingCrypto != null)
                    {
                        existingCrypto.CurrentPrice = cryptoData.CurrentPrice;
                        existingCrypto.MarketCapRank = cryptoData.MarketCapRank;
                        existingCrypto.Volume24h = cryptoData.Volume24h;
                        existingCrypto.Change24h = cryptoData.Change24h;
                        existingCrypto.IconPath = cryptoData.IconPath;

                        Console.WriteLine($"Criptomoneda actualizada: {cryptoData.Name} ({cryptoData.Symbol})");
                    }
                    else
                    {
                        var newCrypto = new Cryptocurrency
                        {
                            ApiId = cryptoData.ApiId,
                            Symbol = cryptoData.Symbol,
                            Name = cryptoData.Name,
                            CurrentPrice = cryptoData.CurrentPrice,
                            MarketCapRank = cryptoData.MarketCapRank,
                            Volume24h = cryptoData.Volume24h,
                            Change24h = cryptoData.Change24h,
                            IconPath = cryptoData.IconPath
                        };

                        context.Cryptocurrencies.Add(newCrypto);
                        Console.WriteLine($"Nueva criptomoneda añadida: {cryptoData.Name} ({cryptoData.Symbol})");
                    }
                }

                await context.SaveChangesAsync();
                Console.WriteLine("Cambios guardados en la base de datos.");
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error de solicitud HTTP al obtener datos de la API de CoinGecko: {httpEx.Message}");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Error al guardar los datos de criptomonedas en la base de datos: {dbEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Error al deserializar la respuesta JSON de la API de CoinGecko: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado durante la sincronización de datos de criptomonedas: {ex.Message}");
            }
        }
    }
}
