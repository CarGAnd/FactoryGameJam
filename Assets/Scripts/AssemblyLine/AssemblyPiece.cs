using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AssemblyPiece : IGridInteractable, ITransportable
{
    private Facing facing = default;
    public Facing Facing { get => facing; }
    protected Vector2Int cellCoords;
    private ITransportable nextPiece;
    private ITransportable previousPiece;
    public ITransportable NextPiece { get => nextPiece; private set => nextPiece = value;}
    public ITransportable PreviousPiece { get => previousPiece; private set => previousPiece = value;}
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

    public AssemblyPiece GetPieceFromCoords(Vector2Int coords)
    {
        if (coords == cellCoords)
            return this;
        return null;
    }

    public void SetPreiousPiece(ITransportable previousPiece)
    {
        PreviousPiece = previousPiece;
    }

    public void SetNextPiece(ITransportable nextPiece)
    {
        NextPiece = nextPiece;
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

    public Vector3 GetWorldPosition() {
        return grid.GetCellCenter(cellCoords);
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
    public void TransportTick()
    {
        if(state == TransportState.Available)
        {
            return;
        }
        if(nextPiece != null && NextPiece.GetState() == TransportState.Available)
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

    public void RemoveFromGrid(Grid grid)
    {
        throw new System.NotImplementedException();
    }
    public List<Vector2Int> GetShapeLayout()
    {
        throw new System.NotImplementedException();
    }

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

    public List<Vector2Int> GetOccupyingCells(Vector2Int startCell, Grid grid) {
        throw new System.NotImplementedException();
    }

    public void OnPlacedOnGrid(Vector2Int startCell, Grid grid) {
        this.cellCoords = startCell;
        this.grid = grid;
        AssemblyLineSystem.Instance.PlaceTransportablePiece(this);
    }

    public void SetPreviousPiece(ITransportable previousPiece)
    {
        PreviousPiece = previousPiece;
    }
}


