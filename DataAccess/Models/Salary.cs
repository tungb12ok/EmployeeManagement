using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class Salary
    {
        public int SalaryId { get; set; }
        public int UserId { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal? Allowance { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? Deduction { get; set; }
        public decimal? TotalIncome { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Remarks { get; set; }

        public virtual User? User { get; set; }
    }
}
