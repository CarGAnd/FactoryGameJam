using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAssemblyTravelMethod
{
    public Action TravelStarted { get; set; }
    public Action TravelFinished { get; set; }
    public Action TravelTick { get; set; }
    public IAssembly AssemblyStart { get; }
    public IAssembly AssemblyDestination { get; }
    public void StartTravel(IAssembly startAssembly, Transform _transform);
    public void UpdateTravel();
    public void FinishTravel<T>(ITravelAssemblyLine<T> assemblyLine);
}
