using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using API_cripto.Data;
using API_cripto.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddConsole();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//servicio de caché en memoria
builder.Services.AddMemoryCache();

// Registro de la tarea en segundo plano
builder.Services.AddHostedService<CryptoDataSyncService>();
builder.Services.AddHttpClient<CoinGeckoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
