using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class AssemblyPiece : IGridInteractable, ITransportable
{
    private Facing facing = default;
    public Facing Facing { get => facing; }
    protected Vector2Int cellCoords;
    private ITransportable nextPiece;
    private int distance;
    private AssemblyTravelingObject travelingObject;
    private Grid grid;
    private AssemblyLineSystem assemblyLineSystem;
    private GameObject gameObject;
    public abstract override string ToString();

    protected AssemblyPiece(AssemblyPieceData data, AssemblyLineSystem assemblyLineSystem)
    {
        this.facing = data.ObjectFacing;
        this.distance = data.movementDistance;
        this.assemblyLineSystem = assemblyLineSystem;
        this.gameObject = data.ModulePrefab;
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
    public void SetNextTransportable(ITransportable nextTransportable)
    {
        this.nextPiece = nextTransportable;
    }
    public void TransportTick()
    {
        if(GetState() == TransportState.Available)
        {
            return;
        }
        if(nextPiece != null && nextPiece.GetState() == TransportState.Available)
        {
            SendObject();   
        }
    }

    public void ReceivedObject(AssemblyTravelingObject travelingObject)
    {
        this.travelingObject = travelingObject;
    }

    public void SendObject()
    {
        nextPiece.ReceivedObject(travelingObject);
        travelingObject.MoveToPiece(cellCoords, nextPiece.GetGridCoords(), grid);
        travelingObject = null;
    }
    public Vector2Int GetNextCellCoords()
    {
        return cellCoords + Movement();
    }

    public TransportState GetState()
    {
        //return the state, if there's no traveling object then it's available
        return travelingObject == null ? TransportState.Available : TransportState.Occupied;
    }

    public Vector2Int GetGridCoords()
    {
        return cellCoords;
    }

///////////////////////////// IGridObject /////////////////////////////
    public void RemoveFromGrid(Grid grid)
    {
        assemblyLineSystem.RemoveTransportablePiece(this);
        grid.RemoveObject(cellCoords);
        MonoBehaviour.Destroy(gameObject);
    }
    public void OnPlacedOnGrid(Vector2Int startCell, Grid grid) {
        this.cellCoords = startCell;
        this.grid = grid;
        assemblyLineSystem.PlaceTransportablePiece(this);
        MonoBehaviour.Instantiate(gameObject, grid.GetCellCenter(cellCoords), grid.Rotation * facing.GetRotationFromFacing());
    }

    public void DestroyObject() {

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


