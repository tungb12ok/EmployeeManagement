using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class Attendance
    {
        public int AttendanceId { get; set; }
        public int? UserId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public int? HoursWorked { get; set; }
        public bool? IsLeave { get; set; }
        public bool? IsOvertime { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual User? User { get; set; }
    }
}
