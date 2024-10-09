using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models;

namespace BusinessLogic.Repository
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Task<IEnumerable<Attendance>> GetMonthlyAttendanceReportAsync(int employeeId, int month, int year);
        Task RecordAttendanceAsync(int employeeId, DateTime attendanceDate, TimeSpan? checkInTime, TimeSpan? checkOutTime = null, bool isLeave = false, bool isOvertime = false);
    }

    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly EmployeeManagementContext _context;

        public AttendanceRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task RecordAttendanceAsync(int employeeId, DateTime attendanceDate, TimeSpan? checkInTime, TimeSpan? checkOutTime = null, bool isLeave = false, bool isOvertime = false)
        {
            int? hoursWorked = null;

            // Tính toán số giờ làm việc nếu có cả check-in và check-out
            if (checkInTime.HasValue && checkOutTime.HasValue)
            {
                hoursWorked = (int)(checkOutTime.Value - checkInTime.Value).TotalHours;
            }

            var attendance = new Attendance
            {
                UserId = employeeId,
                AttendanceDate = attendanceDate,
                CheckInTime = checkInTime,
                CheckOutTime = checkOutTime,
                HoursWorked = hoursWorked,
                IsLeave = isLeave,
                IsOvertime = isOvertime,
                CreatedAt = DateTime.Now
            };

            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
        }

        // Lấy báo cáo chấm công theo tháng
        public async Task<IEnumerable<Attendance>> GetMonthlyAttendanceReportAsync(int employeeId, int month, int year)
        {
            return await _context.Attendances
                                 .Where(a => a.UserId == employeeId && a.AttendanceDate.Month == month && a.AttendanceDate.Year == year)
                                 .OrderBy(a => a.AttendanceDate)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetAllAsync()
        {
            return await _context.Attendances.ToListAsync();
        }

        public async Task<Attendance> GetByIdAsync(int id)
        {
            return await _context.Attendances.FindAsync(id);
        }

        public async Task UpdateAsync(Attendance entity)
        {
            _context.Attendances.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
                await _context.SaveChangesAsync();
            }
        }

        public Task AddAsync(Attendance entity)
        {
            throw new NotImplementedException();
        }
    }
}
