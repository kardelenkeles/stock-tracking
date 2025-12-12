using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Project.Core.Models;
using Project.Data;

namespace Project.Services
{
 public class SupplierService
 {
 private readonly SupplierRepository _repo;
 private readonly ProductRepository _productRepo;
 private readonly ActivityLogService? _log;
 public SupplierService(SupplierRepository repo, ProductRepository productRepo, ActivityLogService? log) { _repo = repo; _productRepo = productRepo; _log = log; }

 public IEnumerable<Supplier> GetAll() => _repo.GetAll();
 public Supplier? GetById(int id) => _repo.GetById(id);
 public void Create(Supplier s, Project.Core.Models.User? performedBy = null)
 {
 if (performedBy == null || performedBy.Role != "Admin") throw new UnauthorizedAccessException("Only Admins can create suppliers.");
 if (string.IsNullOrWhiteSpace(s.Name)) throw new ArgumentException("Supplier name is required");
 if (!string.IsNullOrWhiteSpace(s.Email) && !IsValidEmail(s.Email)) throw new ArgumentException("Invalid email format");
 if (!string.IsNullOrWhiteSpace(s.Phone) && !IsValidPhone(s.Phone)) throw new ArgumentException("Invalid phone format");
 _repo.Add(s);
 _log?.Log(performedBy?.Id, "CREATE", "Supplier", null, s.Name);
 }
 public void Update(Supplier s, Project.Core.Models.User? performedBy = null)
 {
 if (performedBy == null || performedBy.Role != "Admin") throw new UnauthorizedAccessException("Only Admins can update suppliers.");
 if (string.IsNullOrWhiteSpace(s.Name)) throw new ArgumentException("Supplier name is required");
 if (!string.IsNullOrWhiteSpace(s.Email) && !IsValidEmail(s.Email)) throw new ArgumentException("Invalid email format");
 if (!string.IsNullOrWhiteSpace(s.Phone) && !IsValidPhone(s.Phone)) throw new ArgumentException("Invalid phone format");
 _repo.Update(s);
 _log?.Log(performedBy?.Id, "UPDATE", "Supplier", s.Id, s.Name);
 }
 public void Delete(int id, Project.Core.Models.User? performedBy = null)
 {
 if (performedBy == null || performedBy.Role != "Admin") throw new UnauthorizedAccessException("Only Admins can delete suppliers.");
 var count = _productRepo.CountBySupplier(id);
 if (count >0) throw new InvalidOperationException("Cannot delete supplier with associated products.");
 _repo.Delete(id);
 _log?.Log(performedBy?.Id, "DELETE", "Supplier", id, null);
 }

 private static bool IsValidEmail(string email)
 {
 if (string.IsNullOrWhiteSpace(email)) return false;
 // simple RFC-like pattern
 try
 {
 var pattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
 return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
 }
 catch { return false; }
 }

 private static bool IsValidPhone(string phone)
 {
 if (string.IsNullOrWhiteSpace(phone)) return false;
 // Allow digits, spaces, hyphens, parentheses and leading +; require at least7 digits
 var cleaned = Regex.Replace(phone, "[^0-9]", "");
 if (cleaned.Length <7 || cleaned.Length >15) return false;
 var pattern = @"^\+?[0-9\- ()]+$";
 return Regex.IsMatch(phone, pattern);
 }
 }
}
