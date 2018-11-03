using orion.web.TimeEntries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Common
{
    public interface IWeekIdentifierListQuery : IRegisterByConvention
    {
        IEnumerable<WeekIdentifier> GetWeeks(int entriesToShow);
    }
    public class WeekIdentifierListQuery : IWeekIdentifierListQuery
    {
        private readonly IWeekService weekService;

        public WeekIdentifierListQuery(IWeekService weekService)
        {
            this.weekService = weekService;
        }
        public IEnumerable<WeekIdentifier> GetWeeks(int entriesToShow)
        {
            var dt = DateTime.Now;
            var thisWeek = weekService.Get(DateTime.Now);
            while(entriesToShow-- > 0)
            {
                var temp = weekService.Previous(thisWeek.Year, thisWeek.WeekId);
                yield return new WeekIdentifier()
                {
                    WeekEnd = weekService.GetWeekDate(temp.Year, temp.WeekId, WeekIdentifier.WEEK_END),
                    WeekStart = weekService.GetWeekDate(temp.Year, temp.WeekId, WeekIdentifier.WEEK_START),
                    WeekId = temp.WeekId,
                    Year = temp.Year
                };
                thisWeek = temp;
            }
        }
    }
}
