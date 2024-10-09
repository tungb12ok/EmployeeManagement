using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using BusinessLogic.Repository;
using DataAccess.Models;
using EmployeeManagement.ViewModels;
using EmployeeManagement.Views;

namespace EmployeeManagement
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Add DbContext
            services.AddDbContext<EmployeeManagementContext>();

            // Add repositories
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<ISalaryRepository, SalaryRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();

            // Add view models
            services.AddScoped<LoginViewModel>();

            // Add views
            services.AddScoped<LoginView>();
            services.AddScoped<MainWindow>(); 
            services.AddScoped<EmployeeFormWindow>(); 
            services.AddScoped<DepartmentManagementWindow>(); 
            services.AddScoped<SalaryManagementWindow>(); 
            services.AddScoped<AttendanceCheckWindow>(); 
            services.AddScoped<AttendanceReportWindow>(); 
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<LoginView>();
            if (mainWindow != null)
            {
                mainWindow.Show();
            }
            else
            {
            }
        }
    }
}
