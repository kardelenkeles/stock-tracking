using System;
using System.Windows.Forms;
using Project.Services;
using Project.Core.Models;
using Project.Data;

namespace Project.UI.Forms
{
 public class LoginForm : Form
 {
 private readonly AuthService _authService;
 private TextBox txtUsername = new TextBox();
 private TextBox txtPassword = new TextBox();
 private Button btnLogin = new Button();

 public LoginForm(AuthService authService)
 {
 _authService = authService;
 InitializeComponents();
 }

 private void InitializeComponents()
 {
 this.Text = "Login";
 this.Width =300;
 this.Height =200;
 this.FormBorderStyle = FormBorderStyle.FixedDialog;
 this.StartPosition = FormStartPosition.CenterScreen;

 var lblUser = new Label { Text = "Username:", Left =10, Top =20 };
 txtUsername.Left =100; txtUsername.Top =20; txtUsername.Width =150;
 var lblPass = new Label { Text = "Password:", Left =10, Top =60 };
 txtPassword.Left =100; txtPassword.Top =60; txtPassword.Width =150; txtPassword.UseSystemPasswordChar = true;
 btnLogin.Text = "Login"; btnLogin.Left =100; btnLogin.Top =100;
 btnLogin.Click += BtnLogin_Click;

 this.Controls.Add(lblUser);
 this.Controls.Add(txtUsername);
 this.Controls.Add(lblPass);
 this.Controls.Add(txtPassword);
 this.Controls.Add(btnLogin);
 }

 private void BtnLogin_Click(object? sender, EventArgs e)
 {
 try
 {
 var user = _authService.Authenticate(txtUsername.Text.Trim(), txtPassword.Text);
 if (user == null)
 {
 MessageBox.Show("Invalid credentials", "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
 return;
 }

 // Create repositories and services to pass to main form
 var conn = Database.ConnectionString;
 var catRepo = new CategoryRepository(conn);
 var supRepo = new SupplierRepository(conn);
 var prodRepo = new ProductRepository(conn);
 var movRepo = new StockMovementRepository(conn);
 var activityRepo = new ActivityLogRepository(conn);

 var activityService = new ActivityLogService(activityRepo);
 var catService = new CategoryService(catRepo, prodRepo);
 var supService = new SupplierService(supRepo, prodRepo, activityService);
 var prodService = new ProductService(prodRepo);
 var stockService = new StockService(movRepo, prodRepo);
 var reportService = new Project.Reports.ReportService(stockService, prodService);

 // Open main form
 var main = new MainForm(user, catService, supService, prodService, stockService, reportService);
 this.Hide();
 main.ShowDialog();
 this.Close();
 }
 catch (Exception ex)
 {
 MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
 }
 }
 }
}
