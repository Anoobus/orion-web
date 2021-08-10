using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public class TimeSpentViewModel
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime Date { get; set; }
        [Range(0, double.MaxValue)]
        [RegularExpression(@"([0-9]*\.[0-9])|(([0-9])\.?)")]
        public decimal Hours { get; set; }
        [Range(0, double.MaxValue)]
        [RegularExpression(@"([0-9]*\.[0-9])|(([0-9])\.?)")]
        public decimal OvertimeHours { get; set; }
    }
}
