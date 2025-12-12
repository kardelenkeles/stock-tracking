using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Project.Core.Models;
using System;

namespace Project.Data
{
 public class StockMovementRepository
 {
 private readonly string _conn;
 public StockMovementRepository(string connectionString) => _conn = connectionString;

 public IEnumerable<StockMovement> GetAll()
 {
 var list = new List<StockMovement>();
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT Id, ProductId, MovementType, Quantity, MovementDate, UserId, Notes FROM StockMovements ORDER BY MovementDate DESC";
 using var rdr = cmd.ExecuteReader();
 while (rdr.Read())
 {
 list.Add(new StockMovement
 {
 Id = rdr.GetInt32(0),
 ProductId = rdr.GetInt32(1),
 MovementType = rdr.GetString(2),
 Quantity = rdr.GetInt32(3),
 MovementDate = DateTime.Parse(rdr.GetString(4)),
 UserId = rdr.IsDBNull(5)? (int?)null : rdr.GetInt32(5),
 Notes = rdr.IsDBNull(6)? null : rdr.GetString(6)
 });
 }
 return list;
 }

 public void Add(StockMovement m)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "INSERT INTO StockMovements (ProductId, MovementType, Quantity, MovementDate, UserId, Notes) VALUES (@pid, @mt, @q, @md, @uid, @n)";
 cmd.Parameters.AddWithValue("@pid", m.ProductId);
 cmd.Parameters.AddWithValue("@mt", m.MovementType);
 cmd.Parameters.AddWithValue("@q", m.Quantity);
 cmd.Parameters.AddWithValue("@md", m.MovementDate.ToString("o"));
 cmd.Parameters.AddWithValue("@uid", m.UserId ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@n", m.Notes ?? (object)DBNull.Value);
 cmd.ExecuteNonQuery();
 }
 }
}
