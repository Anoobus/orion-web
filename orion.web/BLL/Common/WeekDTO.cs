using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace orion.web.Common
{


    public class WeekDTO
    {
        private static readonly ConcurrentDictionary<WeekDTO, bool> PPEStatusByWeek = new ConcurrentDictionary<WeekDTO, bool>();
        private static readonly ConcurrentDictionary<int, WeekDTO> weeksByWeekId = new ConcurrentDictionary<int, WeekDTO>();
        private static readonly ConcurrentDictionary<WeekDTO,int> weekIdsByWeek = new ConcurrentDictionary<WeekDTO, int>();
        public static readonly DateTime WeekEpoch = new DateTime(2000, 1, 1);

        public const DayOfWeek WEEK_START = DayOfWeek.Saturday;
        public const DayOfWeek WEEK_END = DayOfWeek.Friday;

        public readonly DateTime WeekEnd;
        public readonly DateTime WeekStart;
        public int Year => WeekStart.Year;
        public Lazy<int> WeekId => new Lazy<int>(() => GetWeekId());
        public Lazy<bool> IsPPE => new Lazy<bool>(() => GetPPEStatus());

        private bool GetPPEStatus()
        {
            return WeekId.Value % 2 == 1;
        }

        public WeekDTO(DateTime weekStart)
        {
            if (weekStart.DayOfWeek != WEEK_START)
            {
                throw new InvalidOperationException($"Week start must be on a {WEEK_START}");
            }
            WeekStart = weekStart.Date;
            WeekEnd = weekStart.Date;
            while (WeekEnd.DayOfWeek != WEEK_END)
            {
                WeekEnd = WeekEnd.AddDays(1);
            }
        }

        private int GetWeekId()
        {
            return weekIdsByWeek.GetOrAdd(this, (week) =>
             {
                 var candidate = new WeekDTO(WeekEpoch);
                 var weekId = 0;
                 while (candidate != this)
                 {
                     candidate = candidate.Next();
                     ++weekId;
                 }
                 return weekId;
             });
        }

        public WeekDTO Next()
        {
            return new WeekDTO(WeekEnd.AddDays(1));
        }

        public WeekDTO Previous()
        {
            return new WeekDTO(WeekStart.AddDays(-7));
        }

        public override string ToString()
        {
            return WeekStart.ToShortDateString();
        }

        public string UrlId()
        {
            return $"{WeekStart.Year:0000}.{WeekStart.Month:00}.{WeekStart.Day:00}";
        }

        public static WeekDTO CreateWithWeekContaining(DateTime date)
        {
            while (date.DayOfWeek != WEEK_START)
            {
                date = date.AddDays(-1);
            }
            return new WeekDTO(date);
        }

        public static WeekDTO CreateForWeekId(int weekId)
        {
            return weeksByWeekId.GetOrAdd(weekId, (id) =>
             {
                 var candidate = new WeekDTO(WeekEpoch);
                 var candidateWeekId = 0;
                 while (candidateWeekId != weekId)
                 {
                     candidate = candidate.Next();
                     ++candidateWeekId;
                 }
                 return candidate;
             });
        }

        public static bool operator ==(WeekDTO week, WeekDTO other)
        {
            if (ReferenceEquals(week, null))
            {
                return ReferenceEquals(other, null);
            }

            return week.Equals(other);
        }
        public static bool operator !=(WeekDTO week, WeekDTO other)
        {
            if (ReferenceEquals(week, null))
            {
                return !ReferenceEquals(other, null);
            }
            return !(week.Equals(other));
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as WeekDTO);
        }

        public bool Equals(WeekDTO obj)
        {
            return obj != null
                && obj.WeekStart.Date == WeekStart.Date;
        }

        public override int GetHashCode()
        {
            return WeekStart.Date.GetHashCode();
        }
    }
}
