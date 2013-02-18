using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IncubatorWatch.Info
{
    public enum ActuatorMode
    {
        Manual,
        Auto
    }

    public enum ActuatorCommand
    {
        Open,
        Close,
        Stop    
    }

    public enum ActuatorState
    {
        Open,
        Opening,
        Close,
        Closing,
        Stopped
    }
}
