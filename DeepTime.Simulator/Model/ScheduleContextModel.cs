using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Simulator.Model
{
    internal class ScheduleContextModel
    {
        public DayOfWeek DayOfWeek { get; init; }
        public TimeOnly Time { get; init; }
        public bool IsHoliday { get; init; }
    }
}
