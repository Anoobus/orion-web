using Microsoft.VisualStudio.TestTools.UnitTesting;
using orion.web.Common;
using orion.web.TimeEntries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace orion.web.test
{
    [TestClass]
    public class WeekDTOTests
    {
        [TestMethod]
        public void InternallyConsistandWeekCycles()
        {
            var start = new WeekDTO(new DateTime(WeekDTO.WeekEpoch.Year, WeekDTO.WeekEpoch.Month, WeekDTO.WeekEpoch.Day));
            Assert.AreEqual(0, start.WeekId.Value);
            var next = start.Next();
            Assert.AreEqual(1, next.WeekId.Value);

            var next2 = next.Next();
            Assert.AreEqual(2, next2.WeekId.Value);


            var stillNext2 = WeekDTO.CreateWithWeekContaining(next2.WeekEnd.AddDays(-2));
            Assert.AreEqual(next2, stillNext2);
        }
        [TestMethod]
        public void PayPeriodsWork()
        {
            var start = WeekDTO.CreateWithWeekContaining(new DateTime(2019,2,20));
            Assert.IsTrue(start.IsPPE.Value);

            var next = start.Next();
            Assert.IsFalse(next.IsPPE.Value);


            var next2 = next.Next();
            Assert.IsTrue(next2.IsPPE.Value);

            Assert.IsTrue( WeekDTO.CreateWithWeekContaining(new DateTime(2019, 2, 22)).IsPPE.Value);

        }
    }
}
