using System;
using System.Collections.Generic;
using Project.Core.Models;
using Project.Data;

namespace Project.Services
{
 public class ProductService
 {
 private readonly ProductRepository _repo;
 private readonly StockMovementRepository _movementRepo;
 private readonly ActivityLogService? _log;
 public ProductService(ProductRepository repo, StockMovementRepository movementRepo, ActivityLogService? log = null) => (_repo, _movementRepo, _log) = (repo, movementRepo, log);

 public IEnumerable<Product> GetAll() => _repo.GetAll();
 public Product? GetById(int id) => _repo.GetById(id);
 public void Create(Product p, User? performedBy = null)
 {
 if (performedBy != null && performedBy.Role != "Admin") throw new UnauthorizedAccessException("Only Admins can create products.");
 if (string.IsNullOrWhiteSpace(p.Name)) throw new ArgumentException("Product name is required");
 if (p.Quantity <0) throw new ArgumentException("Quantity cannot be negative");
 if (p.MinQuantity <0) throw new ArgumentException("MinQuantity cannot be negative");
 if (p.PurchasePrice <0) throw new ArgumentException("PurchasePrice cannot be negative");
 if (p.SellPrice <0) throw new ArgumentException("SellPrice cannot be negative");
 _repo.Add(p);
 _log?.Log(performedBy?.Id, "CREATE", "Product", null, p.Name);
 }

 public void Update(Product p, User? performedBy = null)
 {
 if (performedBy != null && performedBy.Role != "Admin") throw new UnauthorizedAccessException("Only Admins can update products.");
 if (string.IsNullOrWhiteSpace(p.Name)) throw new ArgumentException("Product name is required");
 _repo.Update(p);
 _log?.Log(performedBy?.Id, "UPDATE", "Product", p.Id, p.Name);
 }

 public void Delete(int id, User? performedBy = null)
 {
 if (performedBy != null && performedBy.Role != "Admin") throw new UnauthorizedAccessException("Only Admins can delete products.");
 var moves = _movementRepo.CountByProduct(id);
 if (moves >0) throw new InvalidOperationException("Cannot delete product with stock movements.");
 _repo.Delete(id);
 _log?.Log(performedBy?.Id, "DELETE", "Product", id, null);
 }
 public void UpdateQuantity(int productId, int newQuantity) => _repo.UpdateQuantity(productId, newQuantity);


 public bool HasMovements(int productId)
 {
 return _movementRepo.CountByProduct(productId) >0;
 }
 }
}
