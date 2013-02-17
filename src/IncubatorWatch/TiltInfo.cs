using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IncubatorWatch.Info
{
    public enum TiltMode
    {
        Manual,
        Auto
    }

    public enum TiltState
    {
        Open,
        Close,
        Opening,
        Closing,
        Stopped
    }
}
