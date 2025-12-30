using System;
using System.Collections.Generic;
using System.Linq;
using Project.Core.Models;
using Project.Data;

namespace Project.Services
{
 public class StockService
 {
 private readonly StockMovementRepository _repo;
 private readonly ProductRepository _productRepo;
 private readonly ActivityLogService? _log;
 public StockService(StockMovementRepository repo, ProductRepository productRepo, ActivityLogService? log = null)
 {
 _repo = repo;
 _productRepo = productRepo;
 _log = log;
 }

 public IEnumerable<StockMovement> GetAll() => _repo.GetAll();
 public IEnumerable<StockMovement> GetAll(DateTime? from = null, DateTime? to = null, int? productId = null, string? type = null)
 {
 var list = _repo.GetAll();
 if (from.HasValue) list = list.Where(m => m.MovementDate >= from.Value);
 if (to.HasValue) list = list.Where(m => m.MovementDate <= to.Value);
 if (productId.HasValue) list = list.Where(m => m.ProductId == productId.Value);
 if (!string.IsNullOrWhiteSpace(type)) list = list.Where(m => m.MovementType == type);
 return list;
 }
 public void AddMovement(StockMovement m, User? performedBy = null)
 {

 if (m.Quantity <=0) throw new ArgumentException("Quantity must be greater than zero");

 var product = _productRepo.GetById(m.ProductId);
 if (product == null) throw new InvalidOperationException("Product not found");
 int newQty = product.Quantity + (m.MovementType == "IN" ? m.Quantity : -m.Quantity);
 if (newQty <0) throw new InvalidOperationException("Insufficient stock for this operation");
 _productRepo.UpdateQuantity(product.Id, newQty);
 _repo.Add(m);
 _log?.Log(performedBy?.Id, "STOCK_MOVEMENT", m.MovementType, m.ProductId, $"Qty:{m.Quantity}");
 }
 }
}
