using System;
using System.Linq;
using System.Windows.Forms;
using Project.Core.Models;
using Project.Services;

namespace Project.UI.Forms
{
 public class StockMovementForm : Form
 {
 private ComboBox cbProduct = new ComboBox();
 private ComboBox cbType = new ComboBox();
 private NumericUpDown numQuantity = new NumericUpDown();
 private TextBox txtNotes = new TextBox();
 private Button btnAdd = new Button();
 private Button btnCancel = new Button();

 private readonly ProductService _productService;
 private readonly StockService _stockService;
 private readonly int? _currentUserId;

 public StockMovementForm(ProductService productService, StockService stockService, int? currentUserId = null)
 {
 _productService = productService;
 _stockService = stockService;
 _currentUserId = currentUserId;
 InitializeComponents();
 LoadProducts();
 }

 private void InitializeComponents()
 {
 this.Text = "Add Stock Movement";
 this.Width =500;
 this.Height =300;
 this.FormBorderStyle = FormBorderStyle.FixedDialog;
 this.StartPosition = FormStartPosition.CenterParent;

 var lblProduct = new Label { Text = "Product:", Left =10, Top =20, Width =100 };
 cbProduct.Left =120; cbProduct.Top =20; cbProduct.Width =340; cbProduct.DropDownStyle = ComboBoxStyle.DropDownList;

 var lblType = new Label { Text = "Type:", Left =10, Top =60, Width =100 };
 cbType.Left =120; cbType.Top =60; cbType.Width =120;
 cbType.Items.AddRange(new string[] { "IN", "OUT" });
 cbType.SelectedIndex =0;

 var lblQty = new Label { Text = "Quantity:", Left =10, Top =100, Width =100 };
 numQuantity.Left =120; numQuantity.Top =100; numQuantity.Width =120; numQuantity.Minimum =1; numQuantity.Maximum =1000000;

 var lblNotes = new Label { Text = "Notes:", Left =10, Top =140, Width =100 };
 txtNotes.Left =120; txtNotes.Top =140; txtNotes.Width =340; txtNotes.Height =50; txtNotes.Multiline = true;

 btnAdd.Text = "Add"; btnAdd.Left =280; btnAdd.Top =210; btnAdd.Click += BtnAdd_Click;
 btnCancel.Text = "Cancel"; btnCancel.Left =360; btnCancel.Top =210; btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

 this.Controls.Add(lblProduct);
 this.Controls.Add(cbProduct);
 this.Controls.Add(lblType);
 this.Controls.Add(cbType);
 this.Controls.Add(lblQty);
 this.Controls.Add(numQuantity);
 this.Controls.Add(lblNotes);
 this.Controls.Add(txtNotes);
 this.Controls.Add(btnAdd);
 this.Controls.Add(btnCancel);
 }

 private void LoadProducts()
 {
 var products = _productService.GetAll().ToList();
 cbProduct.DataSource = products;
 cbProduct.DisplayMember = "Name";
 cbProduct.ValueMember = "Id";
 }

 private void BtnAdd_Click(object? sender, EventArgs e)
 {
 if (cbProduct.SelectedItem == null)
 {
 MessageBox.Show("Select a product", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
 return;
 }

 var product = cbProduct.SelectedItem as Product;
 var type = cbType.SelectedItem as string ?? "IN";
 var qty = (int)numQuantity.Value;

 try
 {
 var movement = new StockMovement
 {
 ProductId = product!.Id,
 MovementType = type,
 Quantity = qty,
 MovementDate = DateTime.Now,
 UserId = _currentUserId,
 Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim()
 };

 _stockService.AddMovement(movement);
 MessageBox.Show("Stock movement recorded.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
 this.DialogResult = DialogResult.OK;
 }
 catch (Exception ex)
 {
 MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
 }
 }
 }
}
