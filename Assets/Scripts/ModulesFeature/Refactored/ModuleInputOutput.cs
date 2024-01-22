using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleInputOutput : MonoBehaviour, IGridObject
{
    private ModuleSO moduleSettings;
    private Vector2Int originCell;
    private FactoryGrid grid;
    private int numRotations;

    private List<Port> inputPorts;
    private List<Port> outputPorts;

    private List<AssemblyTravelingObject> inputStorage;
    private List<AssemblyTravelingObject> outputStorage;
    private AssemblyLineSystem assemblyLineSystem;

    public void Initialize(ModuleSO moduleSettings, int numRotations, AssemblyLineSystem assemblyLineSystem) {
        this.moduleSettings = moduleSettings;
        this.numRotations = numRotations;
        this.assemblyLineSystem = assemblyLineSystem;
    }

    private void Awake() {
        inputStorage = new List<AssemblyTravelingObject>();
        outputStorage = new List<AssemblyTravelingObject>();
        inputPorts = new List<Port>();
        outputPorts = new List<Port>();
    }

    private void CreatePorts() {
        List<PortSettings> inputSettings = moduleSettings.GetInputs(numRotations);
        List<PortSettings> outputSettings = moduleSettings.GetOutputs(numRotations);

        foreach (PortSettings ps in inputSettings) {
            Port port = new Port(ps.relativePosition + originCell, ps.direction);
            inputPorts.Add(port);
        }
        foreach (PortSettings ps in outputSettings) {
            Port port = new Port(ps.relativePosition + originCell, ps.direction);
            outputPorts.Add(port);
        }
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

    private void Update() {
        if(this.grid == null) {
            return;
        }
        Tick();
    }

    private void Tick() { 
        foreach(Port p in inputPorts) {
            if(p.HasInput()) {
                inputStorage.Add(p.ReceiveFromPort());
            }
        }

        foreach(Port p in outputPorts) {
            if(!p.HasOutput() && outputStorage.Count > 0) {
                p.SendToPort(outputStorage[0]);
                outputStorage.RemoveAt(0);
            }
        }
    }

    private void PlacePorts() {
        foreach (Port p in inputPorts) {
            assemblyLineSystem.PlaceTransportablePiece(p);
        }

        foreach (Port p in outputPorts) {
            assemblyLineSystem.PlaceTransportablePiece(p);
        }
    }

    private void RemovePorts() {
        foreach (Port p in inputPorts) {
            assemblyLineSystem.RemoveTransportablePiece(p);
        }

        foreach (Port p in outputPorts) {
            assemblyLineSystem.RemoveTransportablePiece(p);
        }
    }

    public void OnPlacedOnGrid(Vector2Int startCell, FactoryGrid grid) {
        this.originCell = startCell;
        this.grid = grid;
        CreatePorts();
        PlacePorts();
    }

    public void DestroyObject() {
        RemoveFromGrid(grid);
        Destroy(gameObject);
    }

    public void RemoveFromGrid(FactoryGrid grid) {
        RemovePorts();
        grid.RemoveObject(originCell + moduleSettings.GetLayoutShape(numRotations)[0]);
    }

    #region Debug
    private void OnDrawGizmos() {
        if(grid == null) {
            return;
        }
  
        foreach(Port p in inputPorts) {
            Gizmos.color = Color.green;
            Vector2Int facingDirection = p.direction.GetIntDirection();
            Gizmos.DrawWireCube(grid.GetCellCenter(p.position - facingDirection), new Vector3(grid.CellSize.x, 3, grid.CellSize.y));
        }

        foreach(Port p in outputPorts) {
            Gizmos.color = Color.red;
            Vector2Int facingDirection = p.direction.GetIntDirection();
            Gizmos.DrawWireCube(grid.GetCellCenter(p.position + facingDirection), new Vector3(grid.CellSize.x, 3, grid.CellSize.y));    
        }
    }
    #endregion
}

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
}
