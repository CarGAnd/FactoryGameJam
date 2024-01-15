using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleInputOutput : MonoBehaviour, IGridObject
{
    private ModuleSO moduleSettings;
    private Vector2Int originCell;
    private Grid grid;
    private int numRotations;

    private List<Port> inputPorts;
    private List<Port> outputPorts;

    private List<AssemblyTravelingObject> inputStorage;
    private List<AssemblyTravelingObject> outputStorage;

    public void Initialize(ModuleSO moduleSettings, int numRotations) {
        this.moduleSettings = moduleSettings;
        this.numRotations = numRotations;

        inputStorage = new List<AssemblyTravelingObject>();
        outputStorage = new List<AssemblyTravelingObject>();
        inputPorts = new List<Port>();
        outputPorts = new List<Port>();

        List<PortSettings> inputSettings = moduleSettings.GetInputs(numRotations);
        List<PortSettings> outputSettings = moduleSettings.GetOutputs(numRotations);

        foreach(PortSettings ps in inputSettings) {
            Port port = new Port(ps);
            inputPorts.Add(port);
        }
        foreach(PortSettings ps in outputSettings) {
            Port port = new Port(ps);
            outputPorts.Add(port);
        }
    }

    public Port GetInputAtPosition(Vector2Int gridPosition) {
        foreach(Port ps in inputPorts) {
            if(ps.position + originCell == gridPosition) {
                return ps;
            }
        }
        return null;
    }   
    
    public Port GetOutputAtPosition(Vector2Int gridPosition) {
        foreach(Port ps in outputPorts) {
            if(ps.position + originCell == gridPosition) {
                return ps;
            }
        }
        return null;
    }

    public AssemblyTravelingObject ReceiveFromInput() {
        if(inputStorage.Count > 0) {
            AssemblyTravelingObject obj = inputStorage[0];
            inputStorage.RemoveAt(0);
            return obj;
        }
        else {
            return null;
        }
    }

    public void SendToOutput(AssemblyTravelingObject aObject) {
        outputStorage.Add(aObject);
    }

    private void Tick() {
        foreach(Port p in inputPorts) {
            if(p.GetState() == TransportState.Occupied) {
                inputStorage.Add(p.ReceiveFromPort());
            }
        }

        foreach(Port p in outputPorts) {
            if(p.GetState() == TransportState.Available && outputStorage.Count > 0) {
                p.SendToPort(outputStorage[0]);
                outputStorage.RemoveAt(0);
                p.Tick();
            }
        }
    }

    private void PlacePorts() {

    }

    public List<Vector2Int> GetOccupyingCells(Vector2Int startCell, Grid grid) {
        throw new System.NotImplementedException();
    }

    public void OnPlacedOnGrid(Vector2Int startCell, Grid grid) {
        this.originCell = startCell;
        this.grid = grid;
        PlacePorts();
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

    #region Debug
    private void OnDrawGizmos() {
        if(grid == null) {
            return;
        }
        List<Vector2Int> positions = grid.GetPositionsInSubgrid(originCell, new Vector2Int(moduleSettings.Width, moduleSettings.Height));
        foreach(Vector2Int position in positions) {
            Port input = GetInputAtPosition(position);
            Port output = GetOutputAtPosition(position);
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
    #endregion
}

public class Port
{
    public Vector2Int position;
    public Facing direction;
    private Vector2Int connectedPosition;
    private ITransportable connectedObject;
    private AssemblyTravelingObject currentObject;

    public Port(Vector2Int position, Facing facing) {
        this.position = position;
        this.direction = facing;
        this.connectedPosition = position + facing.GetIntDirection();
    }

    public Port(PortSettings settings) : this(settings.position, settings.direction) {
        
    }

    public void Tick() {
        if(connectedObject != null) {
            connectedObject.ReceivedObject(ReceiveFromPort());
        }
    }
    
    public void SendToPort(AssemblyTravelingObject aObject) {
        currentObject = aObject;
    }

    public AssemblyTravelingObject ReceiveFromPort() {
        AssemblyTravelingObject obj = currentObject;
        currentObject = null;
        return obj;
    }

    public TransportState GetState() {
        return currentObject == null ? TransportState.Available : TransportState.Occupied;
    }

    public void OutputTo(ITransportable transportable) {
        this.connectedObject = transportable;
    }

}
