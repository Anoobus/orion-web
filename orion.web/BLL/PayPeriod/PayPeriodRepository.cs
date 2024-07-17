using System;
using System.Collections.Generic;
using System.Linq;
using Orion.Web.Common;

namespace Orion.Web.PayPeriod
{
    public class PayPeriodRepository
    {
        public static PayPeriodListDTO GetPPRange(DateTime middleDate = default(DateTime), int rangeSize = 15)
        {
            var startPP = GetPPFor(middleDate == default(DateTime) ? DateTime.Now : middleDate);
            var list = new PayPeriodListDTO()
            {
                SelectedPayPeriod = startPP,
                PayPeriodList = new List<PayPeriodDTO>() { startPP }
            };
            var allEntries = new List<PayPeriodDTO>();

            rangeSize = rangeSize - 1;
            var surroundingSize = rangeSize / 2;

            var temp = startPP;
            for (int i = 0; i < surroundingSize; i++)
            {
                temp = GetNextPP(temp);
                list.PayPeriodList.Add(temp);
            }

            temp = startPP;
            for (int i = 0; i < surroundingSize; i++)
            {
                temp = GetPreviousPP(temp);
                list.PayPeriodList.Add(temp);
            }

            list.PayPeriodList = list.PayPeriodList.OrderBy(x => x.PayPeriodEnd).ToList();
            return list;
        }

        public static PayPeriodDTO GetPPFor(DateTime date)
        {
            var currentPP = new PayPeriodDTO();
            var currentWeek = WeekDTO.CreateWithWeekContaining(date);
            if (currentWeek.IsPPE.Value)
            {
                return MapForPPEndWeek(currentWeek);
            }
            else
            {
                return MapForPPStartWeek(currentWeek);
            }
        }

        private static PayPeriodDTO MapForPPStartWeek(WeekDTO currentWeek)
        {
            var next = currentWeek.Next();
            return new PayPeriodDTO()
            {
                PayPeriodStart = currentWeek.WeekStart,
                PayPeriodEnd = next.WeekEnd,
                StartWeekId = currentWeek.WeekId.Value,
                EndWeekId = next.WeekId.Value,
            };
        }

        private static PayPeriodDTO MapForPPEndWeek(WeekDTO currentWeek)
        {
            var previous = currentWeek.Previous();
            return new PayPeriodDTO()
            {
                PayPeriodStart = previous.WeekStart,
                PayPeriodEnd = currentWeek.WeekEnd,
                StartWeekId = previous.WeekId.Value,
                EndWeekId = currentWeek.WeekId.Value
            };
        }

        private static PayPeriodDTO GetNextPP(PayPeriodDTO currentPP)
        {
            var week = WeekDTO.CreateWithWeekContaining(currentPP.PayPeriodEnd.AddDays(1));
            return MapForPPStartWeek(week);
        }

        private static PayPeriodDTO GetPreviousPP(PayPeriodDTO currentPP)
        {
            var week = WeekDTO.CreateWithWeekContaining(currentPP.PayPeriodStart.AddDays(-1));
            return MapForPPEndWeek(week);
        }
    }
}
