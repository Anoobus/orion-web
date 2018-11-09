﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.TimeEntries
{
    public class WeekListViewModel
    {
        public IEnumerable<DetailedWeekIdentifier> Weeks { get; set; }
    }
    public class DetailedWeekIdentifier : WeekIdentifier
    {
        public decimal TotalOverTime { get; set; }
        public decimal TotalRegular { get; set; }
        public TimeApprovalStatus ApprovalStatus { get; set; }
    }
}