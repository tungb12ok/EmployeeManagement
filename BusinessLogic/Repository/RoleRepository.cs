using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public interface IRoleRepository
    {
        List<Role> GetAll();
    }
    public class RoleRepository : IRoleRepository
    {
        private readonly EmployeeManagementContext _context;

        public RoleRepository(EmployeeManagementContext context)
        {
            _context = context;
        }

        public List<Role> GetAll()
        {
           return _context.Roles.ToList();
        }
    }
}
