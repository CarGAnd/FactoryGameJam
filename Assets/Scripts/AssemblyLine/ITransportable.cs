using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransportable
{
    void TransportTick();
    void ReceivedObject(AssemblyTravelingObject travelingObject);
    void SendObject();
    Facing Facing { get; }
    //GetNextCellCoords should be this ITransportable's coords + movement (as seen in AssemblyPiece.cs)
    Vector2Int GetNextCellCoords();
    Vector2Int GetGridCoords();
    TransportState GetState();
    void SetAssemblyLine(AssemblyLine parentAssemblyLine, LinkedListNode<ITransportable> node);

}
public enum TransportState
{
    Available = 0,
    Occupied = 10,
}
