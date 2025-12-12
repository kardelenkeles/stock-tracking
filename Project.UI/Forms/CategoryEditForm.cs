using System;
using System.Windows.Forms;
using Project.Core.Models;

namespace Project.UI.Forms
{
 public class CategoryEditForm : Form
 {
 private TextBox txtName = new TextBox();
 private TextBox txtDescription = new TextBox();
 private Button btnOk = new Button();
 private Button btnCancel = new Button();

 public Category Category { get; private set; }

 public CategoryEditForm(Category? category = null)
 {
 Category = category ?? new Category();
 InitializeComponents();
 if (category != null)
 {
 txtName.Text = Category.Name;
 txtDescription.Text = Category.Description ?? string.Empty;
 }
 }

 private void InitializeComponents()
 {
 this.Text = "Category";
 this.Width =400;
 this.Height =200;
 this.FormBorderStyle = FormBorderStyle.FixedDialog;
 this.StartPosition = FormStartPosition.CenterParent;
 this.MaximizeBox = false;

 var lblName = new Label { Text = "Name:", Left =10, Top =20, Width =80 };
 txtName.Left =100; txtName.Top =20; txtName.Width =260;

 var lblDesc = new Label { Text = "Description:", Left =10, Top =60, Width =80 };
 txtDescription.Left =100; txtDescription.Top =60; txtDescription.Width =260;

 btnOk.Text = "OK"; btnOk.Left =200; btnOk.Top =110; btnOk.Click += BtnOk_Click;
 btnCancel.Text = "Cancel"; btnCancel.Left =290; btnCancel.Top =110; btnCancel.Click += (s,e)=> this.DialogResult = DialogResult.Cancel;

 this.Controls.Add(lblName);
 this.Controls.Add(txtName);
 this.Controls.Add(lblDesc);
 this.Controls.Add(txtDescription);
 this.Controls.Add(btnOk);
 this.Controls.Add(btnCancel);
 }

 private void BtnOk_Click(object? sender, EventArgs e)
 {
 if (string.IsNullOrWhiteSpace(txtName.Text))
 {
 MessageBox.Show("Name is required", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
 return;
 }

 Category.Name = txtName.Text.Trim();
 Category.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim();
 this.DialogResult = DialogResult.OK;
 }
 }
}
