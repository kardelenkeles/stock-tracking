using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Project.Core.Models;

namespace Project.Data
{
 public class CategoryRepository
 {
 private readonly string _conn;
 public CategoryRepository(string connectionString) => _conn = connectionString;

 public IEnumerable<Category> GetAll()
 {
 var list = new List<Category>();
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT Id, Name, Description FROM Categories";
 using var rdr = cmd.ExecuteReader();
 while (rdr.Read())
 {
 list.Add(new Category { Id = rdr.GetInt32(0), Name = rdr.GetString(1), Description = rdr.IsDBNull(2)? null : rdr.GetString(2) });
 }
 return list;
 }

 public Category? GetById(int id)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT Id, Name, Description FROM Categories WHERE Id = @id";
 cmd.Parameters.AddWithValue("@id", id);
 using var rdr = cmd.ExecuteReader();
 if (rdr.Read()) return new Category { Id = rdr.GetInt32(0), Name = rdr.GetString(1), Description = rdr.IsDBNull(2)? null : rdr.GetString(2) };
 return null;
 }

 public void Add(Category c)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "INSERT INTO Categories (Name, Description) VALUES (@n, @d)";
 cmd.Parameters.AddWithValue("@n", c.Name);
 cmd.Parameters.AddWithValue("@d", c.Description ?? (object)DBNull.Value);
 cmd.ExecuteNonQuery();
 }

 public void Update(Category c)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "UPDATE Categories SET Name=@n, Description=@d WHERE Id=@id";
 cmd.Parameters.AddWithValue("@n", c.Name);
 cmd.Parameters.AddWithValue("@d", c.Description ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@id", c.Id);
 cmd.ExecuteNonQuery();
 }

 public void Delete(int id)
 {
 using var conn = new SqliteConnection(_conn);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "DELETE FROM Categories WHERE Id=@id";
 cmd.Parameters.AddWithValue("@id", id);
 cmd.ExecuteNonQuery();
 }
 }
}
