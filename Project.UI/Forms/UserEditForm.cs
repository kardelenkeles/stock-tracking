using System;
using System.Windows.Forms;
using Project.Core.Models;
using Project.Services;

namespace Project.UI.Forms
{
 public class UserEditForm : Form
 {
 private TextBox txtUsername = new TextBox();
 private TextBox txtPassword = new TextBox();
 private ComboBox cbRole = new ComboBox();
 private Button btnOk = new Button();
 private Button btnCancel = new Button();
 public User User { get; private set; }

 public UserEditForm(User? user = null)
 {
 User = user ?? new User();
 InitializeComponents();
 if (user != null)
 {
 txtUsername.Text = User.Username;
 
 cbRole.SelectedItem = User.Role;
 }
 }

 private void InitializeComponents()
 {
 this.Text = "User";
 this.Width =400;
 this.Height =220;
 this.FormBorderStyle = FormBorderStyle.FixedDialog;
 this.StartPosition = FormStartPosition.CenterParent;

 var lblUser = new Label { Text = "Username:", Left =10, Top =20, Width =100 };
 txtUsername.Left =120; txtUsername.Top =20; txtUsername.Width =240;
 var lblPass = new Label { Text = "Password:", Left =10, Top =60, Width =100 };
 txtPassword.Left =120; txtPassword.Top =60; txtPassword.Width =240; txtPassword.UseSystemPasswordChar = true;
 var lblRole = new Label { Text = "Role:", Left =10, Top =100, Width =100 };
 cbRole.Left =120; cbRole.Top =100; cbRole.Width =240; cbRole.Items.AddRange(new string[] { "Admin", "User" }); cbRole.DropDownStyle = ComboBoxStyle.DropDownList; cbRole.SelectedIndex =0;

 btnOk.Text = "OK"; btnOk.Left =200; btnOk.Top =140; btnOk.Click += BtnOk_Click;
 btnCancel.Text = "Cancel"; btnCancel.Left =280; btnCancel.Top =140; btnCancel.Click += (s,e)=> this.DialogResult = DialogResult.Cancel;

 this.Controls.Add(lblUser); this.Controls.Add(txtUsername); this.Controls.Add(lblPass); this.Controls.Add(txtPassword); this.Controls.Add(lblRole); this.Controls.Add(cbRole); this.Controls.Add(btnOk); this.Controls.Add(btnCancel);
 }

 private void BtnOk_Click(object? sender, EventArgs e)
 {
 if (string.IsNullOrWhiteSpace(txtUsername.Text)) { MessageBox.Show("Username required","Validation",MessageBoxButtons.OK,MessageBoxIcon.Warning); return; }
 if (string.IsNullOrWhiteSpace(txtPassword.Text) && User.Id==0) { MessageBox.Show("Password required for new user","Validation",MessageBoxButtons.OK,MessageBoxIcon.Warning); return; }
 User.Username = txtUsername.Text.Trim();
 if (!string.IsNullOrWhiteSpace(txtPassword.Text)) User.PasswordHash = Project.Services.AuthService.HashPassword(txtPassword.Text);
 User.Role = cbRole.SelectedItem as string ?? "User";
 this.DialogResult = DialogResult.OK;
 }
 }
}
