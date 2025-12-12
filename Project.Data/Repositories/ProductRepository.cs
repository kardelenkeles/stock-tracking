using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Project.Core.Models;

namespace Project.Data
{
 public class ProductRepository
 {
 private readonly string _conn;
 public ProductRepository(string connectionString) => _conn = connectionString;

 public IEnumerable<Product> GetAll()
 {
 var list = new List<Product>();
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT Id, Name, CategoryId, SupplierId, Quantity, MinQuantity, PurchasePrice, SellPrice, CreatedAt FROM Products";
 using var rdr = cmd.ExecuteReader();
 while (rdr.Read())
 {
 list.Add(new Product
 {
 Id = rdr.GetInt32(0),
 Name = rdr.GetString(1),
 CategoryId = rdr.IsDBNull(2)? (int?)null : rdr.GetInt32(2),
 SupplierId = rdr.IsDBNull(3)? (int?)null : rdr.GetInt32(3),
 Quantity = rdr.GetInt32(4),
 MinQuantity = rdr.GetInt32(5),
 PurchasePrice = rdr.IsDBNull(6)? (decimal?)null : (decimal)rdr.GetDouble(6),
 SellPrice = rdr.IsDBNull(7)? (decimal?)null : (decimal)rdr.GetDouble(7),
 CreatedAt = rdr.IsDBNull(8)? null : rdr.GetString(8)
 });
 }
 return list;
 }

 public Product? GetById(int id)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT Id, Name, CategoryId, SupplierId, Quantity, MinQuantity, PurchasePrice, SellPrice, CreatedAt FROM Products WHERE Id=@id";
 cmd.Parameters.AddWithValue("@id", id);
 using var rdr = cmd.ExecuteReader();
 if (rdr.Read()) return new Product { Id = rdr.GetInt32(0), Name = rdr.GetString(1), CategoryId = rdr.IsDBNull(2)? (int?)null : rdr.GetInt32(2), SupplierId = rdr.IsDBNull(3)? (int?)null : rdr.GetInt32(3), Quantity = rdr.GetInt32(4), MinQuantity = rdr.GetInt32(5), PurchasePrice = rdr.IsDBNull(6)? (decimal?)null : (decimal)rdr.GetDouble(6), SellPrice = rdr.IsDBNull(7)? (decimal?)null : (decimal)rdr.GetDouble(7), CreatedAt = rdr.IsDBNull(8)? null : rdr.GetString(8) };
 return null;
 }

 public void Add(Product p)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "INSERT INTO Products (Name, CategoryId, SupplierId, Quantity, MinQuantity, PurchasePrice, SellPrice) VALUES (@n, @c, @s, @q, @m, @pp, @sp)";
 cmd.Parameters.AddWithValue("@n", p.Name);
 cmd.Parameters.AddWithValue("@c", p.CategoryId ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@s", p.SupplierId ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@q", p.Quantity);
 cmd.Parameters.AddWithValue("@m", p.MinQuantity);
 cmd.Parameters.AddWithValue("@pp", p.PurchasePrice ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@sp", p.SellPrice ?? (object)DBNull.Value);
 cmd.ExecuteNonQuery();
 }

 public void Update(Product p)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "UPDATE Products SET Name=@n, CategoryId=@c, SupplierId=@s, Quantity=@q, MinQuantity=@m, PurchasePrice=@pp, SellPrice=@sp WHERE Id=@id";
 cmd.Parameters.AddWithValue("@n", p.Name);
 cmd.Parameters.AddWithValue("@c", p.CategoryId ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@s", p.SupplierId ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@q", p.Quantity);
 cmd.Parameters.AddWithValue("@m", p.MinQuantity);
 cmd.Parameters.AddWithValue("@pp", p.PurchasePrice ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@sp", p.SellPrice ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@id", p.Id);
 cmd.ExecuteNonQuery();
 }

 public void Delete(int id)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "DELETE FROM Products WHERE Id=@id";
 cmd.Parameters.AddWithValue("@id", id);
 cmd.ExecuteNonQuery();
 }

 public void UpdateQuantity(int productId, int newQuantity)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "UPDATE Products SET Quantity=@q WHERE Id=@id";
 cmd.Parameters.AddWithValue("@q", newQuantity);
 cmd.Parameters.AddWithValue("@id", productId);
 cmd.ExecuteNonQuery();
 }

 public int CountByCategory(int categoryId)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT COUNT(1) FROM Products WHERE CategoryId = @cid";
 cmd.Parameters.AddWithValue("@cid", categoryId);
 return Convert.ToInt32(cmd.ExecuteScalar());
 }

 public string? GetNameById(int id)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT Name FROM Products WHERE Id=@id";
 cmd.Parameters.AddWithValue("@id", id);
 var r = cmd.ExecuteScalar();
 return r == null || r == DBNull.Value ? null : r.ToString();
 }

 public int CountBySupplier(int supplierId)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT COUNT(1) FROM Products WHERE SupplierId = @sid";
 cmd.Parameters.AddWithValue("@sid", supplierId);
 return Convert.ToInt32(cmd.ExecuteScalar());
 }
 }
}
