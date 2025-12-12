using System;
using System.Linq;
using System.Windows.Forms;
using Project.Services;
using System.Drawing;
using System.Collections.Generic;

namespace Project.UI.Forms
{
 public class DashboardControl : UserControl
 {
 private readonly CategoryService _catService;
 private readonly ProductService _prodService;
 private readonly StockService _stockService;
 private Label lblTotalProducts = new Label();
 private Label lblTotalCategories = new Label();
 private Label lblTodayMovements = new Label();
 private Label lblCriticalCount = new Label();
 private ListBox lstCritical = new ListBox();
 private Panel pnlBar = new Panel();
 private Panel pnlPie = new Panel();
 private Button btnRefresh = new Button();

 // cached data for drawing
 private (string name,int value)[] _barData = Array.Empty<(string,int)>();
 private (string name,double value)[] _pieData = Array.Empty<(string,double)>();

 public DashboardControl(CategoryService catService, ProductService prodService, StockService stockService)
 {
 _catService = catService;
 _prodService = prodService;
 _stockService = stockService;
 InitializeComponents();
 LoadData();
 }

 private void InitializeComponents()
 {
 this.Dock = DockStyle.Fill;
 lblTotalProducts.Left =20; lblTotalProducts.Top =20; lblTotalProducts.Width =300; lblTotalProducts.Font = new Font(FontFamily.GenericSansSerif,12, FontStyle.Bold);
 lblTotalCategories.Left =20; lblTotalCategories.Top =60; lblTotalCategories.Width =300; lblTotalCategories.Font = new Font(FontFamily.GenericSansSerif,12, FontStyle.Bold);
 lblTodayMovements.Left =20; lblTodayMovements.Top =100; lblTodayMovements.Width =300; lblTodayMovements.Font = new Font(FontFamily.GenericSansSerif,12, FontStyle.Bold);
 lblCriticalCount.Left =20; lblCriticalCount.Top =140; lblCriticalCount.Width =300; lblCriticalCount.Font = new Font(FontFamily.GenericSansSerif,12, FontStyle.Bold);
 lstCritical.Left =350; lstCritical.Top =20; lstCritical.Width =400; lstCritical.Height =300;

 pnlBar.Left =20; pnlBar.Top =200; pnlBar.Width =450; pnlBar.Height =250; pnlBar.BorderStyle = BorderStyle.FixedSingle; pnlBar.Paint += PnlBar_Paint;
 pnlPie.Left =500; pnlPie.Top =200; pnlPie.Width =250; pnlPie.Height =250; pnlPie.BorderStyle = BorderStyle.FixedSingle; pnlPie.Paint += PnlPie_Paint;

 btnRefresh.Left =20; btnRefresh.Top =460; btnRefresh.Text = "Refresh"; btnRefresh.Click += (s,e)=> LoadData();

 this.Controls.Add(lblTotalProducts); this.Controls.Add(lblTotalCategories); this.Controls.Add(lblTodayMovements); this.Controls.Add(lblCriticalCount); this.Controls.Add(lstCritical);
 this.Controls.Add(pnlBar); this.Controls.Add(pnlPie); this.Controls.Add(btnRefresh);
 }

 private void LoadData()
 {
 // products and categories
 var products = _prodService.GetAll().ToList();
 lblTotalProducts.Text = $"Total Products: {products.Count}";
 var cats = _catService.GetAll().ToList();
 lblTotalCategories.Text = $"Total Categories: {cats.Count}";

 // today's movements
 var today = DateTime.Today;
 var movements = _stockService.GetAll().Where(m => m.MovementDate >= today && m.MovementDate < today.AddDays(1)).ToList();
 lblTodayMovements.Text = $"Today's Movements: {movements.Count}";

 // critical products
 var critical = products.Where(p => p.Quantity <= p.MinQuantity).ToList();
 lblCriticalCount.Text = $"Critical Products: {critical.Count}";
 lstCritical.Items.Clear();
 foreach(var c in critical) lstCritical.Items.Add($"{c.Name} (Qty: {c.Quantity}, Min: {c.MinQuantity})");

 // bar data: number of critical products per category (top categories)
 var barGroups = critical.GroupBy(p => p.CategoryId).Select(g => (Name: cats.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "Uncategorized", Count: g.Count())).OrderByDescending(x=>x.Count).Take(10).ToArray();
 _barData = barGroups.Select(b => (b.Name, b.Count)).ToArray();
 pnlBar.Invalidate();

 // pie data: today's IN vs OUT by quantity
 var inQty = movements.Where(m => m.MovementType == "IN").Sum(m => m.Quantity);
 var outQty = movements.Where(m => m.MovementType == "OUT").Sum(m => m.Quantity);
 var total = inQty + outQty;
 if (total ==0)
 {
 _pieData = Array.Empty<(string,double)>();
 }
 else
 {
 _pieData = new (string,double)[] { ("IN", inQty), ("OUT", outQty) };
 }
 pnlPie.Invalidate();
 }

 private void PnlBar_Paint(object? sender, PaintEventArgs e)
 {
 var g = e.Graphics;
 g.Clear(Color.White);
 if (_barData == null || _barData.Length ==0) { g.DrawString("No critical data", this.Font, Brushes.Black,10,10); return; }
 int w = pnlBar.ClientSize.Width -40;
 int h = pnlBar.ClientSize.Height -60;
 int count = _barData.Length;
 int spacing =10;
 int barWidth = Math.Max(20, (w - (count+1)*spacing) / count);
 int max = _barData.Max(x => x.value);
 if (max ==0) max =1;
 for (int i=0;i<count;i++)
 {
 var item = _barData[i];
 int barH = (int)((item.value / (double)max) * h);
 int x =20 + spacing + i*(barWidth + spacing);
 int y =20 + (h - barH);
 g.FillRectangle(Brushes.SteelBlue, x, y, barWidth, barH);
 g.DrawRectangle(Pens.Black, x, y, barWidth, barH);
 g.DrawString(item.name, new Font(this.Font.FontFamily,8), Brushes.Black, x,25 + h);
 g.DrawString(item.value.ToString(), new Font(this.Font.FontFamily,8), Brushes.Black, x, y -14);
 }
 }

 private void PnlPie_Paint(object? sender, PaintEventArgs e)
 {
 var g = e.Graphics;
 g.Clear(Color.White);
 if (_pieData == null || _pieData.Length ==0) { g.DrawString("No movements today", this.Font, Brushes.Black,10,10); return; }
 int cx = pnlPie.ClientSize.Width/2;
 int cy = pnlPie.ClientSize.Height/2;
 int radius = Math.Min(pnlPie.ClientSize.Width, pnlPie.ClientSize.Height)/2 -20;
 double total = _pieData.Sum(p=>p.value);
 double startAngle =0;
 var colors = new Color[] { Color.MediumSeaGreen, Color.IndianRed, Color.SteelBlue, Color.Goldenrod };
 for (int i=0;i<_pieData.Length;i++)
 {
 var item = _pieData[i];
 double sweep = (item.value/total)*360.0;
 var rect = new Rectangle(cx - radius, cy - radius, radius*2, radius*2);
 using var brush = new SolidBrush(colors[i % colors.Length]);
 g.FillPie(brush, rect, (float)startAngle, (float)sweep);
 g.DrawPie(Pens.Black, rect, (float)startAngle, (float)sweep);
 // label
 double mid = startAngle + sweep/2.0;
 double rad = (radius*0.6) * Math.PI/180.0;
 float lx = cx + (float)((radius*0.7) * Math.Cos(mid * Math.PI/180.0));
 float ly = cy + (float)((radius*0.7) * Math.Sin(mid * Math.PI/180.0));
 g.DrawString($"{item.name} ({item.value})", new Font(this.Font.FontFamily,8), Brushes.Black, lx, ly);
 startAngle += sweep;
 }
 }
 }
}
