using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HatchWatch.Info
{
    public enum ActuatorCommand
    {
        Start,
        Stop,
        Pause,
        Unknown
    }

    public enum ActuatorState
    {
        Open,
        Opening,
        Close,
        Closing,
        Stopped,
        Paused,
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
