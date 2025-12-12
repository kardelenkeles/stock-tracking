using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Project.Core.Models;

namespace Project.Data
{
 public class SupplierRepository
 {
 private readonly string _conn;
 public SupplierRepository(string connectionString) => _conn = connectionString;

 public IEnumerable<Supplier> GetAll()
 {
 var list = new List<Supplier>();
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT Id, Name, ContactName, Phone, Email, Address FROM Suppliers";
 using var rdr = cmd.ExecuteReader();
 while (rdr.Read())
 {
 list.Add(new Supplier
 {
 Id = rdr.GetInt32(0),
 Name = rdr.GetString(1),
 ContactName = rdr.IsDBNull(2)? null : rdr.GetString(2),
 Phone = rdr.IsDBNull(3)? null : rdr.GetString(3),
 Email = rdr.IsDBNull(4)? null : rdr.GetString(4),
 Address = rdr.IsDBNull(5)? null : rdr.GetString(5)
 });
 }
 return list;
 }

 public Supplier? GetById(int id)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT Id, Name, ContactName, Phone, Email, Address FROM Suppliers WHERE Id=@id";
 cmd.Parameters.AddWithValue("@id", id);
 using var rdr = cmd.ExecuteReader();
 if (rdr.Read()) return new Supplier { Id = rdr.GetInt32(0), Name = rdr.GetString(1), ContactName = rdr.IsDBNull(2)? null : rdr.GetString(2), Phone = rdr.IsDBNull(3)? null : rdr.GetString(3), Email = rdr.IsDBNull(4)? null : rdr.GetString(4), Address = rdr.IsDBNull(5)? null : rdr.GetString(5) };
 return null;
 }

 public void Add(Supplier s)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "INSERT INTO Suppliers (Name, ContactName, Phone, Email, Address) VALUES (@n, @c, @p, @e, @a)";
 cmd.Parameters.AddWithValue("@n", s.Name);
 cmd.Parameters.AddWithValue("@c", s.ContactName ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@p", s.Phone ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@e", s.Email ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@a", s.Address ?? (object)DBNull.Value);
 cmd.ExecuteNonQuery();
 }

 public void Update(Supplier s)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "UPDATE Suppliers SET Name=@n, ContactName=@c, Phone=@p, Email=@e, Address=@a WHERE Id=@id";
 cmd.Parameters.AddWithValue("@n", s.Name);
 cmd.Parameters.AddWithValue("@c", s.ContactName ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@p", s.Phone ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@e", s.Email ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@a", s.Address ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@id", s.Id);
 cmd.ExecuteNonQuery();
 }

 public void Delete(int id)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "DELETE FROM Suppliers WHERE Id=@id";
 cmd.Parameters.AddWithValue("@id", id);
 cmd.ExecuteNonQuery();
 }
 }
}
