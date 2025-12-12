using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Project.Core.Models;

namespace Project.UI.Forms
{
 public class ProductEditForm : Form
 {
 private TextBox txtName = new TextBox();
 private ComboBox cbCategory = new ComboBox();
 private ComboBox cbSupplier = new ComboBox();
 private NumericUpDown numQuantity = new NumericUpDown();
 private NumericUpDown numMinQty = new NumericUpDown();
 private NumericUpDown numPurchase = new NumericUpDown();
 private NumericUpDown numSell = new NumericUpDown();
 private Button btnOk = new Button();
 private Button btnCancel = new Button();

 public Product Product { get; private set; }

 public ProductEditForm(Product? product = null, IEnumerable<Category>? categories = null, IEnumerable<Supplier>? suppliers = null)
 {
 Product = product ?? new Product();
 InitializeComponents();
 if (categories != null) { cbCategory.DataSource = categories.ToList(); cbCategory.DisplayMember = "Name"; cbCategory.ValueMember = "Id"; cbCategory.DropDownStyle = ComboBoxStyle.DropDownList; }
 if (suppliers != null) { cbSupplier.DataSource = suppliers.ToList(); cbSupplier.DisplayMember = "Name"; cbSupplier.ValueMember = "Id"; cbSupplier.DropDownStyle = ComboBoxStyle.DropDownList; }
 if (product != null)
 {
 txtName.Text = Product.Name;
 if (Product.CategoryId.HasValue && cbCategory.Items.Count>0) cbCategory.SelectedItem = (cbCategory.Items.Cast<Category>().FirstOrDefault(c=>c.Id==Product.CategoryId));
 if (Product.SupplierId.HasValue && cbSupplier.Items.Count>0) cbSupplier.SelectedItem = (cbSupplier.Items.Cast<Supplier>().FirstOrDefault(s=>s.Id==Product.SupplierId));
 numQuantity.Value = Product.Quantity;
 numMinQty.Value = Product.MinQuantity;
 if (Product.PurchasePrice.HasValue) numPurchase.Value = Product.PurchasePrice.Value;
 if (Product.SellPrice.HasValue) numSell.Value = Product.SellPrice.Value;
 }
 }

 private void InitializeComponents()
 {
 this.Text = "Product";
 this.Width =600;
 this.Height =350;
 this.FormBorderStyle = FormBorderStyle.FixedDialog;
 this.StartPosition = FormStartPosition.CenterParent;
 this.MaximizeBox = false;

 var lblName = new Label { Text = "Name:", Left =10, Top =20, Width =120 };
 txtName.Left =140; txtName.Top =20; txtName.Width =420;
 var lblCat = new Label { Text = "Category:", Left =10, Top =60, Width =120 };
 cbCategory.Left =140; cbCategory.Top =60; cbCategory.Width =220;
 var lblSup = new Label { Text = "Supplier:", Left =10, Top =100, Width =120 };
 cbSupplier.Left =140; cbSupplier.Top =100; cbSupplier.Width =220;
 var lblQty = new Label { Text = "Quantity:", Left =10, Top =140, Width =120 };
 numQuantity.Left =140; numQuantity.Top =140; numQuantity.Width =120; numQuantity.Minimum =0; numQuantity.Maximum =1000000;
 var lblMin = new Label { Text = "Min Quantity:", Left =300, Top =140, Width =120 };
 numMinQty.Left =420; numMinQty.Top =140; numMinQty.Width =120; numMinQty.Minimum =0; numMinQty.Maximum =100000;
 var lblPurchase = new Label { Text = "Purchase Price:", Left =10, Top =180, Width =120 };
 numPurchase.Left =140; numPurchase.Top =180; numPurchase.DecimalPlaces =2; numPurchase.Maximum =1000000;
 var lblSell = new Label { Text = "Sell Price:", Left =300, Top =180, Width =120 };
 numSell.Left =420; numSell.Top =180; numSell.DecimalPlaces =2; numSell.Maximum =1000000;

 btnOk.Text = "OK"; btnOk.Left =380; btnOk.Top =260; btnOk.Click += BtnOk_Click;
 btnCancel.Text = "Cancel"; btnCancel.Left =480; btnCancel.Top =260; btnCancel.Click += (s,e)=> this.DialogResult = DialogResult.Cancel;

 this.Controls.Add(lblName); this.Controls.Add(txtName);
 this.Controls.Add(lblCat); this.Controls.Add(cbCategory);
 this.Controls.Add(lblSup); this.Controls.Add(cbSupplier);
 this.Controls.Add(lblQty); this.Controls.Add(numQuantity);
 this.Controls.Add(lblMin); this.Controls.Add(numMinQty);
 this.Controls.Add(lblPurchase); this.Controls.Add(numPurchase);
 this.Controls.Add(lblSell); this.Controls.Add(numSell);
 this.Controls.Add(btnOk); this.Controls.Add(btnCancel);
 }

 private void BtnOk_Click(object? sender, EventArgs e)
 {
 if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Name required","Validation",MessageBoxButtons.OK,MessageBoxIcon.Warning); return; }
 Product.Name = txtName.Text.Trim();
 Product.CategoryId = cbCategory.SelectedItem is Category c ? c.Id : (int?)null;
 Product.SupplierId = cbSupplier.SelectedItem is Supplier s ? s.Id : (int?)null;
 Product.Quantity = (int)numQuantity.Value;
 Product.MinQuantity = (int)numMinQty.Value;
 Product.PurchasePrice = numPurchase.Value;
 Product.SellPrice = numSell.Value;
 this.DialogResult = DialogResult.OK;
 }
 }
}
