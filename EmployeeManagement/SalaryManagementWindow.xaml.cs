using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using BusinessLogic.Repository;
using DataAccess.Models;
using System.Threading.Tasks;

namespace EmployeeManagement
{
    public partial class SalaryManagementWindow : Window
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISalaryRepository _salaryRepository;
        private List<User> _employees; // Danh sách nhân viên gốc
        private User _selectedEmployee; // Nhân viên được chọn

        public SalaryManagementWindow(IEmployeeRepository employeeRepository, ISalaryRepository salaryRepository)
        {
            InitializeComponent();
            _employeeRepository = employeeRepository;
            _salaryRepository = salaryRepository;
            LoadEmployees(); // Load all employees when the window loads
        }

        // Load tất cả nhân viên vào ComboBox
        private async Task LoadEmployees()
        {
            _employees = (await _employeeRepository.GetAllAsync()).ToList();
            cbEmployees.ItemsSource = _employees;
        }

        // Khi người dùng nhập văn bản vào ô tìm kiếm
        private void SearchEmployee_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var searchText = txtSearchEmployee.Text.Trim().ToLower();

            if (!string.IsNullOrEmpty(searchText))
            {
                var filteredEmployees = _employees.Where(emp => emp.FullName.ToLower().Contains(searchText)).ToList();
                cbEmployees.ItemsSource = filteredEmployees; // Cập nhật ComboBox với danh sách đã lọc
            }
            else
            {
                cbEmployees.ItemsSource = _employees; // Hiển thị tất cả nhân viên nếu không có tìm kiếm
            }
        }

        // Khi người dùng chọn nhân viên từ ComboBox
        private async void CbEmployees_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbEmployees.SelectedItem is User employee)
            {
                _selectedEmployee = employee;

                // Tải lịch sử lương của nhân viên
                var salaryHistory = await _salaryRepository.GetSalaryHistoryByEmployeeIdAsync(_selectedEmployee.UserId);
                dgSalaryHistory.ItemsSource = salaryHistory;
            }
        }

        // Lưu thông tin lương cho nhân viên đã chọn
        private async void SaveSalary_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null)
            {
                MessageBox.Show("Please select an employee.");
                return;
            }

            // Tạo một mục lương mới
            var salary = new Salary
            {
                UserId = _selectedEmployee.UserId,
                BaseSalary = decimal.Parse(txtBaseSalary.Text),
                Allowance = string.IsNullOrEmpty(txtAllowance.Text) ? (decimal?)null : decimal.Parse(txtAllowance.Text),
                Bonus = string.IsNullOrEmpty(txtBonus.Text) ? (decimal?)null : decimal.Parse(txtBonus.Text),
                Deduction = string.IsNullOrEmpty(txtDeduction.Text) ? (decimal?)null : decimal.Parse(txtDeduction.Text),
                Remarks = txtRemarks.Text,
                PaymentDate = DateTime.Now
            };

            // Tính tổng thu nhập
            salary.TotalIncome = salary.BaseSalary + (salary.Allowance ?? 0) + (salary.Bonus ?? 0) - (salary.Deduction ?? 0);

            await _salaryRepository.AddAsync(salary);

            // Làm mới lịch sử lương sau khi lưu
            var salaryHistory = await _salaryRepository.GetSalaryHistoryByEmployeeIdAsync(_selectedEmployee.UserId);
            dgSalaryHistory.ItemsSource = salaryHistory;
        }

        // Xóa mục lương đã chọn
        private async void DeleteSalary_Click(object sender, RoutedEventArgs e)
        {
            if (dgSalaryHistory.SelectedItem is Salary selectedSalary)
            {
                await _salaryRepository.DeleteAsync(selectedSalary.SalaryId);

                // Làm mới lịch sử lương sau khi xóa
                var salaryHistory = await _salaryRepository.GetSalaryHistoryByEmployeeIdAsync(_selectedEmployee.UserId);
                dgSalaryHistory.ItemsSource = salaryHistory;
            }
            else
            {
                MessageBox.Show("Please select a salary record to delete.");
            }
        }
    }
}
