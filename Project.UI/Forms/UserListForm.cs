using System;
using System.Linq;
using System.Windows.Forms;
using Project.Services;
using Project.Core.Models;
using System.Drawing;

namespace Project.UI.Forms
{
 public class UserListForm : Form
 {
 private readonly UserService _service;
 private readonly int? _currentUserId;
 private DataGridView dgv = new DataGridView();
 private Button btnAdd = new Button();
 private Button btnEdit = new Button();
 private Button btnDelete = new Button();

 public UserListForm(UserService service, int? currentUserId = null)
 {
 _service = service;
 _currentUserId = currentUserId;
 InitializeComponents();
 LoadData();
 }

 private void InitializeComponents()
 {
 this.Text = "Users";
 this.Width =600;
 this.Height =400;

 dgv.Left =10; dgv.Top =10; dgv.Width =560; dgv.Height =300; dgv.ReadOnly = true; dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.AutoGenerateColumns = false;
 dgv.Columns.Add(new DataGridViewImageColumn { HeaderText = "", Width =24 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Id", DataPropertyName = "Id", Width =50 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Username", DataPropertyName = "Username", Width =200 });
 dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Role", DataPropertyName = "Role", Width =120 });
 dgv.SelectionChanged += Dgv_SelectionChanged;

 btnAdd.Text = "Add"; btnAdd.Left =10; btnAdd.Top =320; btnAdd.Click += BtnAdd_Click;
 btnEdit.Text = "Edit"; btnEdit.Left =90; btnEdit.Top =320; btnEdit.Click += BtnEdit_Click;
 btnDelete.Text = "Delete"; btnDelete.Left =170; btnDelete.Top =320; btnDelete.Click += BtnDelete_Click;

 this.Controls.Add(dgv); this.Controls.Add(btnAdd); this.Controls.Add(btnEdit); this.Controls.Add(btnDelete);
 }

 private void LoadData()
 {
 var list = _service.GetAll().ToList();

 var binding = list.Select(u => new {
 u.Id,
 u.Username,
 u.Role,
 HasMovements = _service.HasMovements(u.Id)
 }).ToList();

 dgv.Rows.Clear();
 foreach (var item in binding)
 {
 int idx = dgv.Rows.Add();
 var row = dgv.Rows[idx];
 row.Cells[0].Value = item.HasMovements ? SystemIcons.Shield.ToBitmap() : null;
 row.Cells[1].Value = item.Id;
 row.Cells[2].Value = item.Username;
 row.Cells[3].Value = item.Role;
 }
 }

 private void Dgv_SelectionChanged(object? sender, EventArgs e)
 {
 var sel = SelectedUser();
 if (sel == null) { btnDelete.Enabled = false; return; }
 if (_currentUserId.HasValue && _currentUserId.Value == sel.Id) { btnDelete.Enabled = false; return; }
 var has = _service.HasMovements(sel.Id);
 btnDelete.Enabled = !has;
 }

 private void BtnAdd_Click(object? sender, EventArgs e)
 {
 var f = new UserEditForm();
 if (f.ShowDialog()== DialogResult.OK)
 {
 try { _service.Create(f.User, _currentUserId); LoadData(); MessageBox.Show("User created and logged.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }
 }

 private User? SelectedUser() => dgv.SelectedRows.Count>0 ? _service.GetById(Convert.ToInt32(dgv.SelectedRows[0].Cells[1].Value)) : null;

 private void BtnEdit_Click(object? sender, EventArgs e)
 {
 var sel = SelectedUser();
 if (sel==null) { MessageBox.Show("Select a user","Info",MessageBoxButtons.OK,MessageBoxIcon.Information); return; }
 var f = new UserEditForm(sel);
 if (f.ShowDialog()== DialogResult.OK)
 {
 try { _service.Update(f.User, _currentUserId); LoadData(); MessageBox.Show("User updated and logged.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }
 }

 private void BtnDelete_Click(object? sender, EventArgs e)
 {
 var sel = SelectedUser();
 if (sel==null) { MessageBox.Show("Select a user","Info",MessageBoxButtons.OK,MessageBoxIcon.Information); return; }
 if (_currentUserId.HasValue && _currentUserId.Value == sel.Id) { MessageBox.Show("Cannot delete currently logged-in user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
 if (MessageBox.Show("Delete this user?","Confirm",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!= DialogResult.Yes) return;
 try { _service.Delete(sel.Id, _currentUserId); LoadData(); MessageBox.Show("User deleted and logged.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information); }
 catch (InvalidOperationException ex) { MessageBox.Show("This user has associated stock movements and cannot be deleted.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
 catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
 }
 }
}
