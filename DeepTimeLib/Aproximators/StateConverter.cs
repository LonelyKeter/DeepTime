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
        private static readonly int WorkloadContextEntryCount = 25;

        private static readonly int WorkloadContextSize = WorkloadContextEntryCount * 2;
        private static readonly int ScheduleContextSize = TimeOnlySize + TimeBoundsSize + DayOfWeekSize + IsHollidaySize;

        private static readonly int DayOfWeekSize = 1;
        private static readonly int IsHollidaySize = 1;
        private static readonly int TimeBoundsSize = TimeOnlySize * 2;
        private static readonly int TimeOnlySize = 2;

        public int InputSize => WorkloadContextSize * 2 + ScheduleContextSize;

        public float[] ToInput(State input)
        {
            var buffer = new float[InputSize];
            var offset = 0;

            offset += ConvertScheduleContext(input.ScheduleContext, buffer, offset);
            offset += ConvertWorkloadContext(input.TODO, buffer, offset);
            offset += ConvertWorkloadContext(input.Done, buffer, offset);

            return buffer;
        }

        private static int ConvertWorkloadContext(WorkloadContext workloadContext, float[] buffer, int offset)
        {
            for (var i = 0; i < WorkloadContextEntryCount; i++)
            {
                var entry = workloadContext.MinutesEstimate[i / 5, i + 5];

                buffer[offset + 2 * i] = entry.MinutesEstimate;
                buffer[offset + 2 * i + 1] = entry.Count;
            }

            return WorkloadContextSize;
        }

        private static int ConvertScheduleContext(ScheduleContext scheduleContext, float[] buffer, int offset)
        {
            buffer[offset++] = (float)scheduleContext.DayOfWeek;
            buffer[offset++] = scheduleContext.IsHolliday ? 1 : 0;

            offset += ConvertTimeOnly(scheduleContext.Time, buffer, offset);
            offset += ConvertTimeOnly(scheduleContext.Bounds.Start, buffer, offset);
            ConvertTimeOnly(scheduleContext.Bounds.End, buffer, offset);

            return ScheduleContextSize;
        }

        private static int ConvertTimeOnly(TimeOnly time, float[] buffer, int offset)
        {
            buffer[offset++] = time.Minute;
            buffer[offset++] = time.Hour;

            return TimeOnlySize;
        }
    }
}
