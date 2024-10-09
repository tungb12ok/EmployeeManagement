using BusinessLogic.Dto;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Repository
{
    public interface IReportRepository
    {
        Task<IEnumerable<DepartmentStatistics>> GetEmployeeStatisticsByDepartmentAsync();
        Task<IEnumerable<SalaryStatistics>> GetSalaryStatisticsByTimePeriodAsync(DateTime startDate, DateTime endDate);
        Task ExportReportToExcelAsync(string reportType, IEnumerable<object> data);
        Task ExportReportToPdfAsync(string reportType, IEnumerable<object> data);
    }

    public class ReportRepository : IReportRepository
    {
        private readonly EmployeeManagementContext _context;

        public ReportRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentStatistics>> GetEmployeeStatisticsByDepartmentAsync()
        {
            var statistics = await _context.Departments
                .Select(d => new DepartmentStatistics
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    EmployeeCount = _context.Users.Count(u => u.DepartmentId == d.DepartmentId),
                    MaleEmployeeCount = _context.Users.Count(u => u.DepartmentId == d.DepartmentId && u.Gender == true), 
                    FemaleEmployeeCount = _context.Users.Count(u => u.DepartmentId == d.DepartmentId && u.Gender == false) 
                })
                .ToListAsync();

            return statistics;
        }

        public async Task<IEnumerable<SalaryStatistics>> GetSalaryStatisticsByTimePeriodAsync(DateTime startDate, DateTime endDate)
        {
            var salaryStats = await _context.Salaries
                .Where(s => s.PaymentDate >= startDate && s.PaymentDate <= endDate)
                .Select(s => new SalaryStatistics
                {
                    EmployeeId = s.UserId,
                    FullName = _context.Users.Where(u => u.UserId == s.UserId).Select(u => u.FullName).FirstOrDefault(),
                    BaseSalary = s.BaseSalary,
                    TotalAllowance = s.Allowance ?? 0,
                    TotalBonus = s.Bonus ?? 0,
                    TotalDeduction = s.Deduction ?? 0,
                    TotalIncome = s.TotalIncome ?? 0,
                    PaymentDate = s.PaymentDate
                })
                .ToListAsync();

            return salaryStats;
        }

        public Task ExportReportToExcelAsync(string reportType, IEnumerable<object> data)
        {
            throw new NotImplementedException("Excel export is not implemented yet.");
        }

        public Task ExportReportToPdfAsync(string reportType, IEnumerable<object> data)
        {
            throw new NotImplementedException("PDF export is not implemented yet.");
        }
    }
}
