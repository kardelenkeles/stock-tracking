using System;
using System.Linq;
using System.Windows.Forms;
using Project.Services;
using Project.Core.Models;

namespace Project.UI.Forms
{
 public class ProductListForm : Form
 {
 private readonly ProductService _service;
 private readonly CategoryService _catService;
 private readonly SupplierService _supService;
 private readonly User? _currentUser;
 private DataGridView dgv = new DataGridView();
 private Button btnAdd = new Button();
 private Button btnEdit = new Button();
 private Button btnDelete = new Button();
 private TextBox txtSearch = new TextBox();
 private ComboBox cbCategoryFilter = new ComboBox();
 private Button btnSearch = new Button();
 private Button btnPrev = new Button();
 private Button btnNext = new Button();
 private int _page =1;
 private const int PageSize =20;

 public ProductListForm(ProductService service, CategoryService catService, SupplierService supService, User? currentUser = null)
 {
 _service = service;
 _catService = catService;
 _supService = supService;
 _currentUser = currentUser;
 InitializeComponents();
 LoadData();
 }

 private void InitializeComponents()
 {
 this.Text = "Products";
 this.Width =1000;
 this.Height =500;

 txtSearch.Left =10; txtSearch.Top =10; txtSearch.Width =300;
 cbCategoryFilter.Left =320; cbCategoryFilter.Top =10; cbCategoryFilter.Width =200; cbCategoryFilter.DropDownStyle = ComboBoxStyle.DropDownList;
 btnSearch.Text = "Search"; btnSearch.Left =540; btnSearch.Top =10; btnSearch.Click += (s,e) => { _page =1; LoadData(); };

 dgv.Left =10; dgv.Top =40; dgv.Width =960; dgv.Height =380; dgv.ReadOnly = true; dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.AutoGenerateColumns = false;
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Id", DataPropertyName = "Id", Width =50 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Width =250 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Category", DataPropertyName = "CategoryId", Width =150 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Supplier", DataPropertyName = "SupplierId", Width =150 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Quantity", DataPropertyName = "Quantity", Width =80 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "MinQty", DataPropertyName = "MinQuantity", Width =80 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Purchase", DataPropertyName = "PurchasePrice", Width =100 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Sell", DataPropertyName = "SellPrice", Width =100 });
 dgv.CellFormatting += Dgv_CellFormatting;
 dgv.SelectionChanged += Dgv_SelectionChanged;

 btnAdd.Text = "Add"; btnAdd.Left =10; btnAdd.Top =430; btnAdd.Click += BtnAdd_Click;
 btnEdit.Text = "Edit"; btnEdit.Left =90; btnEdit.Top =430; btnEdit.Click += BtnEdit_Click;
 btnDelete.Text = "Delete"; btnDelete.Left =170; btnDelete.Top =430; btnDelete.Click += BtnDelete_Click;
 btnPrev.Text = "Prev"; btnPrev.Left =260; btnPrev.Top =430; btnPrev.Click += (s,e)=> { if (_page>1) { _page--; LoadData(); } };
 btnNext.Text = "Next"; btnNext.Left =340; btnNext.Top =430; btnNext.Click += (s,e)=> { _page++; LoadData(); };

 this.Controls.Add(txtSearch);
 this.Controls.Add(cbCategoryFilter);
 this.Controls.Add(btnSearch);
 this.Controls.Add(dgv);
 this.Controls.Add(btnAdd);
 this.Controls.Add(btnEdit);
 this.Controls.Add(btnDelete);
 this.Controls.Add(btnPrev);
 this.Controls.Add(btnNext);


 
 if (_currentUser == null || _currentUser.Role != "Admin")
 {
 btnAdd.Enabled = false;
 btnEdit.Enabled = false;
 btnDelete.Enabled = false;
 }
 }

 private void Dgv_SelectionChanged(object? sender, EventArgs e)
 {
 
 var sel = SelectedProduct();
 if (sel == null) { btnDelete.Enabled = false; return; }
 try
 {
 var has = _service.HasMovements(sel.Id);
 
 btnDelete.Enabled = (_currentUser != null && _currentUser.Role == "Admin") && !has;
 }
 catch { btnDelete.Enabled = false; }
 }

 private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
 {
 if (dgv.Columns[e.ColumnIndex].DataPropertyName == "CategoryId")
 {
 var pid = (int?)e.Value;
 if (pid.HasValue) { var cat = _catService.GetById(pid.Value); e.Value = cat?.Name ?? ""; }
 else e.Value = "";
 }
 if (dgv.Columns[e.ColumnIndex].DataPropertyName == "SupplierId")
 {
 var pid = (int?)e.Value;
 if (pid.HasValue) { var s = _supService.GetById(pid.Value); e.Value = s?.Name ?? ""; }
 else e.Value = "";
 }
 }

 private void LoadData()
 {
 
 if (cbCategoryFilter.Items.Count ==0)
 {
 var cats = _catService.GetAll().ToList();
 cbCategoryFilter.Items.Add(new { Id = (int?)null, Name = "All" });
 foreach(var c in cats) cbCategoryFilter.Items.Add(c);
 cbCategoryFilter.DisplayMember = "Name"; cbCategoryFilter.ValueMember = "Id";
 cbCategoryFilter.SelectedIndex =0;
 }

 var list = _service.GetAll().ToList();
 
 if (!string.IsNullOrWhiteSpace(txtSearch.Text)) list = list.Where(p => p.Name.Contains(txtSearch.Text.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
 if (cbCategoryFilter.SelectedItem is Category selCat) list = list.Where(p => p.CategoryId == selCat.Id).ToList();
 
 list = list.Skip((_page-1)*PageSize).Take(PageSize).ToList();
 dgv.DataSource = list;
 }

 private void BtnAdd_Click(object? sender, EventArgs e)
 {
 var cats = _catService.GetAll();
 var sups = _supService.GetAll();
 var f = new ProductEditForm(null, cats, sups);
 if (f.ShowDialog()== DialogResult.OK)
 {
 try { _service.Create(f.Product, _currentUser); LoadData(); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }
 }

 private Product? SelectedProduct()
 {
 if (dgv.SelectedRows.Count ==0) return null;
 return dgv.SelectedRows[0].DataBoundItem as Product;
 }

 private void BtnEdit_Click(object? sender, EventArgs e)
 {
 var sel = SelectedProduct();
 if (sel==null) { MessageBox.Show("Select a product","Info",MessageBoxButtons.OK,MessageBoxIcon.Information); return; }
 var cats = _catService.GetAll();
 var sups = _supService.GetAll();
 var f = new ProductEditForm(sel, cats, sups);
 if (f.ShowDialog()== DialogResult.OK)
 {
 try { _service.Update(f.Product, _currentUser); LoadData(); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }
 }

 private void BtnDelete_Click(object? sender, EventArgs e)
 {
 var sel = SelectedProduct();
 if (sel==null) { MessageBox.Show("Select a product","Info",MessageBoxButtons.OK,MessageBoxIcon.Information); return; }
 if (MessageBox.Show("Delete this product?","Confirm",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!= DialogResult.Yes) return;
 try { _service.Delete(sel.Id, _currentUser); LoadData(); }
 catch (InvalidOperationException ex) { MessageBox.Show("Bu �r�n�n stok hareketleri mevcut oldu?u i�in silinemez.", "Silme Hatas?", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }
 }
}
