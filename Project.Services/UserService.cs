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
 private readonly StockMovementRepository _movementRepo;
 public UserService(UserRepository repo, ActivityLogService log, StockMovementRepository movementRepo) { _repo = repo; _log = log; _movementRepo = movementRepo; }

 public IEnumerable<User> GetAll() => _repo.GetAll();
 public User? GetById(int id) => _repo.GetById(id);
 public void Create(User u, int? performedBy = null)
 {
 if (string.IsNullOrWhiteSpace(u.Username)) throw new ArgumentException("Username required");
 if (string.IsNullOrWhiteSpace(u.PasswordHash)) throw new ArgumentException("Password required");
 _repo.Add(u);

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
 if (performedBy.HasValue && performedBy.Value == id)
 throw new InvalidOperationException("Cannot delete currently logged-in user.");

 var userToDelete = _repo.GetById(id);
 if (userToDelete == null)
 throw new InvalidOperationException("User not found.");

 
 var performingUser = _repo.GetById(performedBy ??0);
 bool isAdmin = performingUser != null && performingUser.Role == "Admin";

 if (!isAdmin)
 {
 var count = _movementRepo.CountByUser(id);
 if (count >0)
 throw new InvalidOperationException("Cannot delete user with associated stock movements.");
 }

 _repo.Delete(id);
 _log.Log(performedBy, "DELETE", "User", id, null);
 }

 public bool HasMovements(int userId)
 {
 return _movementRepo.CountByUser(userId) >0;
 }
 }
}
