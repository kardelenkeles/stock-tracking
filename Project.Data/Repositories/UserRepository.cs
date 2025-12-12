using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Project.Core.Models;

namespace Project.Data
{
 public class UserRepository
 {
 private readonly string _connectionString;
 public UserRepository(string connectionString)
 {
 _connectionString = connectionString;
 }

 public User? GetByUsername(string username)
 {
 using var conn = new SqliteConnection(_connectionString);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT Id, Username, PasswordHash, Role FROM Users WHERE Username = @username";
 cmd.Parameters.AddWithValue("@username", username);
 using var rdr = cmd.ExecuteReader();
 if (rdr.Read())
 {
 return new User
 {
 Id = rdr.GetInt32(0),
 Username = rdr.GetString(1),
 PasswordHash = rdr.GetString(2),
 Role = rdr.GetString(3)
 };
 }
 return null;
 }

 public IEnumerable<User> GetAll()
 {
 var list = new List<User>();
 using var conn = new SqliteConnection(_connectionString);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT Id, Username, PasswordHash, Role FROM Users ORDER BY Username";
 using var rdr = cmd.ExecuteReader();
 while (rdr.Read())
 {
 list.Add(new User { Id = rdr.GetInt32(0), Username = rdr.GetString(1), PasswordHash = rdr.GetString(2), Role = rdr.GetString(3) });
 }
 return list;
 }

 public User? GetById(int id)
 {
 using var conn = new SqliteConnection(_connectionString);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT Id, Username, PasswordHash, Role FROM Users WHERE Id = @id";
 cmd.Parameters.AddWithValue("@id", id);
 using var rdr = cmd.ExecuteReader();
 if (rdr.Read())
 {
 return new User { Id = rdr.GetInt32(0), Username = rdr.GetString(1), PasswordHash = rdr.GetString(2), Role = rdr.GetString(3) };
 }
 return null;
 }

 public void Add(User user)
 {
 using var conn = new SqliteConnection(_connectionString);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@u, @p, @r)";
 cmd.Parameters.AddWithValue("@u", user.Username);
 cmd.Parameters.AddWithValue("@p", user.PasswordHash);
 cmd.Parameters.AddWithValue("@r", user.Role);
 cmd.ExecuteNonQuery();
 }

 public void Update(User user)
 {
 using var conn = new SqliteConnection(_connectionString);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "UPDATE Users SET Username=@u, PasswordHash=@p, Role=@r WHERE Id=@id";
 cmd.Parameters.AddWithValue("@u", user.Username);
 cmd.Parameters.AddWithValue("@p", user.PasswordHash);
 cmd.Parameters.AddWithValue("@r", user.Role);
 cmd.Parameters.AddWithValue("@id", user.Id);
 cmd.ExecuteNonQuery();
 }

 public void Delete(int id)
 {
 using var conn = new SqliteConnection(_connectionString);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "DELETE FROM Users WHERE Id=@id";
 cmd.Parameters.AddWithValue("@id", id);
 cmd.ExecuteNonQuery();
 }
 }
}
