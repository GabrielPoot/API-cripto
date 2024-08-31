using System.ComponentModel.DataAnnotations;

public class AddFavoriteDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public CryptocurrencyDto Cryptocurrency { get; set; }
}

public class CryptocurrencyDto
{
    public int Id { get; set; } 
    public string ApiId { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public double CurrentPrice { get; set; }
    public int MarketCapRank { get; set; }
    public double Volume24h { get; set; }
    public double Change24h { get; set; }
    public string IconPath { get; set; }
}
