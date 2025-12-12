using System;
using Project.Core.Models;
using Project.Data;

namespace Project.Services
{
 public class ActivityLogService
 {
 private readonly ActivityLogRepository _repo;
 public ActivityLogService(ActivityLogRepository repo) => _repo = repo;

 public void Log(int? userId, string action, string entity, int? entityId = null, string? details = null)
 {
 try
 {
 var log = new ActivityLog { UserId = userId, Action = action, Entity = entity, EntityId = entityId, Details = details, CreatedAt = DateTime.Now };
 _repo.Add(log);
 }
 catch { /* do not throw logging errors */ }
 }
 }
}
