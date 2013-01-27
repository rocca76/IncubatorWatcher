using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace IncubatorWatch
{
    public class IncubatorDataCollection : RingArray<IncubatorData>
    {
        private const int TotalData = 300;
        public IntPtr CurrPointer = IntPtr.Zero;
        public IntPtr PrevPointer = IntPtr.Zero;

        public IncubatorDataCollection()
            : base(TotalData)
        {
        }

        public IncubatorDataCollection(int capacity)
            : base(capacity)
        {
        }
    }

    public class IncubatorData
    {
        public IncubatorData(DateTime time, double temperature, double relativeHumidity)
        {
            Time = time;
            this.Temperature = temperature;
            this.RelativeHumidity = relativeHumidity;
        }

        public DateTime Time { get; set; }

        public double Temperature { get; set; }

        public double RelativeHumidity { get; set; }
    }
}
