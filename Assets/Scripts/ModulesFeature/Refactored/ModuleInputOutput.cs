using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModuleInputOutput : MonoBehaviour, IGridObject
{
    public UnityEvent<AssemblyTravelingObject> receivedObject;

    private ModuleSO moduleSettings;
    private Vector2Int originCell;
    private FactoryGrid grid;
    private Facing facing;

    private List<Port> inputPorts;
    private List<Port> outputPorts;

    private List<AssemblyTravelingObject> inputStorage;
    private List<AssemblyTravelingObject> outputStorage;
    private AssemblyLineSystem assemblyLineSystem;

    public void Initialize(ModuleSO moduleSettings, Facing facing, AssemblyLineSystem assemblyLineSystem) {
        this.moduleSettings = moduleSettings;
        this.facing = facing;
        this.assemblyLineSystem = assemblyLineSystem;
    }

    private void Awake() {
        inputStorage = new List<AssemblyTravelingObject>();
        outputStorage = new List<AssemblyTravelingObject>();
        inputPorts = new List<Port>();
        outputPorts = new List<Port>();
    }

    private void CreatePorts() {
        List<PortSettings> inputSettings = moduleSettings.GetInputs(facing);
        List<PortSettings> outputSettings = moduleSettings.GetOutputs(facing);

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
                AssemblyTravelingObject newObject = p.ReceiveFromPort();
                inputStorage.Add(newObject);
                receivedObject.Invoke(newObject);
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
        grid.RemoveObject(originCell + moduleSettings.GetLayoutShape(facing)[0]);
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
