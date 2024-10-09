using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class User
    {
        public User()
        {
            Attendances = new HashSet<Attendance>();
            Salaries = new HashSet<Salary>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int? RoleId { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public int? DepartmentId { get; set; }
        public string? Position { get; set; }
        public decimal Salary { get; set; }
        public DateTime StartDate { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Department? Department { get; set; }
        public virtual Role? Role { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
        public virtual ICollection<Salary> Salaries { get; set; }
    }
}
