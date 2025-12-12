using System;
using System.Collections.Generic;
using Project.Core.Models;
using Project.Data;

namespace Project.Services
{
 public class UserService
 {
 private readonly UserRepository _repo;
 private readonly ActivityLogService _log;
 public UserService(UserRepository repo, ActivityLogService log) { _repo = repo; _log = log; }

 public IEnumerable<User> GetAll() => _repo.GetAll();
 public User? GetById(int id) => _repo.GetById(id);
 public void Create(User u, int? performedBy = null)
 {
 if (string.IsNullOrWhiteSpace(u.Username)) throw new ArgumentException("Username required");
 if (string.IsNullOrWhiteSpace(u.PasswordHash)) throw new ArgumentException("Password required");
 _repo.Add(u);
 // log
 _log.Log(performedBy, "CREATE", "User", null, u.Username);
 }
 public void Update(User u, int? performedBy = null)
 {
 if (string.IsNullOrWhiteSpace(u.Username)) throw new ArgumentException("Username required");
 if (string.IsNullOrWhiteSpace(u.PasswordHash)) throw new ArgumentException("Password required");
 _repo.Update(u);
 _log.Log(performedBy, "UPDATE", "User", u.Id, u.Username);
 }
 public void Delete(int id, int? performedBy = null)
 {
 // Prevent self-deletion
 if (performedBy.HasValue && performedBy.Value == id) throw new InvalidOperationException("Cannot delete currently logged-in user.");
 _repo.Delete(id);
 _log.Log(performedBy, "DELETE", "User", id, null);
 }
 }
}
