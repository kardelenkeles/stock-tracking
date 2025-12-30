using System;
using System.Linq;
using System.Windows.Forms;
using Project.Services;
using Project.Core.Models;

namespace Project.UI.Forms
{
 public class CategoryListForm : Form
 {
 private readonly CategoryService _service;
 private readonly User? _currentUser;
 private DataGridView dgv = new DataGridView();
 private Button btnAdd = new Button();
 private Button btnEdit = new Button();
 private Button btnDelete = new Button();
 private TextBox txtSearch = new TextBox();
 private Button btnSearch = new Button();
 private Button btnPrev = new Button();
 private Button btnNext = new Button();
 private int _page =1;
 private const int PageSize =20;

 public CategoryListForm(CategoryService service, User? currentUser = null)
 {
 _service = service;
 _currentUser = currentUser;
 InitializeComponents();
 LoadData();
 }

 private void InitializeComponents()
 {
 this.Text = "Categories";
 this.Width =700;
 this.Height =450;

 txtSearch.Left =10; txtSearch.Top =10; txtSearch.Width =300;
 btnSearch.Text = "Search"; btnSearch.Left =320; btnSearch.Top =10; btnSearch.Click += (s,e) => { _page =1; LoadData(); };

 dgv.Left =10; dgv.Top =40; dgv.Width =660; dgv.Height =320; dgv.ReadOnly = true; dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.AutoGenerateColumns = false;
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Id", DataPropertyName = "Id", Width =50 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Width =200 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Description", DataPropertyName = "Description", Width =380 });

 btnAdd.Text = "Add"; btnAdd.Left =10; btnAdd.Top =370; btnAdd.Click += BtnAdd_Click;
 btnEdit.Text = "Edit"; btnEdit.Left =90; btnEdit.Top =370; btnEdit.Click += BtnEdit_Click;
 btnDelete.Text = "Delete"; btnDelete.Left =170; btnDelete.Top =370; btnDelete.Click += BtnDelete_Click;
 btnPrev.Text = "Prev"; btnPrev.Left =260; btnPrev.Top =370; btnPrev.Click += (s,e)=> { if (_page>1) { _page--; LoadData(); } };
 btnNext.Text = "Next"; btnNext.Left =340; btnNext.Top =370; btnNext.Click += (s,e)=> { _page++; LoadData(); };

 this.Controls.Add(txtSearch);
 this.Controls.Add(btnSearch);
 this.Controls.Add(dgv);
 this.Controls.Add(btnAdd);
 this.Controls.Add(btnEdit);
 this.Controls.Add(btnDelete);
 this.Controls.Add(btnPrev);
 this.Controls.Add(btnNext);

 
 if (_currentUser == null || _currentUser.Role != "Admin") {
 btnAdd.Enabled = false; btnEdit.Enabled = false; btnDelete.Enabled = false;
 }
 }

 private void LoadData()
 {
 var list = _service.GetAll(txtSearch.Text.Trim(), _page, PageSize).ToList();
 dgv.DataSource = list;
 }

 private void BtnAdd_Click(object? sender, EventArgs e)
 {
 var f = new CategoryEditForm();
 if (f.ShowDialog() == DialogResult.OK)
 {
 try
 {
 _service.Create(f.Category, _currentUser);
 LoadData();
 }
 catch (Exception ex)
 {
 MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
 }
 }
 }

 private Category? SelectedCategory()
 {
 if (dgv.SelectedRows.Count ==0) return null;
 return dgv.SelectedRows[0].DataBoundItem as Category;
 }

 private void BtnEdit_Click(object? sender, EventArgs e)
 {
 var sel = SelectedCategory();
 if (sel == null) { MessageBox.Show("Select a category", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
 var f = new CategoryEditForm(sel);
 if (f.ShowDialog() == DialogResult.OK)
 {
 try { _service.Update(f.Category, _currentUser); LoadData(); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }
 }

 private void BtnDelete_Click(object? sender, EventArgs e)
 {
 var sel = SelectedCategory();
 if (sel==null) { MessageBox.Show("Select a category","Info",MessageBoxButtons.OK,MessageBoxIcon.Information); return; }
 if (MessageBox.Show("Delete this category?","Confirm",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!= DialogResult.Yes) return;
 try { _service.Delete(sel.Id, _currentUser); LoadData(); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }
 }
}
