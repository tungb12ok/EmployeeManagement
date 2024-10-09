using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Dto
{
    public class DepartmentStatistics
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int EmployeeCount { get; set; }
        public int MaleEmployeeCount { get; set; }
        public int FemaleEmployeeCount { get; set; }
    }
}
