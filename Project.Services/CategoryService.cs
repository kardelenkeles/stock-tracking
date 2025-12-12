using System;
using System.Collections.Generic;
using System.Linq;
using Project.Core.Models;
using Project.Data;

namespace Project.Services
{
 public class CategoryService
 {
 private readonly CategoryRepository _repo;
 private readonly ProductRepository _productRepo;
 private readonly ActivityLogService? _log;
 public CategoryService(CategoryRepository repo, ProductRepository productRepo, ActivityLogService? log = null) { _repo = repo; _productRepo = productRepo; _log = log; }

 public IEnumerable<Category> GetAll(string? search = null, int page =1, int pageSize =50)
 {
 var list = _repo.GetAll();
 if (!string.IsNullOrWhiteSpace(search)) list = list.Where(c => c.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || (c.Description ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase));
 return list.Skip((page-1)*pageSize).Take(pageSize);
 }
 public Category? GetById(int id) => _repo.GetById(id);
 public void Create(Category c, Project.Core.Models.User? performedBy = null)
 {
 if (performedBy == null || performedBy.Role != "Admin") throw new UnauthorizedAccessException("Only Admins can create categories.");
 if (string.IsNullOrWhiteSpace(c.Name)) throw new ArgumentException("Category name is required");
 _repo.Add(c);
 _log?.Log(performedBy?.Id, "CREATE", "Category", null, c.Name);
 }
 public void Update(Category c, Project.Core.Models.User? performedBy = null)
 {
 if (performedBy == null || performedBy.Role != "Admin") throw new UnauthorizedAccessException("Only Admins can update categories.");
 if (string.IsNullOrWhiteSpace(c.Name)) throw new ArgumentException("Category name is required");
 _repo.Update(c);
 _log?.Log(performedBy?.Id, "UPDATE", "Category", c.Id, c.Name);
 }
 public void Delete(int id, Project.Core.Models.User? performedBy = null)
 {
 if (performedBy == null || performedBy.Role != "Admin") throw new UnauthorizedAccessException("Only Admins can delete categories.");
 var count = _productRepo.CountByCategory(id);
 if (count >0) throw new InvalidOperationException("Cannot delete category with associated products.");
 _repo.Delete(id);
 _log?.Log(performedBy?.Id, "DELETE", "Category", id, null);
 }
 }
}
