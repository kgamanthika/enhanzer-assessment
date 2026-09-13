using System.ComponentModel.DataAnnotations;

namespace Enhanzer.Api.DTOs;

public class PurchaseBillRequest
{
    [Required(ErrorMessage = "Item is required.")]
    public string ItemName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Batch is required.")]
    public string BatchName { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Standard cost must be greater than 0.")]
    public decimal StandardCost { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Standard price must be greater than 0.")]
    public decimal StandardPrice { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
    public decimal DiscountPercentage { get; set; }
}