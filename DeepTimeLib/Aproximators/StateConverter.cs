using DeepTime.Lib.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Lib.Aproximators
{
    public struct StateConverter : IStateConverter<Data.State>
    {
        private static readonly int WorkloadContextSize = WorkloadContext.ArrayPresentationLength * 2;
        private static readonly int ScheduleContextSize = TimeOnlySize + TimeBoundsSize + DayOfWeekSize + IsHollidaySize;

        private static readonly int DayOfWeekSize = 1;
        private static readonly int IsHollidaySize = 1;
        private static readonly int TimeBoundsSize = TimeOnlySize * 2;
        private static readonly int TimeOnlySize = 2;

        public int InputSize => WorkloadContextSize * 2 + ScheduleContextSize;

        public double[] ToInput(State input)
        {
            var buffer = new double[InputSize];
            var offset = 0;

            offset += ConvertScheduleContext(input.ScheduleContext, buffer, offset);
            offset += ConvertWorkloadContext(input.TODO, buffer, offset);
            ConvertWorkloadContext(input.Done, buffer, offset);

            return buffer;
        }

        private static int ConvertWorkloadContext(WorkloadContext workloadContext, double[] buffer, int offset)
        {
            foreach (var (i, entry) in workloadContext.Select((val, i) => (i, val))) {
                buffer[offset + i * 2] = entry.MinutesEstimate;
                buffer[offset + i * 2 + 1] = entry.Count;
            }

            return WorkloadContextSize;
        }

        private static int ConvertScheduleContext(ScheduleContext scheduleContext, double[] buffer, int offset)
        {
            buffer[offset++] = (double)scheduleContext.DayOfWeek;
            buffer[offset++] = scheduleContext.IsHolliday ? 1 : 0;

            offset += ConvertTimeOnly(scheduleContext.Time, buffer, offset);
            offset += ConvertTimeOnly(scheduleContext.Bounds.Start, buffer, offset);
            ConvertTimeOnly(scheduleContext.Bounds.End, buffer, offset);

            return ScheduleContextSize;
        }

        private static int ConvertTimeOnly(TimeOnly time, double[] buffer, int offset)
        {
            buffer[offset++] = time.Minute;
            buffer[offset++] = time.Hour;

            return TimeOnlySize;
        }
    }
}
