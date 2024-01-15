using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransportable
{
    void TransportTick();
    void ReceivedObject(AssemblyTravelingObject travelingObject);
    void SendObject();
    Vector2Int GetGridCoords();
}
public enum TransportState
{
    Available = 0,
    Occupied = 10,
}