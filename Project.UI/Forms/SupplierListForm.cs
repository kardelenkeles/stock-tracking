using System;
using System.Linq;
using System.Windows.Forms;
using Project.Services;
using Project.Core.Models;

namespace Project.UI.Forms
{
 public class SupplierListForm : Form
 {
 private readonly SupplierService _service;
 private readonly User? _currentUser;
 private DataGridView dgv = new DataGridView();
 private Button btnAdd = new Button();
 private Button btnEdit = new Button();
 private Button btnDelete = new Button();

 public SupplierListForm(SupplierService service, User? currentUser = null)
 {
 _service = service;
 _currentUser = currentUser;
 InitializeComponents();
 LoadData();
 }

 private void InitializeComponents()
 {
 this.Text = "Suppliers";
 this.Width =800;
 this.Height =450;

 dgv.Left =10; dgv.Top =10; dgv.Width =760; dgv.Height =350; dgv.ReadOnly = true; dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.AutoGenerateColumns = false;
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Id", DataPropertyName = "Id", Width =50 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Width =200 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Contact", DataPropertyName = "ContactName", Width =150 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Phone", DataPropertyName = "Phone", Width =120 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Email", DataPropertyName = "Email", Width =150 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Address", DataPropertyName = "Address", Width =250 });

 btnAdd.Text = "Add"; btnAdd.Left =10; btnAdd.Top =370; btnAdd.Click += BtnAdd_Click;
 btnEdit.Text = "Edit"; btnEdit.Left =90; btnEdit.Top =370; btnEdit.Click += BtnEdit_Click;
 btnDelete.Text = "Delete"; btnDelete.Left =170; btnDelete.Top =370; btnDelete.Click += BtnDelete_Click;

 this.Controls.Add(dgv);
 this.Controls.Add(btnAdd);
 this.Controls.Add(btnEdit);
 this.Controls.Add(btnDelete);

 
 if (_currentUser == null || _currentUser.Role != "Admin") {
 btnAdd.Enabled = false; btnEdit.Enabled = false; btnDelete.Enabled = false;
 }
 }

 private void LoadData()
 {
 var list = _service.GetAll().ToList();
 dgv.DataSource = list;
 }

 private void BtnAdd_Click(object? sender, EventArgs e)
 {
 var f = new SupplierEditForm();
 if (f.ShowDialog()== DialogResult.OK)
 {
 try { _service.Create(f.Supplier, _currentUser); LoadData(); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }
 }

 private Supplier? SelectedSupplier()
 {
 if (dgv.SelectedRows.Count ==0) return null;
 return dgv.SelectedRows[0].DataBoundItem as Supplier;
 }

 private void BtnEdit_Click(object? sender, EventArgs e)
 {
 var sel = SelectedSupplier();
 if (sel==null) { MessageBox.Show("Select a supplier","Info",MessageBoxButtons.OK,MessageBoxIcon.Information); return; }
 var f = new SupplierEditForm(sel);
 if (f.ShowDialog()== DialogResult.OK)
 {
 try { _service.Update(f.Supplier, _currentUser); LoadData(); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }
 }

 private void BtnDelete_Click(object? sender, EventArgs e)
 {
 var sel = SelectedSupplier();
 if (sel==null) { MessageBox.Show("Select a supplier","Info",MessageBoxButtons.OK,MessageBoxIcon.Information); return; }
 if (MessageBox.Show("Delete this supplier?","Confirm",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!= DialogResult.Yes) return;
 try { _service.Delete(sel.Id, _currentUser); LoadData(); }
 catch (InvalidOperationException ex) { MessageBox.Show($"Cannot delete supplier: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }
 }
}
