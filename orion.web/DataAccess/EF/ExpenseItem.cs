
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace orion.web.DataAccess.EF
{
    public class ExpenseItem
    {
        public int ExpenseItemId { get; set; }
        public int EmployeeId { get; set; }
        public int WeekId { get; set; }
        public decimal Amount { get; set; }
        public string AdditionalNotes { get; set; }
        public int JobId { get; set; }
        public string Classification { get; set; }
        public string AttachmentUploadId { get; set; }
        public string AttachmentName { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        [ForeignKey("JobId")]
        public virtual Job RelatedJob { get; set; }
    }
}
