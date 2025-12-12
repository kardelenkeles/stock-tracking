using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using Project.Core.Models;
using Project.Services;

namespace Project.Reports
{
 public class ReportService
 {
 private readonly StockService _stockService;
 private readonly ProductService _productService;
 public ReportService(StockService stockService, ProductService productService)
 {
 _stockService = stockService;
 _productService = productService;
 }

 public void ExportCriticalStockToExcel(string filePath)
 {
 var products = _productService.GetAll().Where(p => p.Quantity <= p.MinQuantity).ToList();
 using var wb = new XLWorkbook();
 var ws = wb.Worksheets.Add("CriticalStock");
 ws.Cell(1,1).Value = "Id"; ws.Cell(1,2).Value = "Name"; ws.Cell(1,3).Value = "Quantity"; ws.Cell(1,4).Value = "MinQuantity"; ws.Cell(1,5).Value = "CategoryId"; ws.Cell(1,6).Value = "SupplierId";
 int r =2;
 foreach(var p in products)
 {
 ws.Cell(r,1).Value = p.Id;
 ws.Cell(r,2).Value = p.Name;
 ws.Cell(r,3).Value = p.Quantity;
 ws.Cell(r,4).Value = p.MinQuantity;
 ws.Cell(r,5).Value = p.CategoryId;
 ws.Cell(r,6).Value = p.SupplierId;
 r++;
 }
 wb.SaveAs(filePath);
 }

 public void ExportCriticalStockToPdf(string filePath)
 {
 var products = _productService.GetAll().Where(p => p.Quantity <= p.MinQuantity).ToList();
 using var doc = new PdfDocument();
 var page = doc.AddPage();
 var gfx = XGraphics.FromPdfPage(page);
 var font = new XFont("Verdana",10);
 double y =20;
 gfx.DrawString("Critical Stock Report", new XFont("Verdana",14, XFontStyle.Bold), XBrushes.Black, new XRect(0, y, page.Width,20), XStringFormats.TopCenter);
 y +=30;
 foreach(var p in products)
 {
 var line = $"{p.Id} - {p.Name} - Qty: {p.Quantity} - Min: {p.MinQuantity}";
 gfx.DrawString(line, font, XBrushes.Black, new XRect(20, y, page.Width-40,20), XStringFormats.TopLeft);
 y +=20;
 if (y > page.Height -40) { page = doc.AddPage(); gfx = XGraphics.FromPdfPage(page); y =20; }
 }
 doc.Save(filePath);
 }

 public IEnumerable<(Product product,int totalOut)> GetTopConsumedProducts(int top =10)
 {
 var outs = _stockService.GetAll().Where(m => m.MovementType == "OUT");
 var groups = outs.GroupBy(m => m.ProductId).Select(g => (Product: _productService.GetById(g.Key), Total: g.Sum(x => x.Quantity))).Where(x => x.Product != null).OrderByDescending(x => x.Total).Take(top).Select(x => (x.Product!, x.Total));
 return groups;
 }

 public void ExportTopConsumedToExcel(string filePath, int top =10)
 {
 var topList = GetTopConsumedProducts(top).ToList();
 using var wb = new XLWorkbook();
 var ws = wb.Worksheets.Add("TopConsumed");
 ws.Cell(1,1).Value = "ProductId"; ws.Cell(1,2).Value = "Name"; ws.Cell(1,3).Value = "TotalOut";
 int r =2;
 foreach(var t in topList)
 {
 ws.Cell(r,1).Value = t.product.Id;
 ws.Cell(r,2).Value = t.product.Name;
 ws.Cell(r,3).Value = t.totalOut;
 r++;
 }
 wb.SaveAs(filePath);
 }

 public void ExportTopConsumedToPdf(string filePath, int top =10)
 {
 var topList = GetTopConsumedProducts(top).ToList();
 using var doc = new PdfDocument();
 var page = doc.AddPage();
 var gfx = XGraphics.FromPdfPage(page);
 var font = new XFont("Verdana",10);
 double y =20;
 gfx.DrawString("Top Consumed Products", new XFont("Verdana",14, XFontStyle.Bold), XBrushes.Black, new XRect(0, y, page.Width,20), XStringFormats.TopCenter);
 y +=30;
 foreach(var t in topList)
 {
 var line = $"{t.product.Id} - {t.product.Name} - Total OUT: {t.totalOut}";
 gfx.DrawString(line, font, XBrushes.Black, new XRect(20, y, page.Width-40,20), XStringFormats.TopLeft);
 y +=20;
 if (y > page.Height -40) { page = doc.AddPage(); gfx = XGraphics.FromPdfPage(page); y =20; }
 }
 doc.Save(filePath);
 }
 }
}
