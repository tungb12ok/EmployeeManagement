using System;
using System.Windows;
using BusinessLogic.Repository; 
using DataAccess.Models; 
using System.Threading.Tasks;

namespace EmployeeManagement
{
    public partial class DepartmentManagementWindow : Window
    {
        private readonly IDepartmentRepository _departmentRepository;
        private Department _selectedDepartment; 

        public DepartmentManagementWindow(IDepartmentRepository departmentRepository)
        {
            InitializeComponent();
            _departmentRepository = departmentRepository;
            LoadDepartments(); 
        }

        // Load all departments
        private async Task LoadDepartments()
        {
            var departments = await _departmentRepository.GetAllAsync();
            dgDepartments.ItemsSource = departments;
        }

        // Add new department
        private async void AddDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDepartmentName.Text))
            {
                var newDepartment = new Department
                {
                    DepartmentName = txtDepartmentName.Text,
                    CreatedAt = DateTime.Now
                };

                await _departmentRepository.AddAsync(newDepartment);
                await LoadDepartments(); // Tải lại danh sách phòng ban sau khi thêm
                ClearInput();
            }
            else
            {
                MessageBox.Show("Please enter a department name.");
            }
        }

        // Update selected department
        private async void UpdateDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDepartment != null && !string.IsNullOrEmpty(txtDepartmentName.Text))
            {
                _selectedDepartment.DepartmentName = txtDepartmentName.Text;
                await _departmentRepository.UpdateAsync(_selectedDepartment);
                await LoadDepartments(); // Tải lại danh sách phòng ban sau khi cập nhật
                ClearInput();
            }
            else
            {
                MessageBox.Show("Please select a department and enter a new name.");
            }
        }

        // Delete selected department
        private async void DeleteDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDepartment != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {_selectedDepartment.DepartmentName}?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    await _departmentRepository.DeleteAsync(_selectedDepartment.DepartmentId);
                    await LoadDepartments(); // Tải lại danh sách phòng ban sau khi xóa
                    ClearInput();
                }
            }
            else
            {
                MessageBox.Show("Please select a department to delete.");
            }
        }

        // When a department is selected in DataGrid, show its name in TextBox
        private void DgDepartments_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgDepartments.SelectedItem is Department department)
            {
                _selectedDepartment = department;
                txtDepartmentName.Text = department.DepartmentName;
            }
        }
        // Method to refresh the list of departments
        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadDepartments();
            ClearInput();
        }

        // Clear input fields
        private void ClearInput()
        {
            txtDepartmentName.Clear();
            _selectedDepartment = null;
        }
    }
}
