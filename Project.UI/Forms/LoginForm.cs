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
 this.Width =380;
 this.Height =200;
 this.FormBorderStyle = FormBorderStyle.FixedDialog;
 this.StartPosition = FormStartPosition.CenterScreen;
 this.MaximizeBox = false;

 var lblUser = new Label { Text = "Username:", Left =20, Top =30, Width =70, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
 txtUsername.Left =100; txtUsername.Top =25; txtUsername.Width =240;
 var lblPass = new Label { Text = "Password:", Left =20, Top =70, Width =70, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
 txtPassword.Left =100; txtPassword.Top =65; txtPassword.Width =240; txtPassword.UseSystemPasswordChar = true;
 btnLogin.Text = "Login"; btnLogin.Left =240; btnLogin.Top =110; btnLogin.Width =100;
 btnLogin.Click += BtnLogin_Click;

 this.Controls.Add(lblUser);
 this.Controls.Add(txtUsername);
 this.Controls.Add(lblPass);
 this.Controls.Add(txtPassword);
 this.Controls.Add(btnLogin);

 this.AcceptButton = btnLogin;
 }

 private void BtnLogin_Click(object? sender, EventArgs e)
 {
 try
 {
 var user = _authService.Authenticate(txtUsername.Text.Trim(), txtPassword.Text);
 if (user == null)
 {
 MessageBox.Show("Invalid credentials", "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
 txtPassword.Clear();
 txtPassword.Focus();
 return;
 }


 var conn = Database.ConnectionString;
 var catRepo = new CategoryRepository(conn);
 var supRepo = new SupplierRepository(conn);
 var prodRepo = new ProductRepository(conn);
 var movRepo = new StockMovementRepository(conn);
 var activityRepo = new ActivityLogRepository(conn);

 var activityService = new ActivityLogService(activityRepo);
 var catService = new CategoryService(catRepo, prodRepo, activityService);
 var supService = new SupplierService(supRepo, prodRepo, activityService);
 var prodService = new ProductService(prodRepo, movRepo, activityService);
 var stockService = new StockService(movRepo, prodRepo, activityService);
 var reportService = new Project.Reports.ReportService(stockService, prodService);


 this.Hide();
 var main = new MainForm(user, catService, supService, prodService, stockService, reportService);
 main.ShowDialog();

 this.Show();
 
 txtPassword.Clear();
 txtPassword.Focus();
 }
 catch (Exception ex)
 {
 MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
 }
 }
 }
}
