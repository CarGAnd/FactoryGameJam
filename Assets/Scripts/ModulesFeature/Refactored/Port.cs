using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Port : ITransportable
{
    public Vector2Int position;
    public Facing direction;
    private Vector2Int connectedPosition;
    private ITransportable connectedObject;
    private AssemblyTravelingObject outputObject;
    private AssemblyTravelingObject inputObject;

    public Facing Facing => direction;

    public Port(Vector2Int position, Facing facing) {
        this.position = position;
        this.direction = facing;
        this.connectedPosition = position + facing.GetIntDirection();
    }

    public AssemblyTravelingObject ReceiveFromPort() {
        AssemblyTravelingObject obj = inputObject;
        inputObject = null;
        return obj;
    }

    public void SendToPort(AssemblyTravelingObject obj) {
        outputObject = obj;
    }
    
    public void ReceivedObject(AssemblyTravelingObject aObject) {
        inputObject = aObject;
    }

    public void SendObject() {
        connectedObject.ReceivedObject(outputObject);
        outputObject = null;
    }

    public bool HasInput() {
        return inputObject != null;
    }

    public bool HasOutput() {
        return outputObject != null;
    }

    public TransportState GetState() {
        return HasInput() ? TransportState.Occupied : TransportState.Available;
    }

    public void TransportTick() {
        if(!HasOutput()) {
            return;
        }

        if(connectedObject != null && connectedObject.GetState() == TransportState.Available) {
            SendObject();
        }
    }

    public Vector2Int GetNextCellCoords() {
        return connectedPosition;
    }

    public Vector2Int GetGridCoords() {
        return position;
    }

    public void SetNextTransportable(ITransportable nextTransportable) {
        this.connectedObject = nextTransportable;
    }

    public List<Facing> GetInputDirections() {
        throw new NotImplementedException();
    }
}
