using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace IncubatorWatch
{
    public class IncubatorDataCollection : RingArray<IncubatorData>
    {
        private const int TotalData = 60;
        public IntPtr CurrPointer = IntPtr.Zero;
        public IntPtr PrevPointer = IntPtr.Zero;

        /*public IncubatorDataCollection()
            : base(TotalData)
        {
        }*/

        public IncubatorDataCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public class IncubatorData
    {
        public IncubatorData(DateTime time, Int64 temperature, Int64 relativeHumidity)
        {
            Time = time;
            Temperature = temperature;
            RelativeHumidity = relativeHumidity;
        }

        public DateTime Time { get; set; }

        public Int64 Temperature { get; set; }

        public Int64 RelativeHumidity { get; set; }
    }
}
