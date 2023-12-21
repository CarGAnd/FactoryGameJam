using System.Collections;
using System.Collections.Generic;
using Codice.CM.Client.Differences;
using UnityEngine;

public abstract class AssemblyPiece : IGridInteractable
{
    public Facing facing = default;
    private Cell cell;
    public AssemblyPiece NextPiece {get; set;}
    public AssemblyPiece PreviousPiece {get; set;}

    protected AssemblyPiece(AssemblyPieceData data, Cell cell)
    {
        this.cell = cell;
    }
    public void OnTick()
    {

    }

    public Cell GetCell()
    {
        return cell;
    }
    public abstract Vector2Int Movement();

    public void PlaceOnGrid(Cell startCell, Grid grid)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveFromGrid(Grid grid)
    {
        throw new System.NotImplementedException();
    }
    public T GetObject<T>()
    {
        throw new System.NotImplementedException();
    }

    public List<Cell> GetOccupyingCells(Cell startCell, Grid grid)
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

    public void OnPlacedOnGrid()
    {
        throw new System.NotImplementedException();
    }

    public void OnRemovedFromGrid()
    {
        throw new System.NotImplementedException();
    }

    public void OnSelected()
    {
        throw new System.NotImplementedException();
    }

    
}

public enum Facing
{
    Default = 0,
    North = 10,
    East = 20,
    South = 30,
    West = 40
}

public enum PieceState
{
    Available = 0,
    Occupied = 10,
}
