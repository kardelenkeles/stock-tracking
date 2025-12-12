using System;
using System.Windows.Forms;
using Project.Core.Models;
using Project.Services;
using Project.Reports;

namespace Project.UI.Forms
{
 public class MainForm : Form
 {
 private readonly User _currentUser;
 private readonly CategoryService _catService;
 private readonly SupplierService _supService;
 private readonly ProductService _prodService;
 private readonly StockService _stockService;
 private readonly ReportService _reportService;

 private Panel navPanel = new Panel();
 private Panel contentPanel = new Panel();
 private Button btnDashboard = new Button();
 private Button btnProducts = new Button();
 private Button btnCategories = new Button();
 private Button btnSuppliers = new Button();
 private Button btnStock = new Button();
 private Button btnReports = new Button();
 private Button btnUsers = new Button();
 private Label lblUserInfo = new Label();

 public MainForm(User user, CategoryService catService, SupplierService supService, ProductService prodService, StockService stockService, ReportService reportService)
 {
 _currentUser = user;
 _catService = catService;
 _supService = supService;
 _prodService = prodService;
 _stockService = stockService;
 _reportService = reportService;
 InitializeComponents();
 }

 private void InitializeComponents()
 {
 this.Text = "Smart Inventory System";
 this.WindowState = FormWindowState.Maximized;

 navPanel.Left =0; navPanel.Top =0; navPanel.Width =200; navPanel.Height = this.ClientSize.Height; navPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
 contentPanel.Left =200; contentPanel.Top =0; contentPanel.Width = this.ClientSize.Width -200; contentPanel.Height = this.ClientSize.Height; contentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

 btnDashboard.Text = "Dashboard"; btnDashboard.Left =10; btnDashboard.Top =20; btnDashboard.Width =180; btnDashboard.Click += (s,e)=> ShowDashboard();
 btnProducts.Text = "Products"; btnProducts.Left =10; btnProducts.Top =60; btnProducts.Width =180; btnProducts.Click += (s,e)=> OpenProducts();
 btnCategories.Text = "Categories"; btnCategories.Left =10; btnCategories.Top =100; btnCategories.Width =180; btnCategories.Click += (s,e)=> OpenCategories();
 btnSuppliers.Text = "Suppliers"; btnSuppliers.Left =10; btnSuppliers.Top =140; btnSuppliers.Width =180; btnSuppliers.Click += (s,e)=> OpenSuppliers();
 btnStock.Text = "Stock Movements"; btnStock.Left =10; btnStock.Top =180; btnStock.Width =180; btnStock.Click += (s,e)=> OpenStock();
 btnReports.Text = "Reports"; btnReports.Left =10; btnReports.Top =220; btnReports.Width =180; btnReports.Click += (s,e)=> OpenReports();
 btnUsers.Text = "Users"; btnUsers.Left =10; btnUsers.Top =300; btnUsers.Width =180; btnUsers.Click += (s,e)=> OpenUsers();

 lblUserInfo.Left =10; lblUserInfo.Top =260; lblUserInfo.Width =180; lblUserInfo.Text = $"User: {_currentUser.Username} ({_currentUser.Role})";

 navPanel.Controls.Add(btnDashboard); navPanel.Controls.Add(btnProducts); navPanel.Controls.Add(btnCategories); navPanel.Controls.Add(btnSuppliers); navPanel.Controls.Add(btnStock); navPanel.Controls.Add(btnReports); navPanel.Controls.Add(btnUsers); navPanel.Controls.Add(lblUserInfo);

 this.Controls.Add(navPanel); this.Controls.Add(contentPanel);

 // role-based visibility
 btnUsers.Visible = _currentUser.Role == "Admin";
 // make Categories and Suppliers admin-only
 btnCategories.Visible = _currentUser.Role == "Admin";
 btnSuppliers.Visible = _currentUser.Role == "Admin";
 btnProducts.Visible = _currentUser.Role == "Admin" || _currentUser.Role == "User";
 btnStock.Visible = true;
 btnReports.Visible = _currentUser.Role == "Admin";
 }

 private void ShowDashboard()
 {
 contentPanel.Controls.Clear();
 var dashboard = new DashboardControl(_catService, _prodService, _stockService);
 contentPanel.Controls.Add(dashboard);
 }

 private void OpenCategories()
 {
 var form = new CategoryListForm(_catService, _currentUser);
 form.TopLevel = false; form.FormBorderStyle = FormBorderStyle.None; form.Dock = DockStyle.Fill;
 contentPanel.Controls.Clear(); contentPanel.Controls.Add(form); form.Show();
 }

 private SupplierService CreateSupplierService()
 {
 var supRepo = new Project.Data.SupplierRepository(Project.Data.Database.ConnectionString);
 var prodRepo = new Project.Data.ProductRepository(Project.Data.Database.ConnectionString);
 var activityRepo = new Project.Data.ActivityLogRepository(Project.Data.Database.ConnectionString);
 var activityService = new ActivityLogService(activityRepo);
 return new SupplierService(supRepo, prodRepo, activityService);
 }

 private void OpenSuppliers()
 {
 var supService = CreateSupplierService();
 var form = new SupplierListForm(supService, _currentUser);
 form.TopLevel = false; form.FormBorderStyle = FormBorderStyle.None; form.Dock = DockStyle.Fill;
 contentPanel.Controls.Clear(); contentPanel.Controls.Add(form); form.Show();
 }

 private void OpenProducts()
 {
 var form = new ProductListForm(_prodService, _catService, _supService, _currentUser);
 form.TopLevel = false; form.FormBorderStyle = FormBorderStyle.None; form.Dock = DockStyle.Fill;
 contentPanel.Controls.Clear(); contentPanel.Controls.Add(form); form.Show();
 }

 private void OpenStock()
 {
 var form = new StockMovementListForm(_stockService, _prodService, _catService, _supService, _currentUser.Id);
 form.TopLevel = false; form.FormBorderStyle = FormBorderStyle.None; form.Dock = DockStyle.Fill;
 contentPanel.Controls.Clear(); contentPanel.Controls.Add(form); form.Show();
 }

 private void OpenReports()
 {
 var ctrl = new ReportsForm(_reportService, _prodService);
 ctrl.Dock = DockStyle.Fill;
 contentPanel.Controls.Clear(); contentPanel.Controls.Add(ctrl);
 }

 private void OpenUsers()
 {
 var userRepo = new Project.Data.UserRepository(Project.Data.Database.ConnectionString);
 var activityRepo = new Project.Data.ActivityLogRepository(Project.Data.Database.ConnectionString);
 var activityService = new ActivityLogService(activityRepo);
 var userService = new UserService(userRepo, activityService);
 var form = new UserListForm(userService, _currentUser.Id);
 form.TopLevel = false; form.FormBorderStyle = FormBorderStyle.None; form.Dock = DockStyle.Fill;
 contentPanel.Controls.Clear(); contentPanel.Controls.Add(form); form.Show();
 }
 }
}
