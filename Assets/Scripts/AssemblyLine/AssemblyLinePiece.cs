using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AssemblyLinePiece : MonoBehaviour, IGridInteractable
{
    public Cell CellReference { get; protected set; }
    public Direction Direction { get; protected set; }

    public int MovementDistance { get; protected set; }

    public virtual void Initialize(Cell cellRef, int movementDistance)
    {
        CellReference = cellRef;
        Direction = Direction.undefined;
        MovementDistance = movementDistance;
    }
    public abstract void MoveObject();
    public abstract void ConnectToNeighbor(AssemblyLinePiece neighbor);

    public void OnSelected()
    {
        throw new System.NotImplementedException();
    }

    public bool IsSelected()
    {
        throw new System.NotImplementedException();
    }

    public bool IsPlaced()
    {
        throw new System.NotImplementedException();
    }

    public List<Cell> GetOccupyingCells(Cell startCell, Grid grid)
    {
        return new List<Cell>() { startCell };
    }

    public T GetObject<T>()
    {
        return (T)(object)this;
    }

    public void PlaceOnGrid(Cell startCell, Grid grid)
    {
        grid.PlaceObject(this, startCell, null);
    }

    public void RemoveFromGrid(Grid grid)
    {
        grid.RemoveObject(CellReference);
    }

    public void OnRemovedFromGrid()
    {
        throw new System.NotImplementedException();
    }

    public void OnPlacedOnGrid()
    {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> GetShapeLayout()
    {
        return new List<Vector2Int>() { Vector2Int.zero };
    }
}

public enum Direction
{
    undefined = 0,
    forward = 10,
    backward = 20,
    uniDirection = 30,
}
