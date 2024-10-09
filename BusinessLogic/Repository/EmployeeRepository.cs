using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Util;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Repository
{
    public interface IEmployeeRepository : IRepository<User>
    {
        Task<IEnumerable<User>> SearchByNameAsync(string name);
        Task<IEnumerable<User>> FilterByDepartmentAsync(int departmentId);
        Task<IEnumerable<User>> FilterByPositionAsync(string position);
        Task<IEnumerable<User>> FilterByCriteriaAsync(string name, int? departmentId, string position, decimal? minSalary, decimal? maxSalary);
        Task<User> GetEmployeeDetailsAsync(int id);
        Task<User> Authentication(string username, string password);
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeManagementContext _context;

        public EmployeeRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        // Add a new 
        public async Task AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // Delete an employee by ID
        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users
                .FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        // Get all employees
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(x => x.Role)
                .Include(x => x.Department)
                .ToListAsync();
        }

        // Get employee by ID
        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users
                 .Include(x => x.Role)
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.UserId == id);
        }

        // Get detailed information for an employee
        public async Task<User> GetEmployeeDetailsAsync(int id)
        {
            return await _context.Users
                                 .Include(u => u.Department)
                                 .Include(u => u.Role)
                                 .FirstOrDefaultAsync(u => u.UserId == id);
        }

        // Search employees by name
        public async Task<IEnumerable<User>> SearchByNameAsync(string name)
        {
            return await _context.Users
                                 .Include(x => x.Role)
                                 .Include(x => x.Department)
                                 .Where(u => u.FullName.Contains(name))
                                 .ToListAsync();
        }

        // Filter employees by department
        public async Task<IEnumerable<User>> FilterByDepartmentAsync(int departmentId)
        {
            return await _context.Users
                                    .Include(x => x.Role)
                                    .Include(x => x.Department)
                                     .Where(u => u.DepartmentId == departmentId)
                                     .ToListAsync();
        }

        // Filter employees by position
        public async Task<IEnumerable<User>> FilterByPositionAsync(string position)
        {
            return await _context.Users
                 .Include(x => x.Role)
                .Include(x => x.Department)
                                 .Where(u => u.Position == position)
                                 .ToListAsync();
        }

        // Filter employees by various criteria
        public async Task<IEnumerable<User>> FilterByCriteriaAsync(string name, int? departmentId, string position, decimal? minSalary, decimal? maxSalary)
        {
            var query = _context.Users
                .Include(x => x.Role)
                .Include(x => x.Department)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.FullName.Contains(name));
            }
            if (departmentId.HasValue)
            {
                query = query.Where(u => u.DepartmentId == departmentId.Value);
            }
            if (!string.IsNullOrEmpty(position))
            {
                query = query.Where(u => u.Position == position);
            }
            if (minSalary.HasValue)
            {
                query = query.Where(u => u.Salary >= minSalary.Value);
            }
            if (maxSalary.HasValue)
            {
                query = query.Where(u => u.Salary <= maxSalary.Value);
            }

            return await query.ToListAsync();
        }

        // Update an existing employee
        public async Task UpdateAsync(User entity)
        {
            var user = await _context.Users.FindAsync(entity.UserId);
            if (user != null)
            {
                user.FullName = entity.FullName;
                user.DateOfBirth = entity.DateOfBirth;
                user.Gender = entity.Gender;
                user.Address = entity.Address;
                user.PhoneNumber = entity.PhoneNumber;
                user.DepartmentId = entity.DepartmentId;
                user.Position = entity.Position;
                user.Salary = entity.Salary;
                user.ProfilePicture = entity.ProfilePicture;
                user.StartDate = entity.StartDate;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> Authentication(string username, string password)
        {
            return await _context.Users
                 .Include(x => x.Role)
                .Include(x => x.Department)
                //.FirstOrDefaultAsync(x => x.Username == username && x.PasswordHash == ComputeSha256Hash.ComputeSha256HashPassword(password));
                .FirstOrDefaultAsync(x => x.Username == username && x.PasswordHash == password);

        }
    }
}
