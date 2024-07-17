using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Orion.Web.TimeEntries
{
    public class TimeSpentViewModel
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime Date { get; set; }

        [Required(AllowEmptyStrings = true)]
        [RegularExpression(@"([0-9]*\.[0-9])|(([0-9])\.?)", ErrorMessage = "Hours must be a value with up to one decimal point precission")]
        [DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true)]
        public decimal? Hours { get; set; }

        [Required(AllowEmptyStrings = true)]
        [RegularExpression(@"([0-9]*\.[0-9])|(([0-9])\.?)", ErrorMessage = "Overtime Hours must be a value with up to one decimal point precission")]
        [DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true)]
        public decimal? OvertimeHours { get; set; }
    }
}
