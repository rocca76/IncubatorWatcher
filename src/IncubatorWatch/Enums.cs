using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IncubatorWatch.Info
{
    public enum ActuatorMode
    {
        Manual,
        ManualCentered,
        Auto
    }

    public enum ActuatorState
    {
        Open,
        Opening,
        Close,
        Closing,
        Stopped,
        Unknown
    }

    public enum PumpStateEnum
    {
        Stopped,
        Running
    }

    public enum FanStateEnum
    {
        Stopped,
        Running
    }

    public enum TrapStateEnum
    {
        Closed,
        Opened
    }

    public enum VentilationState
    {
        Stopped,
        Started
    }
}
