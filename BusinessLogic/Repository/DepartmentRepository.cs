using DataAccess.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Repository
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<IEnumerable<Department>> GetDepartmentsAsync(); // Corrected method name and signature
        Task<IEnumerable<User>> GetEmployeesByDepartmentAsync(int departmentId);
    }

    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly EmployeeManagementContext _context;

        public DepartmentRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        // Add a new department
        public async Task AddAsync(Department entity)
        {
            await _context.Departments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // Delete a department by ID
        public async Task DeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }

        // Get all departments
        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        // Get a department by its ID
        public async Task<Department> GetByIdAsync(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        // Get all departments (Corrected method)
        public async Task<IEnumerable<Department>> GetDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        // Get all employees by department ID
        public async Task<IEnumerable<User>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            return await _context.Users
                                 .Where(u => u.DepartmentId == departmentId)
                                 .ToListAsync();
        }

        // Update an existing department
        public async Task UpdateAsync(Department entity)
        {
            var department = await _context.Departments.FindAsync(entity.DepartmentId);
            if (department != null)
            {
                department.DepartmentName = entity.DepartmentName;
                department.CreatedAt = entity.CreatedAt;

                _context.Departments.Update(department);
                await _context.SaveChangesAsync();
            }
        }
    }
}
