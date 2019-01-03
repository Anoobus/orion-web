using Microsoft.VisualStudio.TestTools.UnitTesting;
using orion.web.Common;
using orion.web.TimeEntries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace orion.web.test
{
    [TestClass]
    public class WeekServiceTests
    {
        [TestMethod]
        public void InternallyConsistandWeekCycles()
        {
            var start = new WeekDTO(new DateTime(2018, 12, 15));
            Assert.AreEqual(0, start.WeekId.Value);
            var next = start.Next();
            Assert.AreEqual(1, next.WeekId.Value);

            var next2 = next.Next();
            Assert.AreEqual(2, next2.WeekId.Value);


            var thisWeek = WeekDTO.CreateWithWeekContaining(DateTime.Now);
            Assert.AreEqual(next2, thisWeek);
        }
        //[TestMethod]
        //public void InternallyConsitandWeekCycles()
        //{


        //    var weeks = Enumerable.Range(1, 52).Select(x => x);
        //    var weekListTemp = Enumerable.Range(2018, 2030).Select(x => new { year = x, weeks });
        //    var weekList = weekListTemp.SelectMany(x => x.weeks.Select(z => new WeekDTO() { Year = x.year, WeekId = z }));

        //    var ws = new WeekService();
        //    WeekDTO prev = null;
        //    foreach (var week in weekList)
        //    {
        //        var startDt = ws.GetWeekDate(week.Year, week.WeekId, WeekIdentifier.WEEK_START);
        //        var between = ws.Get(startDt.AddDays(1));
        //        Assert.AreEqual(between, week);
        //        var endDt = ws.GetWeekDate(week.Year, week.WeekId, WeekIdentifier.WEEK_END);

        //        var next = ws.Next(week.Year, week.WeekId);
        //        var nextStartDt = ws.GetWeekDate(next.Year, next.WeekId, WeekIdentifier.WEEK_START);
        //        var nextEndDt = ws.GetWeekDate(next.Year, next.WeekId, WeekIdentifier.WEEK_END);

        //        var nextAlt = ws.Get(endDt.AddDays(1));
        //        Assert.AreEqual(next, nextAlt);
        //        var nextAltStartDt = ws.GetWeekDate(next.Year, next.WeekId, WeekIdentifier.WEEK_START);
        //        Assert.AreEqual(nextStartDt, nextAltStartDt);
        //        var nextAltEndDt = ws.GetWeekDate(next.Year, next.WeekId, WeekIdentifier.WEEK_END);
        //        Assert.AreEqual(nextEndDt, nextAltEndDt);

        //        if (prev != null)
        //        {
        //            var prevStartDt = ws.GetWeekDate(prev.Year, prev.WeekId, WeekIdentifier.WEEK_START);
        //            var prevEndDt = ws.GetWeekDate(prev.Year, prev.WeekId, WeekIdentifier.WEEK_END);

        //            Assert.AreEqual(prevEndDt.AddDays(1), startDt);
        //        }
        //        prev = week;

        //        Assert.AreEqual(endDt.AddDays(1), nextStartDt);

        //        Assert.AreNotEqual(week, next);
        //    }
        //}
        //[TestMethod]
        //public void ReturnForNextWeekOnDecember2018()
        //{
        //    //22-28
        //    //29-04
        //    //05-11
        //    var testDates = new List<Tuple<DateTime, string>>()
        //    {
        //        Tuple.Create(new DateTime(2018, 12, 21),"2018.50"),
        //        Tuple.Create(new DateTime(2018, 12, 22),"2018.51"),
        //        Tuple.Create(new DateTime(2018, 12, 27),"2018.51"),
        //        Tuple.Create(new DateTime(2018, 12, 28),"2018.51"),
        //        Tuple.Create(new DateTime(2018, 12, 29),"2018.52"),
        //        Tuple.Create(new DateTime(2019, 1,4 ),"2018.52"),
        //        Tuple.Create(new DateTime(2019, 1,5 ),"2019.1"),
        //        Tuple.Create(new DateTime(2019, 1,11 ),"2019.1"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.2"),

        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.3"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.4"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.5"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.6"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.7"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.8"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.9"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.10"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.11"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.12"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.13"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.14"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.15"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.16"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.17"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.18"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.19"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.20"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.21"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.22"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.23"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.24"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.25"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.26"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.27"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.28"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.29"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.30"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.31"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.32"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.33"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.34"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.35"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.36"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.37"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.38"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.39"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.40"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.41"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.42"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.43"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.44"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.45"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.46"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.47"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.48"),
        //        Tuple.Create(new DateTime(2019, 1,12 ),"2019.49"),

        //        Tuple.Create(new DateTime(2019, 12, 20),"2019.50"),
        //        Tuple.Create(new DateTime(2019, 12, 21),"2019.51"),
        //        Tuple.Create(new DateTime(2019, 12, 26),"2019.51"),
        //        Tuple.Create(new DateTime(2019, 12, 27),"2019.51"),
        //        Tuple.Create(new DateTime(2019, 12, 28),"2019.52"),
        //        Tuple.Create(new DateTime(2020, 1,3 ),"2019.52"),
        //        Tuple.Create(new DateTime(2020, 1,4 ), "2020.1"),
        //        Tuple.Create(new DateTime(2020, 1,10 ),"2020.1"),
        //        Tuple.Create(new DateTime(2020, 1,11 ),"2020.2")
        //    };
        //    var results = new List<string>();
        //    foreach (var item in testDates)
        //    {
        //        var res = GetWeekInfoByDate(item.Item1);
        //        results.Add($"For {item.Item1.ToShortDateString()}, got {res.Year}.{res.WeekId} expected {item.Item2}");
        //        Assert.AreEqual(item.Item2, $"{res.Year}.{res.WeekId}");
        //    }

        //    Console.WriteLine(string.Join(Environment.NewLine, results));
        //}

        //[TestMethod]
        //public void ReturnForNextWeekOnDecember2019()
        //{
        //    //21-27
        //    //28-03
        //    //04-10
        //    var testDates = new List<Tuple<DateTime, string>>()
        //    {
        //        Tuple.Create(new DateTime(2019, 12, 20),"2019.50"),
        //        Tuple.Create(new DateTime(2019, 12, 21),"2019.51"),
        //        Tuple.Create(new DateTime(2019, 12, 26),"2019.51"),
        //        Tuple.Create(new DateTime(2019, 12, 27),"2019.51"),
        //        Tuple.Create(new DateTime(2019, 12, 28),"2019.52"),
        //        Tuple.Create(new DateTime(2020, 1,3 ),"2019.52"),
        //        Tuple.Create(new DateTime(2020, 1,4 ), "2020.1"),
        //        Tuple.Create(new DateTime(2020, 1,10 ),"2020.1"),
        //        Tuple.Create(new DateTime(2020, 1,11 ),"2020.2")
        //    };
        //    var results = new List<string>();
        //    foreach (var item in testDates)
        //    {
        //        var res = GetWeekInfoByDate(item.Item1);
        //        results.Add($"For {item.Item1.ToShortDateString()}, got {res.Year}.{res.WeekId} expected {item.Item2}");
        //        Assert.AreEqual(item.Item2, $"{res.Year}.{res.WeekId}");
        //    }

        //    Console.WriteLine(string.Join(Environment.NewLine, results));
        //}

        //private WeekDTO GetWeekInfoByDate(DateTime date)
        //{
        //    var ws = new WeekService();
        //    return ws.Get(date);
        //}
        //[TestMethod]
        //public void CanUseNextAndPreviousAcrossYearConsitantly()
        //{
        //    var ws = new WeekService();
        //    var current = new WeekDTO()
        //    {
        //        WeekId = 50,
        //        Year = 2018
        //    };

        //    var first = ws.Next(current.Year, current.WeekId);
        //    Assert.AreNotEqual(current.WeekId, first.WeekId, $"{current} != {first}");
        //    Assert.AreEqual(current.Year, first.Year, $"{current} != {first}");

        //    var second = ws.Next(first.Year, first.WeekId);
        //    Assert.AreNotEqual(first.WeekId, second.WeekId, $"{first} != {second}");
        //    Assert.AreEqual(first.Year, second.Year, $"{first} != {second}");

        //    var third = ws.Next(second.Year, second.WeekId);
        //    Assert.AreNotEqual(second.WeekId, third.WeekId, $"{second} != {third}");
        //    Assert.AreNotEqual(second.Year, third.Year, $"{second} != {third}");

        //    var fourth = ws.Next(third.Year, third.WeekId);
        //    Assert.AreNotEqual(third.WeekId, fourth.WeekId, $"{third} != {fourth}");
        //    Assert.AreEqual(third.Year, fourth.Year, $"{third} != {fourth}");


        //    var prevThird = ws.Previous(fourth.Year, fourth.WeekId);
        //    Assert.AreEqual(third, prevThird);

        //    var prevSecond = ws.Previous(prevThird.Year, prevThird.WeekId);
        //    Assert.AreEqual(second, prevSecond);

        //    var prevFirst = ws.Previous(prevSecond.Year, prevSecond.WeekId);
        //    Assert.AreEqual(first, prevFirst);
        //}


    }
}
