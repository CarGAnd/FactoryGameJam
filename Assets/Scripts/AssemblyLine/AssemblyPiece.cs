using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class AssemblyPiece : IGridInteractable, ITransportable
{
    private Facing facing = default;
    public Facing Facing { get => facing; }
    protected Vector2Int cellCoords;
    private AssemblyLine parentAssemblyLine;
    private LinkedListNode<ITransportable> node;
    private ITransportable nextPiece;
    private TransportState state = TransportState.Available;
    private int distance;
    private AssemblyTravelingObject travelingObject;
    private Grid grid;
    public abstract override string ToString();

    protected AssemblyPiece(AssemblyPieceData data)
    {
        this.facing = data.Facing;
        this.distance = data.movementDistance;
    }
    public Vector2Int Movement()
    {
        Vector2Int movement = Vector2Int.zero;
        switch(facing)
        {
            case Facing.North:
                movement = Vector2Int.up;
                break;
            case Facing.East:
                movement = Vector2Int.right;
                break;
            case Facing.South:
                movement = Vector2Int.down;
                break;
            case Facing.West:
                movement = Vector2Int.left;
                break;
        }
        return movement * distance;
    }
///////////////////////////// ITransportable /////////////////////////////
    public void SetAssemblyLine(AssemblyLine parentAssemblyLine, LinkedListNode<ITransportable> node)
    {
        this.parentAssemblyLine = parentAssemblyLine;
        this.node = node;
    }
    public void TransportTick()
    {
        if(state == TransportState.Available)
        {
            return;
        }
        nextPiece = parentAssemblyLine.GetNextPiece(node);
        if(nextPiece != null && nextPiece.GetState() == TransportState.Available)
        {
            SendObject();   
        }
    }

    public void ReceivedObject(AssemblyTravelingObject travelingObject)
    {
        state = TransportState.Occupied;
        this.travelingObject = travelingObject;
    }

    public void SendObject()
    {
        state = TransportState.Available;
        nextPiece.ReceivedObject(travelingObject);
        travelingObject.MoveToPiece(cellCoords, nextPiece.GetGridCoords(), grid);
    }
    public Vector2Int GetNextCellCoords()
    {
        return cellCoords + Movement();
    }

    public TransportState GetState()
    {
        return state;
    }

    public Vector2Int GetGridCoords()
    {
        return cellCoords;
    }

///////////////////////////// IGridObject /////////////////////////////
    public void RemoveFromGrid(Grid grid)
    {
        throw new System.NotImplementedException();
    }
    public void OnPlacedOnGrid(Vector2Int startCell, Grid grid) {
        this.cellCoords = startCell;
        this.grid = grid;
        AssemblyLineSystem.Instance.PlaceTransportablePiece(this);
    }
///////////////////////////// IGridInteractable /////////////////////////////
    public bool IsPlaced()
    {
        throw new System.NotImplementedException();
    }

    public bool IsSelected()
    {
        throw new System.NotImplementedException();
    }

    public void OnSelected()
    {
        throw new System.NotImplementedException();
    }
}


