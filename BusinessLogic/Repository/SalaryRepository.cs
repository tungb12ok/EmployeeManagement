using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public interface ISalaryRepository : IRepository<Salary>
    {
        Task<IEnumerable<Salary>> GetSalaryHistoryByEmployeeIdAsync(int employeeId);
        Task<decimal> CalculateTotalIncomeAsync(int employeeId);
    }

    public class SalaryRepository : ISalaryRepository
    {
        private readonly EmployeeManagementContext _context;

        public SalaryRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        // Add a new salary entry for an employee
        public async Task AddAsync(Salary entity)
        {
            await _context.Salaries.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var salary = await _context.Salaries.FindAsync(id);
            if (salary != null)
            {
                _context.Salaries.Remove(salary);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Salary>> GetAllAsync()
        {
            return await _context.Salaries.ToListAsync();
        }

        public async Task<Salary> GetByIdAsync(int id)
        {
            return await _context.Salaries.FindAsync(id);
        }

        public async Task<IEnumerable<Salary>> GetSalaryHistoryByEmployeeIdAsync(int employeeId)
        {
            return await _context.Salaries
                                 .Where(s => s.UserId == employeeId)
                                 .OrderByDescending(s => s.PaymentDate) 
                                 .ToListAsync();
        }

        public async Task<decimal> CalculateTotalIncomeAsync(int employeeId)
        {
            var salaries = await _context.Salaries
                                         .Where(s => s.UserId == employeeId)
                                         .ToListAsync();

            return salaries.Sum(s => s.TotalIncome ?? 0);
        }

        public async Task UpdateAsync(Salary entity)
        {
            var salary = await _context.Salaries.FindAsync(entity.SalaryId);
            if (salary != null)
            {
                salary.BaseSalary = entity.BaseSalary;
                salary.Allowance = entity.Allowance;
                salary.Bonus = entity.Bonus;
                salary.Deduction = entity.Deduction;
                salary.PaymentDate = entity.PaymentDate;
                salary.Remarks = entity.Remarks;

                _context.Salaries.Update(salary);
                await _context.SaveChangesAsync();
            }
        }
    }
}
