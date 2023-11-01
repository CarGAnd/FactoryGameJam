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

    public abstract void StartTravel(List<Vector3> travelPoints);
    public abstract void UpdateTravel();
    public abstract void OnTravelFinished();
}
