using System;

namespace Project.Core.Models
{
 public class ActivityLog
 {
 public int Id { get; set; }
 public int? UserId { get; set; }
 public string Action { get; set; } = null!;
 public string Entity { get; set; } = null!; 
 public int? EntityId { get; set; }
 public string? Details { get; set; }
 public DateTime CreatedAt { get; set; }
 }
}
