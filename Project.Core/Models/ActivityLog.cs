using System;

namespace Project.Core.Models
{
 public class ActivityLog
 {
 public int Id { get; set; }
 public int? UserId { get; set; }
 public string Action { get; set; } = null!; // e.g., CREATE, UPDATE, DELETE, LOGIN
 public string Entity { get; set; } = null!; // e.g., User, Product
 public int? EntityId { get; set; }
 public string? Details { get; set; }
 public DateTime CreatedAt { get; set; }
 }
}