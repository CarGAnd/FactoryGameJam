using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITravelAssemblyLine<T>
{
    public Action<GameObject> TravelStarted {
        get;
        set;
    }

    public Action<GameObject> TravelStopped {
        get;
        set;
    }

    public bool IsTraveling {
        get;
        set;
    }

    public T Value {
        get;
    }

    public IAssembly DestinationAssembly {
        get;
    }

    public abstract void StartTravel(IAssembly destination);
    public abstract void UpdateTravel();
    public abstract void OnTravelFinished();
}
