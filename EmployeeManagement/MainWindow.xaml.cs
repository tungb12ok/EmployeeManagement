using System;
using System.Windows;
using System.Threading.Tasks;
using BusinessLogic.Repository;
using DataAccess.Models;
using EmployeeManagement.Views;
namespace EmployeeManagement
{
    public partial class MainWindow : Window
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public MainWindow(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            InitializeComponent();
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;

            // Initialize data
            InitializeAsync();
        }

        // Asynchronous initialization method
        private async void InitializeAsync()
        {
            await LoadDepartmentsAsync();
            await LoadEmployeesAsync();
        }

        // Load all departments
        private async Task LoadDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetDepartmentsAsync();
            cbDepartmentFilter.ItemsSource = departments;
            cbDepartmentFilter.SelectedIndex = 0;
        }

        // Load all employees
        private async Task LoadEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            dgEmployees.ItemsSource = employees;
        }

        // Search button click
        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            var searchText = txtSearch.Text;
            var selectedDepartment = cbDepartmentFilter.SelectedItem as Department;
            int? departmentId = selectedDepartment?.DepartmentId;

            var employees = await _employeeRepository.FilterByCriteriaAsync(searchText, departmentId, null, null, null);
            dgEmployees.ItemsSource = employees;
        }

        // Add Employee button click
        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var addEmployeeWindow = new EmployeeFormWindow(_employeeRepository, _departmentRepository, null);
            if (addEmployeeWindow.ShowDialog() == true)
            {
                LoadEmployeesAsync(); // Refresh the employee list after adding
            }
        }

        // Edit Employee button click
        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem is User selectedEmployee)
            {
                var editEmployeeWindow = new EmployeeFormWindow(_employeeRepository, _departmentRepository, selectedEmployee);
                if (editEmployeeWindow.ShowDialog() == true)
                {
                    LoadEmployeesAsync(); // Refresh the employee list after editing
                }
            }
            else
            {
                MessageBox.Show("Please select an employee to edit.");
            }
        }

        // Delete Employee button click
        private async void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem is User selectedEmployee)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {selectedEmployee.FullName}?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    await _employeeRepository.DeleteAsync(selectedEmployee.UserId);
                    await LoadEmployeesAsync(); // Refresh the list after deletion
                }
            }
            else
            {
                MessageBox.Show("Please select an employee to delete.");
            }
        }
    }
}
