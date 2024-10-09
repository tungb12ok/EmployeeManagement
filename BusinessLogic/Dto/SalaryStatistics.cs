using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Dto
{
    public class SalaryStatistics
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal TotalAllowance { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal TotalIncome { get; set; }
        public DateTime PaymentDate { get; set; }
    }

}
