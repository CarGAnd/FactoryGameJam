using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleInputOutput : MonoBehaviour, IGridObject, ITransportable
{
    private ModuleSO moduleSettings;
    private Vector2Int originCell;
    private Grid grid;
    private int numRotations;

    private List<PortSettings> inputPorts;
    private List<PortSettings> outputPorts;

    public ITransportable NextPiece => throw new System.NotImplementedException();

    public ITransportable PreviousPiece => throw new System.NotImplementedException();

    public Facing Facing => throw new System.NotImplementedException();

    public void Initialize(ModuleSO moduleSettings, int numRotations) {
        this.moduleSettings = moduleSettings;
        this.inputPorts = moduleSettings.GetInputs(numRotations);
        this.outputPorts = moduleSettings.GetOutputs(numRotations);
        this.numRotations = numRotations;
    }

    public PortSettings GetInputAtPosition(Vector2Int gridPosition) {
        foreach(PortSettings ps in inputPorts) {
            if(ps.position + originCell == gridPosition) {
                return ps;
            }
        }
        return null;
    }   
    
    public PortSettings GetOutputAtPosition(Vector2Int gridPosition) {
        foreach(PortSettings ps in outputPorts) {
            if(ps.position + originCell == gridPosition) {
                return ps;
            }
        }
        return null;
    }

    public List<Vector2Int> GetOccupyingCells(Vector2Int startCell, Grid grid) {
        throw new System.NotImplementedException();
    }

    public void OnPlacedOnGrid(Vector2Int startCell, Grid grid) {
        this.originCell = startCell;
        this.grid = grid;
    }

    public void Destroy() {
        RemoveFromGrid(grid);
    }

    public void RemoveFromGrid(Grid grid) {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> GetShapeLayout() {
        throw new System.NotImplementedException();
    }

    public void TransportTick() {
        throw new System.NotImplementedException();
    }

    public void ReceivedObject(AssemblyTravelingObject travelingObject) {
        throw new System.NotImplementedException();
    }

    public void SendObject() {
        throw new System.NotImplementedException();
    }

    public Vector2Int GetGridCoords() {
        throw new System.NotImplementedException();
    }

    private void OnDrawGizmos() {
        if(grid == null) {
            return;
        }
        List<Vector2Int> positions = grid.GetPositionsInSubgrid(originCell, new Vector2Int(moduleSettings.Width, moduleSettings.Height));
        foreach(Vector2Int position in positions) {
            PortSettings input = GetInputAtPosition(position);
            PortSettings output = GetOutputAtPosition(position);
            if (input != null) {
                Gizmos.color = Color.green;
                Vector2Int facingDirection = input.direction.GetIntDirection();
                Gizmos.DrawWireCube(grid.GetCellCenter(position - facingDirection), new Vector3(grid.CellSize.x, 3, grid.CellSize.y));

            }
            else if (output != null) {
                Gizmos.color = Color.red;
                Vector2Int facingDirection = output.direction.GetIntDirection();
                Gizmos.DrawWireCube(grid.GetCellCenter(position + facingDirection), new Vector3(grid.CellSize.x, 3, grid.CellSize.y));
            }
        }
    }

    public void SetPreviousPiece(ITransportable previousPiece) {
        throw new System.NotImplementedException();
    }

    public void SetNextPiece(ITransportable nextPiece) {
        throw new System.NotImplementedException();
    }

    public Vector2Int GetNextCellCoords() {
        throw new System.NotImplementedException();
    }

    public TransportState GetState() {
        throw new System.NotImplementedException();
    }
}
