using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransportable
{
    void TransportTick();
    void ReceivedObject(AssemblyTravelingObject travelingObject);
    void SendObject();
    void SetPreviousPiece(ITransportable previousPiece);
    void SetNextPiece(ITransportable nextPiece);
    ITransportable NextPiece { get; }
    ITransportable PreviousPiece { get; }
    Facing Facing { get; }
    //GetNextCellCoords should be this ITransportable's coords + movement (as seen in AssemblyPiece.cs)
    Vector2Int GetNextCellCoords();
    Vector2Int GetGridCoords();
    TransportState GetState();

}
public enum TransportState
{
    Available = 0,
    Occupied = 10,
}
public enum Facing
{
    North = 0,
    East = 10,
    South = 20,
    West = 30
}