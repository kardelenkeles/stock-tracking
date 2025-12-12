using System;

namespace Project.Core.Models
{
 public class StockMovement
 {
 public int Id { get; set; }
 public int ProductId { get; set; }
 public string MovementType { get; set; } = null!; // "IN" or "OUT"
 public int Quantity { get; set; }
 public DateTime MovementDate { get; set; }
 public int? UserId { get; set; }
 public string? Notes { get; set; }
 }
}