using System;
using System.Linq;
using System.Windows.Forms;
using Project.Reports;
using Project.Services;
using System.Drawing;

namespace Project.UI.Forms
{
 public class ReportsForm : UserControl
 {
 private readonly ReportService _reportService;
 private readonly ProductService _productService;
 private Button btnExportExcel = new Button();
 private Button btnExportPdf = new Button();
 private Button btnExportTopExcel = new Button();
 private Button btnExportTopPdf = new Button();
 private Button btnShowTop = new Button();
 private Panel pnlChart = new Panel();
 private (string name,int value)[] _chartData = Array.Empty<(string,int)>();

 public ReportsForm(ReportService reportService, ProductService productService)
 {
 _reportService = reportService;
 _productService = productService;
 InitializeComponents();
 }

 private void InitializeComponents()
 {
 this.Dock = DockStyle.Fill;
 var lbl = new Label { Text = "Reports", Left =10, Top =10, AutoSize = true, Font = new Font(FontFamily.GenericSansSerif,12, FontStyle.Bold) };
 btnExportExcel.Text = "Export Critical Stock to Excel"; btnExportExcel.Left =10; btnExportExcel.Top =40; btnExportExcel.Width =240; btnExportExcel.Click += BtnExportExcel_Click;
 btnExportPdf.Text = "Export Critical Stock to PDF"; btnExportPdf.Left =260; btnExportPdf.Top =40; btnExportPdf.Width =240; btnExportPdf.Click += BtnExportPdf_Click;
 btnShowTop.Text = "Show Top Consumed"; btnShowTop.Left =520; btnShowTop.Top =40; btnShowTop.Width =140; btnShowTop.Click += BtnShowTop_Click;
 btnExportTopExcel.Text = "Export Top Consumed to Excel"; btnExportTopExcel.Left =670; btnExportTopExcel.Top =40; btnExportTopExcel.Width =200; btnExportTopExcel.Click += BtnExportTopExcel_Click;
 btnExportTopPdf.Text = "Export Top Consumed to PDF"; btnExportTopPdf.Left =880; btnExportTopPdf.Top =40; btnExportTopPdf.Width =200; btnExportTopPdf.Click += BtnExportTopPdf_Click;

 pnlChart.Left =10; pnlChart.Top =80; pnlChart.Width =1100; pnlChart.Height =400; pnlChart.BorderStyle = BorderStyle.FixedSingle; pnlChart.Paint += PnlChart_Paint; this.AutoScroll = true;

 this.Controls.Add(lbl); this.Controls.Add(btnExportExcel); this.Controls.Add(btnExportPdf); this.Controls.Add(btnShowTop); this.Controls.Add(btnExportTopExcel); this.Controls.Add(btnExportTopPdf); this.Controls.Add(pnlChart);
 }

 private void BtnExportExcel_Click(object? sender, EventArgs e)
 {
 using var sfd = new SaveFileDialog { Filter = "Excel Files|*.xlsx", FileName = "CriticalStock.xlsx" };
 if (sfd.ShowDialog() != DialogResult.OK) return;
 try { _reportService.ExportCriticalStockToExcel(sfd.FileName); MessageBox.Show("Exported", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }

 private void BtnExportPdf_Click(object? sender, EventArgs e)
 {
 using var sfd = new SaveFileDialog { Filter = "PDF Files|*.pdf", FileName = "CriticalStock.pdf" };
 if (sfd.ShowDialog() != DialogResult.OK) return;
 try { _reportService.ExportCriticalStockToPdf(sfd.FileName); MessageBox.Show("Exported", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }

 private void BtnExportTopExcel_Click(object? sender, EventArgs e)
 {
 using var sfd = new SaveFileDialog { Filter = "Excel Files|*.xlsx", FileName = "TopConsumed.xlsx" };
 if (sfd.ShowDialog() != DialogResult.OK) return;
 try { _reportService.ExportTopConsumedToExcel(sfd.FileName); MessageBox.Show("Exported", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }

 private void BtnExportTopPdf_Click(object? sender, EventArgs e)
 {
 using var sfd = new SaveFileDialog { Filter = "PDF Files|*.pdf", FileName = "TopConsumed.pdf" };
 if (sfd.ShowDialog() != DialogResult.OK) return;
 try { _reportService.ExportTopConsumedToPdf(sfd.FileName); MessageBox.Show("Exported", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }

 private void BtnShowTop_Click(object? sender, EventArgs e)
 {
 var top = _reportService.GetTopConsumedProducts(10).ToList();
 _chartData = top.Select(t => (t.product.Name, t.totalOut)).ToArray();
 pnlChart.Invalidate();
 }

 private void PnlChart_Paint(object? sender, PaintEventArgs e)
 {
 var g = e.Graphics;
 g.Clear(Color.White);
 if (_chartData == null || _chartData.Length ==0) { g.DrawString("No data", this.Font, Brushes.Black,10,10); return; }
 int w = pnlChart.ClientSize.Width -40;
 int h = pnlChart.ClientSize.Height -40;
 int barCount = _chartData.Length;
 int barWidth = Math.Max(20, w / (barCount *2));
 int max = _chartData.Max(x => x.value);
 if (max ==0) max =1;
 for (int i=0;i<barCount;i++)
 {
 var item = _chartData[i];
 int barH = (int)((item.value / (double)max) * (h -40));
 int x =20 + i*(barWidth*2);
 int y =20 + (h - barH);
 g.FillRectangle(Brushes.SteelBlue, x, y, barWidth, barH);
 g.DrawString(item.name, new Font(this.Font.FontFamily,8), Brushes.Black, x,22 + h);
 g.DrawString(item.value.ToString(), new Font(this.Font.FontFamily,8), Brushes.Black, x, y -14);
 }
 }
 }
}
