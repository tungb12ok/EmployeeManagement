using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using BusinessLogic.Repository;
using DataAccess.Models;

namespace EmployeeManagement
{
    public partial class EmployeeFormWindow : Window
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private User _employee; // Employee being edited or added
        private byte[] _avatarBytes; // Byte array to store profile picture

        public EmployeeFormWindow(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, User employee = null)
        {
            InitializeComponent();
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _employee = employee;

            LoadDepartments();
            if (_employee != null)
            {
                LoadEmployeeData();
                HidePasswordField(); // Ẩn trường Password nếu đang chỉnh sửa nhân viên
            }
            else
            {
                ShowPasswordField(); // Hiển thị trường Password nếu đang thêm mới nhân viên
            }
        }

        // Ẩn trường Password nếu đang chỉnh sửa nhân viên
        private void HidePasswordField()
        {
            lblPassword.Visibility = Visibility.Collapsed;
            txtPassword.Visibility = Visibility.Collapsed;
        }

        // Hiển thị trường Password nếu đang thêm mới nhân viên
        private void ShowPasswordField()
        {
            lblPassword.Visibility = Visibility.Visible;
            txtPassword.Visibility = Visibility.Visible;
        }

        // Load departments into the ComboBox
        private async void LoadDepartments()
        {
            var departments = await _departmentRepository.GetDepartmentsAsync();
            cbDepartment.ItemsSource = departments;
        }

        // Load existing employee data if editing
        private void LoadEmployeeData()
        {
            txtFullName.Text = _employee.FullName;
            txtUsername.Text = _employee.Username;
            dpDateOfBirth.SelectedDate = _employee.DateOfBirth;
            rbMale.IsChecked = _employee.Gender == true;
            rbFemale.IsChecked = _employee.Gender == false;
            txtAddress.Text = _employee.Address;
            txtPhoneNumber.Text = _employee.PhoneNumber;
            cbDepartment.SelectedItem = _employee.Department;
            txtPosition.Text = _employee.Position;
            txtSalary.Text = _employee.Salary.ToString();
            dpStartDate.SelectedDate = _employee.StartDate;

            // Load avatar (ProfilePicture is byte[])
            if (_employee.ProfilePicture != null && _employee.ProfilePicture.Length > 0)
            {
                imgAvatar.Source = LoadImage(_employee.ProfilePicture);
            }
        }

        // Convert byte[] to BitmapImage for display
        private BitmapImage LoadImage(byte[] imageData)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                }
                return image;
            }
            catch
            {
                return null;
            }
        }

        // Upload avatar button click (convert image to byte[])
        private void UploadAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _avatarBytes = File.ReadAllBytes(openFileDialog.FileName); // Convert image to byte[]
                imgAvatar.Source = new BitmapImage(new Uri(openFileDialog.FileName)); // Display the image
            }
        }

        // Save button click (store the profile picture as byte[])
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_employee == null) // Adding a new employee
                {
                    _employee = new User();
                    _employee.PasswordHash = txtPassword.Password; // Lưu mật khẩu mới
                }

                _employee.FullName = txtFullName.Text;
                _employee.Username = txtUsername.Text;
                _employee.DateOfBirth = dpDateOfBirth.SelectedDate.Value;
                _employee.Gender = rbMale.IsChecked == true;
                _employee.Address = txtAddress.Text;
                _employee.PhoneNumber = txtPhoneNumber.Text;
                _employee.DepartmentId = (cbDepartment.SelectedItem as Department).DepartmentId;
                _employee.Position = txtPosition.Text;
                _employee.Salary = decimal.Parse(txtSalary.Text);
                _employee.StartDate = dpStartDate.SelectedDate.Value;

                // Save profile picture as byte[] (only if new avatar is selected)
                if (_avatarBytes != null)
                {
                    _employee.ProfilePicture = _avatarBytes;
                }

                if (_employee.UserId == 0) // New employee
                {
                    await _employeeRepository.AddAsync(_employee);
                }
                else // Editing existing employee
                {
                    await _employeeRepository.UpdateAsync(_employee);
                }

                MessageBox.Show("Employee saved successfully!");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving employee: {ex.Message}");
            }
        }

        // Cancel button click
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
