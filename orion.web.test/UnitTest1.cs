using Microsoft.VisualStudio.TestTools.UnitTesting;
using orion.web.Common;
using orion.web.TimeEntries;

namespace orion.web.test
{
    [TestClass]
    public class WeekServiceTests
    {
        [TestMethod]
        public void ReturnForNextWeekOnDecember2018()
        {
            var ws = new WeekService();
            var actual = ws.Next(2018, 53);
            Assert.AreEqual(54, actual.WeekId);
            Assert.AreEqual(2018, actual.Year);

            var NextWeekStart = ws.GetWeekDate(actual.Year, actual.WeekId, WeekIdentifier.WEEK_START);
            var NextWeekEnd = ws.GetWeekDate(actual.Year, actual.WeekId, WeekIdentifier.WEEK_END);

            var NextWeekStart18 = ws.GetWeekDate(2018, 53, WeekIdentifier.WEEK_START);
            var NextWeekEnd18 = ws.GetWeekDate(2018, 53, WeekIdentifier.WEEK_END);

            Assert.AreEqual(2019, NextWeekStart.Year);
            Assert.AreEqual(2019, NextWeekEnd.Year);
        }
        [TestMethod]
        public void ReturnForNextWeekOnDecember2018Good()
        {
            var ws = new WeekService();
            var actual = ws.Next(2018, 52);
            Assert.AreEqual(53, actual.WeekId);
            Assert.AreEqual(2018, actual.Year);
        }

        [TestMethod]
        public void ReturnForPreviousWeekOnFirstOfJanuary2019()
        {
            var ws = new WeekService();
            var actual = ws.Previous(2019, 1);
            Assert.AreEqual(53, actual.WeekId);
            Assert.AreEqual(2018, actual.Year);
        }
    }
}
