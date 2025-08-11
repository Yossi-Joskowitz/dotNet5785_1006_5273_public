namespace Dal;
using DO;
using DalApi;
using System;
using System.Runtime.CompilerServices;

/// <summary>
/// The implementation of the configuration's interface.
/// </summary>
internal class ConfigImplementation : IConfig
{
    
    public DateTime Clock//the system clock getter and setter.
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.Clock;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => Config.Clock = value;
    }


    public TimeSpan RiskRange //the range of time for the risk getter and setter.
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.RiskRange;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => Config.RiskRange = value;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Reset()//reset the system.
    {
        Config.Reset();
    }
}
