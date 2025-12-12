// See https://aka.ms/new-console-template for more information
using System;
using System.Windows.Forms;
using Project.UI.Forms;
using Project.Data;
using Project.Services;

namespace visual_project
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            try
            {
                // Initialize database and ensure tables exist
                Database.Initialize("app.db");

                // Create repositories and services
                var userRepo = new UserRepository(Database.ConnectionString);
                var authService = new AuthService(userRepo);

                // Ensure default admin exists
                authService.EnsureDefaultAdmin();

                Application.Run(new LoginForm(authService));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fatal error during startup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
