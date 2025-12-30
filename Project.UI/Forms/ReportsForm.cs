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
 btnExportPdf.Text = "Export Critical Stock to PDF"; btnExportPdf.Left =10; btnExportPdf.Top =80; btnExportPdf.Width =240; btnExportPdf.Click += BtnExportPdf_Click;
 btnExportTopExcel.Text = "Export Top Consumed to Excel"; btnExportTopExcel.Left =10; btnExportTopExcel.Top =120; btnExportTopExcel.Width =240; btnExportTopExcel.Click += BtnExportTopExcel_Click;
 btnExportTopPdf.Text = "Export Top Consumed to PDF"; btnExportTopPdf.Left =10; btnExportTopPdf.Top =160; btnExportTopPdf.Width =240; btnExportTopPdf.Click += BtnExportTopPdf_Click;

 this.Controls.Add(lbl);
 this.Controls.Add(btnExportExcel);
 this.Controls.Add(btnExportPdf);
 this.Controls.Add(btnExportTopExcel);
 this.Controls.Add(btnExportTopPdf);
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

 }
}
