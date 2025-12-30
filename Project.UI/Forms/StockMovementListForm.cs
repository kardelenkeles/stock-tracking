using System;
using System.Linq;
using System.Windows.Forms;
using Project.Services;
using Project.Core.Models;

namespace Project.UI.Forms
{
 public class StockMovementListForm : Form
 {
 private DataGridView dgv = new DataGridView();
 private Button btnAdd = new Button();
 private DateTimePicker dtFrom = new DateTimePicker();
 private DateTimePicker dtTo = new DateTimePicker();
 private TextBox txtProduct = new TextBox();
 private ComboBox cbType = new ComboBox();
 private Button btnFilter = new Button();

 private readonly StockService _stockService;
 private readonly ProductService _productService;
 private readonly CategoryService _categoryService;
 private readonly SupplierService _supplier_service;
 private readonly int? _currentUserId;

 public StockMovementListForm(StockService stockService, ProductService productService, CategoryService categoryService, SupplierService supplierService, int? currentUserId = null)
 {
 _stockService = stockService;
 _productService = productService;
 _categoryService = categoryService;
 _supplier_service = supplierService;
 _currentUserId = currentUserId;
 InitializeComponents();
 LoadData();
 }

 private void InitializeComponents()
 {
 this.Text = "Stock Movements";
 this.Width =1000;
 this.Height =600;

 var lblFrom = new Label { Text = "From:", Left =10, Top =10, Width =50 };
 dtFrom.Left =70; dtFrom.Top =10; dtFrom.Width =200; dtFrom.Format = DateTimePickerFormat.Short; dtFrom.Value = DateTime.Now.AddMonths(-1);
 var lblTo = new Label { Text = "To:", Left =280, Top =10, Width =30 };
 dtTo.Left =320; dtTo.Top =10; dtTo.Width =200; dtTo.Format = DateTimePickerFormat.Short;
 var lblProduct = new Label { Text = "Product:", Left =530, Top =10, Width =60 };
 txtProduct.Left =600; txtProduct.Top =10; txtProduct.Width =200;
 var lblType = new Label { Text = "Type:", Left =810, Top =10, Width =40 };
 cbType.Left =860; cbType.Top =10; cbType.Width =100; cbType.Items.AddRange(new string[] { "All", "IN", "OUT" }); cbType.SelectedIndex =0;
 btnFilter.Text = "Filter"; btnFilter.Left =900; btnFilter.Top =40; btnFilter.Click += (s,e)=> LoadData();

 dgv.Left =10; dgv.Top =80; dgv.Width =960; dgv.Height =420; dgv.ReadOnly = true; dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.AutoGenerateColumns = false;
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Id", DataPropertyName = "Id", Width =50 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Product", DataPropertyName = "ProductId", Width =250 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Type", DataPropertyName = "MovementType", Width =80 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Qty", DataPropertyName = "Quantity", Width =80 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Date", DataPropertyName = "MovementDate", Width =200 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "UserId", DataPropertyName = "UserId", Width =80 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Notes", DataPropertyName = "Notes", Width =400 });
 dgv.CellFormatting += Dgv_CellFormatting;

 btnAdd.Text = "Add Movement"; btnAdd.Left =10; btnAdd.Top =520; btnAdd.Click += BtnAdd_Click;

 this.Controls.Add(lblFrom); this.Controls.Add(dtFrom);
 this.Controls.Add(lblTo); this.Controls.Add(dtTo);
 this.Controls.Add(lblProduct); this.Controls.Add(txtProduct);
 this.Controls.Add(lblType); this.Controls.Add(cbType);
 this.Controls.Add(btnFilter);
 this.Controls.Add(dgv);
 this.Controls.Add(btnAdd);
 }

 private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
 {
 if (dgv.Columns[e.ColumnIndex].DataPropertyName == "ProductId")
 {
 var pid = (int)e.Value;
 var name = _productService.GetById(pid)?.Name ?? string.Empty;
 e.Value = name;
 }
 if (dgv.Columns[e.ColumnIndex].DataPropertyName == "MovementDate")
 {
 if (e.Value is DateTime dt) e.Value = dt.ToString("g");
 }
 }

 private void LoadData()
 {
 var list = _stockService.GetAll().ToList();
 
 var from = dtFrom.Value.Date;
 var to = dtTo.Value.Date.AddDays(1).AddSeconds(-1);
 list = list.Where(m => m.MovementDate >= from && m.MovementDate <= to).ToList();
 
 if (!string.IsNullOrWhiteSpace(txtProduct.Text)) list = list.Where(m => (_productService.GetById(m.ProductId)?.Name ?? string.Empty).Contains(txtProduct.Text.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
 
 if (cbType.SelectedItem is string t && t != "All") list = list.Where(m => m.MovementType == t).ToList();

 dgv.DataSource = list;
 }

 private void BtnAdd_Click(object? sender, EventArgs e)
 {
 var f = new StockMovementForm(_productService, _stockService, _currentUserId);
 if (f.ShowDialog() == DialogResult.OK) LoadData();
 }
 }
}
