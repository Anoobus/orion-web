﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Orion.Web.Common;

namespace Orion.Web.Expense
{
    public class EditExpenseViewModel : AddExpenseViewModel
    {
        public int ExpenseItemId { get; set; }
        public Guid? AttachmentId { get; set; }
    }

    public class AddExpenseViewModel
    {
        public ExpenseModel ExpenseToSave { get; set; }
        public IEnumerable<Jobs.CoreJobDto> AvailableJobs { get; set; }
        public WeekDTO Week { get; set; }
        public int SelectedJobId { get; set; }
        public string CancelUrl { get; set; }
        public IFormFile UploadFile { get; set; }
        public int WeekId { get; set; }
    }

    public class ExpenseModel
    {
        public DateTimeOffset SaveDate { get; set; }
        public decimal Amount { get; set; }
        [Display(Name = "Expense (Related Attachment)")]
        public string AttachmentName { get; set; }
        public string Classification { get; set; }
        public string AddtionalNotes { get; set; }
        public Jobs.CoreJobDto RelatedJob { get; set; }
    }
}
