namespace Project.Core.Models
{
 public class Product
 {
 public int Id { get; set; }
 public string Name { get; set; } = null!;
 public int? CategoryId { get; set; }
 public int? SupplierId { get; set; }
 public int Quantity { get; set; }
 public int MinQuantity { get; set; }
 public decimal? PurchasePrice { get; set; }
 public decimal? SellPrice { get; set; }
 public string? CreatedAt { get; set; }
 }
}
