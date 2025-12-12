using System;
using System.Windows.Forms;
using Project.Core.Models;

namespace Project.UI.Forms
{
 public class SupplierEditForm : Form
 {
 private TextBox txtName = new TextBox();
 private TextBox txtContact = new TextBox();
 private TextBox txtPhone = new TextBox();
 private TextBox txtEmail = new TextBox();
 private TextBox txtAddress = new TextBox();
 private Button btnOk = new Button();
 private Button btnCancel = new Button();

 public Supplier Supplier { get; private set; }

 public SupplierEditForm(Supplier? supplier = null)
 {
 Supplier = supplier ?? new Supplier();
 InitializeComponents();
 if (supplier != null)
 {
 txtName.Text = Supplier.Name;
 txtContact.Text = Supplier.ContactName ?? string.Empty;
 txtPhone.Text = Supplier.Phone ?? string.Empty;
 txtEmail.Text = Supplier.Email ?? string.Empty;
 txtAddress.Text = Supplier.Address ?? string.Empty;
 }
 }

 private void InitializeComponents()
 {
 this.Text = "Supplier";
 this.Width =500;
 this.Height =300;
 this.FormBorderStyle = FormBorderStyle.FixedDialog;
 this.StartPosition = FormStartPosition.CenterParent;
 this.MaximizeBox = false;

 var lblName = new Label { Text = "Name:", Left =10, Top =20, Width =100 };
 txtName.Left =120; txtName.Top =20; txtName.Width =340;
 var lblContact = new Label { Text = "Contact:", Left =10, Top =60, Width =100 };
 txtContact.Left =120; txtContact.Top =60; txtContact.Width =340;
 var lblPhone = new Label { Text = "Phone:", Left =10, Top =100, Width =100 };
 txtPhone.Left =120; txtPhone.Top =100; txtPhone.Width =200;
 var lblEmail = new Label { Text = "Email:", Left =10, Top =140, Width =100 };
 txtEmail.Left =120; txtEmail.Top =140; txtEmail.Width =340;
 var lblAddress = new Label { Text = "Address:", Left =10, Top =180, Width =100 };
 txtAddress.Left =120; txtAddress.Top =180; txtAddress.Width =340;

 btnOk.Text = "OK"; btnOk.Left =300; btnOk.Top =220; btnOk.Click += BtnOk_Click;
 btnCancel.Text = "Cancel"; btnCancel.Left =380; btnCancel.Top =220; btnCancel.Click += (s,e)=> this.DialogResult = DialogResult.Cancel;

 this.Controls.Add(lblName); this.Controls.Add(txtName);
 this.Controls.Add(lblContact); this.Controls.Add(txtContact);
 this.Controls.Add(lblPhone); this.Controls.Add(txtPhone);
 this.Controls.Add(lblEmail); this.Controls.Add(txtEmail);
 this.Controls.Add(lblAddress); this.Controls.Add(txtAddress);
 this.Controls.Add(btnOk); this.Controls.Add(btnCancel);
 }

 private void BtnOk_Click(object? sender, EventArgs e)
 {
 if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Name required","Validation",MessageBoxButtons.OK,MessageBoxIcon.Warning); return; }
 Supplier.Name = txtName.Text.Trim();
 Supplier.ContactName = string.IsNullOrWhiteSpace(txtContact.Text)? null : txtContact.Text.Trim();
 Supplier.Phone = string.IsNullOrWhiteSpace(txtPhone.Text)? null : txtPhone.Text.Trim();
 Supplier.Email = string.IsNullOrWhiteSpace(txtEmail.Text)? null : txtEmail.Text.Trim();
 Supplier.Address = string.IsNullOrWhiteSpace(txtAddress.Text)? null : txtAddress.Text.Trim();
 this.DialogResult = DialogResult.OK;
 }
 }
}
