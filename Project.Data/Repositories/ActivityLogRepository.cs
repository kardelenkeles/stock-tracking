using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Project.Core.Models;

namespace Project.Data
{
 public class ActivityLogRepository
 {
 private readonly string _conn;
 public ActivityLogRepository(string connectionString) => _conn = connectionString;

 public void Add(ActivityLog log)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "INSERT INTO ActivityLogs (UserId, Action, Entity, EntityId, Details, CreatedAt) VALUES (@uid, @act, @ent, @eid, @det, @cat)";
 cmd.Parameters.AddWithValue("@uid", log.UserId ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@act", log.Action);
 cmd.Parameters.AddWithValue("@ent", log.Entity);
 cmd.Parameters.AddWithValue("@eid", log.EntityId ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@det", log.Details ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@cat", log.CreatedAt.ToString("o"));
 cmd.ExecuteNonQuery();
 }

 public IEnumerable<ActivityLog> GetAll(int page =1, int pageSize =50)
 {
 var list = new List<ActivityLog>();
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT Id, UserId, Action, Entity, EntityId, Details, CreatedAt FROM ActivityLogs ORDER BY CreatedAt DESC LIMIT @lim OFFSET @off";
 cmd.Parameters.AddWithValue("@lim", pageSize);
 cmd.Parameters.AddWithValue("@off", (page-1)*pageSize);
 using var rdr = cmd.ExecuteReader();
 while (rdr.Read())
 {
 list.Add(new ActivityLog { Id = rdr.GetInt32(0), UserId = rdr.IsDBNull(1)? (int?)null : rdr.GetInt32(1), Action = rdr.GetString(2), Entity = rdr.GetString(3), EntityId = rdr.IsDBNull(4)? (int?)null : rdr.GetInt32(4), Details = rdr.IsDBNull(5)? null : rdr.GetString(5), CreatedAt = DateTime.Parse(rdr.GetString(6)) });
 }
 return list;
 }
 }
}
