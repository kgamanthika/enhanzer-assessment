namespace Enhanzer.Api.Models;

public class PurchaseBill
{
    public int Id { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public string BatchName { get; set; } = string.Empty;

    public decimal StandardCost { get; set; }

    public decimal StandardPrice { get; set; }

    public int Quantity { get; set; }

    public decimal DiscountPercentage { get; set; }

    public decimal TotalCost { get; set; }

    public decimal TotalSelling { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}