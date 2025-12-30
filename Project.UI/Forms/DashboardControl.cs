using System;
using System.Linq;
using System.Windows.Forms;
using Project.Services;
using System.Drawing;
using System.Collections.Generic;
using Project.Core.Models;

namespace Project.UI.Forms
{
 public class DashboardControl : UserControl
 {
 private readonly CategoryService _catService;
 private readonly ProductService _prodService;
 private readonly StockService _stockService;
 private readonly SupplierService _supService; 

 private Label lblTotalProducts = new Label();
 private Label lblTotalCategories = new Label();
 private Label lblTodayMovements = new Label();
 private Label lblCriticalCount = new Label();
 private Button btnRefresh = new Button();

 private Panel pnlBar = new Panel();
 private Panel pnlPie = new Panel();
 private ListBox lstCritical = new ListBox();


 private (string name,int value)[] _barData = Array.Empty<(string,int)>();
 private (string name,double value)[] _pieData = Array.Empty<(string,double)>();

 public DashboardControl(CategoryService catService, ProductService prodService, StockService stockService, SupplierService supService)
 {
 _catService = catService;
 _prodService = prodService;
 _stockService = stockService;
 _supService = supService; 
 InitializeComponents();
 LoadData();
 }

 private void InitializeComponents()
 {
 this.Dock = DockStyle.Fill;
 this.Padding = new Padding(12);

 lblTotalProducts.Left =12; lblTotalProducts.Top =12; lblTotalProducts.AutoSize = true; lblTotalProducts.Font = new Font(FontFamily.GenericSansSerif,11, FontStyle.Bold);
 lblTotalCategories.Left =220; lblTotalCategories.Top =12; lblTotalCategories.AutoSize = true; lblTotalCategories.Font = new Font(FontFamily.GenericSansSerif,11, FontStyle.Bold);
 lblTodayMovements.Left =460; lblTodayMovements.Top =12; lblTodayMovements.AutoSize = true; lblTodayMovements.Font = new Font(FontFamily.GenericSansSerif,11, FontStyle.Bold);
 lblCriticalCount.Left =680; lblCriticalCount.Top =12; lblCriticalCount.AutoSize = true; lblCriticalCount.Font = new Font(FontFamily.GenericSansSerif,11, FontStyle.Bold);

 btnRefresh.Text = "Refresh"; btnRefresh.Left =860; btnRefresh.Top =8; btnRefresh.Width =100; btnRefresh.Click += (s,e) => LoadData();

 pnlBar.Left =12; pnlBar.Top =48; pnlBar.Width = this.ClientSize.Width/2 -24; pnlBar.Height = this.ClientSize.Height -120; pnlBar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left; pnlBar.BorderStyle = BorderStyle.FixedSingle; pnlBar.Paint += PnlBar_Paint; pnlBar.Resize += (s,e)=> pnlBar.Invalidate();

 
 pnlPie.Left = pnlBar.Right +16; pnlPie.Top =48; pnlPie.Width = this.ClientSize.Width - pnlBar.Width -48; pnlPie.Height = (this.ClientSize.Height -120)/2 -8; pnlPie.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left; pnlPie.BorderStyle = BorderStyle.FixedSingle; pnlPie.Paint += PnlPie_Paint; pnlPie.Resize += (s,e)=> pnlPie.Invalidate();

 
 lstCritical.Left = pnlPie.Left; lstCritical.Top = pnlPie.Bottom +8; lstCritical.Width = pnlPie.Width; lstCritical.Height = this.ClientSize.Height - lstCritical.Top -20; lstCritical.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

 this.Controls.Add(lblTotalProducts); this.Controls.Add(lblTotalCategories); this.Controls.Add(lblTodayMovements); this.Controls.Add(lblCriticalCount);
 this.Controls.Add(btnRefresh);
 this.Controls.Add(pnlBar); this.Controls.Add(pnlPie); this.Controls.Add(lstCritical);

 this.Resize += (s,e) => LayoutPanels();
 }

 private void LayoutPanels()
 {
 pnlBar.Width = Math.Max(250, (this.ClientSize.Width -60) /2); 
 pnlBar.Height = this.ClientSize.Height -140; 
 pnlPie.Left = pnlBar.Right +16;
 pnlPie.Width = this.ClientSize.Width - pnlPie.Left -12;
 pnlPie.Height = Math.Max(180, (this.ClientSize.Height -140)/2 -8); 
 lstCritical.Left = pnlPie.Left;
 lstCritical.Top = pnlPie.Bottom +8;
 lstCritical.Width = pnlPie.Width;
 lstCritical.Height = this.ClientSize.Height - lstCritical.Top -20;
 pnlBar.Invalidate(); pnlPie.Invalidate();
 }

 private void LoadData()
 {
 var products = _prodService.GetAll().ToList();
 var cats = _catService.GetAll().ToList();
 var suppliers = _supService.GetAll().ToList(); 

 lblTotalProducts.Text = $"Total Products: {products.Count}";
 lblTotalCategories.Text = $"Total Categories: {cats.Count}";

 var today = DateTime.Today;
 var movements = _stockService.GetAll().Where(m => m.MovementDate >= today && m.MovementDate < today.AddDays(1)).ToList();
 lblTodayMovements.Text = $"Today's Movements: {movements.Count}";

 var critical = products.Where(p => p.Quantity <= p.MinQuantity).ToList();
 lblCriticalCount.Text = $"Critical Products: {critical.Count}";

 lstCritical.Items.Clear();
 lstCritical.DrawMode = DrawMode.OwnerDrawFixed; 
 lstCritical.ItemHeight =20; 
 lstCritical.DrawItem += (s, e) =>
 {
 e.DrawBackground();
 var item = lstCritical.Items[e.Index].ToString();
 var isEven = e.Index %2 ==0;
 e.Graphics.FillRectangle(new SolidBrush(isEven ? Color.LightGray : Color.White), e.Bounds);
 e.Graphics.DrawString(item, e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
 e.DrawFocusRectangle();
 };

 foreach (var c in critical)
 {
 var category = cats.FirstOrDefault(cat => cat.Id == c.CategoryId)?.Name ?? "Uncategorized";
 var supplier = suppliers.FirstOrDefault(sup => sup.Id == c.SupplierId)?.Name ?? "Unknown Supplier";
 lstCritical.Items.Add($"{c.Name} (Qty: {c.Quantity}, Min: {c.MinQuantity}, Category: {category}, Supplier: {supplier})");
 }

 var barGroups = critical.GroupBy(p => p.CategoryId)
 .Select(g => (Name: cats.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "Uncategorized", Count: g.Count()))
 .OrderByDescending(x => x.Count)
 .Take(10)
 .ToArray();
 _barData = barGroups.Select(b => (b.Name, b.Count)).ToArray();

 var inQty = movements.Where(m => m.MovementType == "IN").Sum(m => m.Quantity);
 var outQty = movements.Where(m => m.MovementType == "OUT").Sum(m => m.Quantity);
 if (inQty + outQty ==0)
 _pieData = Array.Empty<(string, double)>();
 else
 _pieData = new (string, double)[] { ("IN", inQty), ("OUT", outQty) };

 pnlBar.Invalidate();
 pnlPie.Invalidate();
 }

 private void PnlBar_Paint(object? sender, PaintEventArgs e)
 {
 var g = e.Graphics;
 g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
 g.Clear(Color.White);

 using (var titleFont = new Font(this.Font.FontFamily,12, FontStyle.Bold))
 g.DrawString("Critical Products by Category", titleFont, Brushes.Black,8,6);

 if (_barData == null || _barData.Length ==0)
 {
 g.DrawString("No data", this.Font, Brushes.Gray,10,40);
 return;
 }

 int marginLeft =80; 
 int marginBottom =100; 
 int marginTop =40; 
 int marginRight =20; 

 int w = pnlBar.ClientSize.Width - marginLeft - marginRight;
 int h = pnlBar.ClientSize.Height - marginBottom - marginTop;
 int count = _barData.Length;
 int spacing =16; 
 int barWidth = Math.Max(20, (w - (count +1) * spacing) / Math.Max(1, count));
 int max = _barData.Max(x => x.value);
 if (max ==0) max =1;

 
 using (var gridPen = new Pen(Color.LightGray,1.5f)) 
 {
 for (int i =0; i <=5; i++)
 {
 int y = marginTop + i * (h /5);
 g.DrawLine(gridPen, marginLeft, y, marginLeft + w, y);
 }
 }

 
 var barColors = new Color[] { Color.MediumSlateBlue, Color.Coral, Color.MediumSeaGreen, Color.Goldenrod, Color.SteelBlue };

 
 for (int i =0; i < count; i++)
 {
 var item = _barData[i];
 int barH = (int)((item.value / (double)max) * h);
 int x = marginLeft + spacing + i * (barWidth + spacing);
 int y = marginTop + (h - barH);
 var rect = new Rectangle(x, y, barWidth, barH);

 using (var barBrush = new SolidBrush(barColors[i % barColors.Length]))
 g.FillRectangle(barBrush, rect);
 g.DrawRectangle(Pens.Black, rect);

 
 var valueStr = item.value.ToString();
 using (var valueFont = new Font(this.Font.FontFamily,9))
 {
 var valueSize = g.MeasureString(valueStr, valueFont);
 g.DrawString(valueStr, valueFont, Brushes.Black, x + (barWidth - valueSize.Width) /2, y - valueSize.Height -2);
 }

 
 var label = item.name;
 using (var labelFont = new Font(this.Font.FontFamily,8))
 {
 var labelSize = g.MeasureString(label, labelFont);
 var labelX = x + barWidth /2;
 var labelY = marginTop + h +10;
 g.TranslateTransform(labelX, labelY);
 g.RotateTransform(-45); 
 g.DrawString(label, labelFont, Brushes.Black,0,0);
 g.ResetTransform();
 }
 }

 
 HashSet<int> drawnValues = new HashSet<int>(); 
 for (int i =0; i <=5; i++)
 {
 int y = marginTop + i * (h /5);
 int value = (int)Math.Round(max * (1 - i /5.0));
 if (!drawnValues.Contains(value)) 
 {
 drawnValues.Add(value);
 using (var axisFont = new Font(this.Font.FontFamily,8))
 {
 var valueSize = g.MeasureString(value.ToString(), axisFont);
 g.DrawString(value.ToString(), axisFont, Brushes.Black, marginLeft - valueSize.Width -6, y - valueSize.Height /2);
 }
 }
 }
 }

 private void PnlPie_Paint(object? sender, PaintEventArgs e)
 {
 var g = e.Graphics; g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
 g.Clear(Color.White);
 using(var f = new Font(this.Font.FontFamily,12, FontStyle.Bold)) g.DrawString("Today's IN vs OUT", f, Brushes.Black,8,6);
 if (_pieData == null || _pieData.Length ==0) { g.DrawString("No movements today", this.Font, Brushes.Gray,10,40); return; }
 int cx = pnlPie.ClientSize.Width/2; int cy = pnlPie.ClientSize.Height/2 +10; int radius = Math.Min(pnlPie.ClientSize.Width, pnlPie.ClientSize.Height)/2 -40; double total = _pieData.Sum(p=>p.value); double startAngle =0; var colors = new Color[]{ Color.MediumSeaGreen, Color.IndianRed };
 for(int i=0;i<_pieData.Length;i++){ var item = _pieData[i]; double sweep = (item.value/total)*360.0; var rect = new Rectangle(cx - radius, cy - radius, radius*2, radius*2);
 using(var brush = new SolidBrush(colors[i%colors.Length])) g.FillPie(brush, rect, (float)startAngle, (float)sweep); g.DrawPie(Pens.Black, rect, (float)startAngle, (float)sweep);
 double mid = startAngle + sweep/2.0; float lx = cx + (float)((radius +14) * Math.Cos(mid * Math.PI/180.0)); float ly = cy + (float)((radius +14) * Math.Sin(mid * Math.PI/180.0)); var pct = Math.Round((item.value/total)*100,1);
 g.DrawString($"{item.name} ({item.value}, {pct}%)", new Font(this.Font.FontFamily,9), Brushes.Black, lx -30, ly -8);
 startAngle += sweep; }

 int legendX =10; int legendY = pnlPie.ClientSize.Height -60; for(int i=0;i<_pieData.Length;i++){ using(var brush=new SolidBrush(colors[i%colors.Length])) g.FillRectangle(brush, legendX, legendY + i*20,12,12); g.DrawRectangle(Pens.Black, legendX, legendY + i*20,12,12); g.DrawString(_pieData[i].name, new Font(this.Font.FontFamily,9), Brushes.Black, legendX +18, legendY + i*20 -2); }
 }
 }
}
