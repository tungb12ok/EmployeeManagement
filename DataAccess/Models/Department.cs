using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class Department
    {
        public Department()
        {
            Users = new HashSet<User>();
        }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
