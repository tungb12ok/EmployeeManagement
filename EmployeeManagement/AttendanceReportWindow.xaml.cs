using System.Windows;
using BusinessLogic.Repository;
using System.Threading.Tasks;

namespace EmployeeManagement
{
    public partial class AttendanceReportWindow : Window
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly int _employeeId;

        public AttendanceReportWindow(IAttendanceRepository attendanceRepository, int employeeId)
        {
            InitializeComponent();
            _attendanceRepository = attendanceRepository;
            _employeeId = employeeId;

            LoadAttendanceReport();
        }

        // Tải báo cáo chấm công
        private async Task LoadAttendanceReport()
        {
            var report = await _attendanceRepository.GetMonthlyAttendanceReportAsync(_employeeId, DateTime.Now.Month, DateTime.Now.Year);
            dgAttendanceReport.ItemsSource = report;
        }
    }
}
