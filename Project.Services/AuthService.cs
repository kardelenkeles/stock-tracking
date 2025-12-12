using System;
using System.Security.Cryptography;
using System.Text;
using Project.Data;
using Project.Core.Models;

namespace Project.Services
{
 public class AuthService
 {
 private readonly UserRepository _userRepo;
 public AuthService(UserRepository userRepo)
 {
 _userRepo = userRepo;
 }

 public User? Authenticate(string username, string password)
 {
 var user = _userRepo.GetByUsername(username);
 if (user == null) return null;
 var hash = HashPassword(password);
 if (user.PasswordHash == hash) return user;
 return null;
 }

 public static string HashPassword(string password)
 {
 using var sha256 = SHA256.Create();
 var bytes = Encoding.UTF8.GetBytes(password);
 var hash = sha256.ComputeHash(bytes);
 return Convert.ToBase64String(hash);
 }

 public void EnsureDefaultAdmin()
 {
 var admin = _userRepo.GetByUsername("admin");
 if (admin == null)
 {
 var user = new User
 {
 Username = "admin",
 PasswordHash = HashPassword("admin123"),
 Role = "Admin"
 };
 _userRepo.Add(user);
 }
 }
 }
}
