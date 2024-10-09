using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BusinessLogic.Repository;
using EmployeeManagement.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private  readonly IDepartmentRepository _departmentRepository;
        private readonly IServiceProvider _serviceProvider;
        private string _username;
        private string _password;
        private string _loginStatus;

        // Property to hold username input
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                RaiseCanExecuteChanged();
            }
        }

        // Property to hold password input
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                // No need to raise PropertyChanged for sensitive data like Password
                RaiseCanExecuteChanged();
            }
        }

        // Property to hold the login status message
        public string LoginStatus
        {
            get => _loginStatus;
            set
            {
                _loginStatus = value;
                OnPropertyChanged(nameof(LoginStatus));
            }
        }

        // ICommand property for login command
        public ICommand LoginCommand { get; }

        // Constructor with dependencies injected
        public LoginViewModel(IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, IAttendanceRepository attendanceRepository, IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _serviceProvider = serviceProvider; // Inject the service provider
            LoginCommand = new RelayCommand(async () => await LoginAsync(), CanExecuteLogin);
            _attendanceRepository = attendanceRepository;
            _departmentRepository = departmentRepository;
        }

        // Determines if the Login button can be clicked
        private bool CanExecuteLogin()
        {
            // Check if both Username and Password are provided
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private async Task LoginAsync()
        {
            try
            {
                var user = await _employeeRepository.Authentication(Username, Password);
                if (user != null)
                {
                    if (user.Role.RoleName == "Admin")
                    {
                        LoginStatus = "Admin đăng nhập thành công!";

                        // Mở cửa sổ MainWindow cho Admin thông qua DI
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var mainWindow = _serviceProvider.GetService<MainWindow>(); // Get MainWindow via DI
                            if (mainWindow != null)
                            {
                                mainWindow.Show();
                            }
                            else
                            {
                                MessageBox.Show("MainWindow is not registered in the service collection.");
                            }

                            // Đóng cửa sổ hiện tại (cửa sổ login)
                            Application.Current.MainWindow.Close();
                        });
                    }
                    else if (user.Role.RoleName == "Employee")
                    {
                        LoginStatus = "Nhân viên đăng nhập thành công!";

                        // Mở cửa sổ AttendanceReportWindow cho Employee
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            // Tạo thủ công AttendanceReportWindow và truyền employeeId
                            var attendanceWindow = new AttendanceCheckWindow(_attendanceRepository, _employeeRepository, _departmentRepository, user.UserId);
                            attendanceWindow.Show();

                            // Đóng cửa sổ hiện tại (cửa sổ login)
                            Application.Current.MainWindow.Close();
                        });
                    }
                }
                else
                {
                    LoginStatus = "Tên đăng nhập hoặc mật khẩu không đúng!";
                }
            }
            catch (Exception ex)
            {
                LoginStatus = $"Đã xảy ra lỗi: {ex.Message}";
            }
        }


        // Event to notify changes in properties
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Method to force re-evaluation of CanExecute for LoginCommand
        private void RaiseCanExecuteChanged()
        {
            if (LoginCommand is RelayCommand loginCommand)
            {
                loginCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
