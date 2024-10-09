using System;
using System.Windows;
using BusinessLogic.Repository;
using DataAccess.Models;
using System.Threading.Tasks;

namespace EmployeeManagement
{
    public partial class AttendanceCheckWindow : Window
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly int _employeeId;
        private string _employeeName;

        private bool _isCheckedIn = false;
        private DateTime _checkInTime;

        public AttendanceCheckWindow(IAttendanceRepository attendanceRepository, IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, int employeeId)
        {
            InitializeComponent();
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _employeeId = employeeId;

            // Lấy thông tin nhân viên từ ID và hiển thị
            LoadEmployeeInfo();
        }

        // Hàm để tải thông tin nhân viên từ employeeId
        private async void LoadEmployeeInfo()
        {
            var employee = await _employeeRepository.GetByIdAsync(_employeeId);
            if (employee != null)
            {
                _employeeName = employee.FullName;
                txtEmployeeName.Text = $"Employee: {_employeeName}"; // Hiển thị tên nhân viên
            }
            else
            {
                MessageBox.Show("Employee not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xử lý Check-In
        private async void CheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (!_isCheckedIn)
            {
                _checkInTime = DateTime.Now; // Ghi nhận thời gian Check-In
                await _attendanceRepository.RecordAttendanceAsync(_employeeId, _checkInTime.Date, _checkInTime.TimeOfDay, null, false, false);

                MessageBox.Show("Check-In successful!", "Attendance", MessageBoxButton.OK, MessageBoxImage.Information);
                _isCheckedIn = true; // Đánh dấu nhân viên đã Check-In
            }
            else
            {
                MessageBox.Show("You have already checked in.", "Attendance", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Xử lý Check-Out
        private async void CheckOut_Click(object sender, RoutedEventArgs e)
        {
            if (_isCheckedIn)
            {
                DateTime checkOutTime = DateTime.Now; // Ghi nhận thời gian Check-Out

                // Ghi nhận Check-Out và cập nhật số giờ làm việc
                await _attendanceRepository.RecordAttendanceAsync(_employeeId, _checkInTime.Date, _checkInTime.TimeOfDay, checkOutTime.TimeOfDay, false, false);

                MessageBox.Show("Check-Out successful!", "Attendance", MessageBoxButton.OK, MessageBoxImage.Information);
                _isCheckedIn = false; // Reset trạng thái
            }
            else
            {
                MessageBox.Show("Please Check-In first.", "Attendance", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Xử lý hiển thị báo cáo chấm công
        private async void ViewReport_Click(object sender, RoutedEventArgs e)
        {
            // Giả sử ta có một cửa sổ riêng để hiển thị báo cáo
            var reportWindow = new AttendanceReportWindow(_attendanceRepository, _employeeId);
            reportWindow.Show();
        }

        // Xử lý xem thông tin cá nhân
        private async void ViewProfile_Click(object sender, RoutedEventArgs e)
        {
            var employee = await _employeeRepository.GetByIdAsync(_employeeId);

            if (employee != null)
            {
                // Mở cửa sổ EmployeeFormWindow với thông tin nhân viên đã chọn
                var editEmployeeWindow = new EmployeeFormWindow(_employeeRepository, _departmentRepository, employee); // Sửa lại biến thành employee thay vì selectedEmployee
                if (editEmployeeWindow.ShowDialog() == true)
                {
                    MessageBox.Show("Profile updated successfully.", "Profile", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Employee profile not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
