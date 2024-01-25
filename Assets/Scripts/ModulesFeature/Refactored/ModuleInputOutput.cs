using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModuleInputOutput : MonoBehaviour, IGridObject
{
    [HideInInspector] public UnityEvent<AssemblyTravelingObject> receivedObject;
    [HideInInspector] public UnityEvent<AssemblyTravelingObject, Vector2Int, Vector2Int> sentObject;
    public FactoryGrid Grid { get; private set; }

    private ModuleSO moduleSettings;
    private Vector2Int originCell;
    private Facing facing;

    private List<Port> inputPorts;
    private List<Port> outputPorts;
    private List<Port> allPorts;

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
        allPorts = new List<Port>();
    }

    private void CreatePorts() {
        List<PortSettings> portSettings = moduleSettings.GetPorts(facing, originCell); 
        foreach (PortSettings ps in portSettings) {
            Port port = new Port(ps.gridPosition, ps.inputDirection, ps.outputDirection);
            if (ps.isInput) {
                inputPorts.Add(port);
            }
            if (ps.isOutput) {
                outputPorts.Add(port);
            }
            allPorts.Add(port);
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
        if(this.Grid == null) {
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
                AssemblyTravelingObject obj = outputStorage[0];
                p.SendToPort(obj);
                outputStorage.RemoveAt(0);
                sentObject.Invoke(obj, p.GetGridCoords(), p.GetNextCellCoords());
            }
        }
    }

    private void PlacePorts() {
        foreach(Port p in allPorts) {
            assemblyLineSystem.PlaceTransportablePiece(p);
        }
    }

    private void RemovePorts() {
        foreach (Port p in allPorts) {
            assemblyLineSystem.RemoveTransportablePiece(p);
        }
    }

    public void OnPlacedOnGrid(Vector2Int startCell, FactoryGrid grid) {
        this.originCell = startCell;
        this.Grid = grid;
        CreatePorts();
        PlacePorts();
    }

    public void DestroyObject() {
        RemoveFromGrid(Grid);
        Destroy(gameObject);
    }

    public void RemoveFromGrid(FactoryGrid grid) {
        RemovePorts();
        grid.RemoveObject(originCell + moduleSettings.GetLayoutShape(facing)[0]);
    }

    #region Debug
    private void OnDrawGizmos() {
        if(Grid == null) {
            return;
        }
        Matrix4x4 gizmoMatrix = new Matrix4x4();
        gizmoMatrix.SetTRS(Grid.Origin, Grid.Rotation, Vector3.one);
        Gizmos.matrix = gizmoMatrix;
  
        foreach(Port p in inputPorts) {
            Gizmos.color = Color.green;
            Vector2Int facingDirection = p.Facing.GetIntDirection();
            Vector3 cellCenter = Grid.GetCellCenter(p.GetGridCoords() - facingDirection);
            Vector3 cubePos = Quaternion.Inverse(Grid.Rotation) * (cellCenter - Grid.Origin);
            Gizmos.DrawWireCube(cubePos, new Vector3(Grid.CellSize.x, 3, Grid.CellSize.y));
        }

        foreach(Port p in outputPorts) {
            Gizmos.color = Color.red;
            Vector2Int facingDirection = p.Facing.GetIntDirection();
            Vector3 cellCenter = Grid.GetCellCenter(p.GetGridCoords() + facingDirection);
            Vector3 cubePos = Quaternion.Inverse(Grid.Rotation) * (cellCenter - Grid.Origin);
            Gizmos.DrawWireCube(cubePos, new Vector3(Grid.CellSize.x, 3, Grid.CellSize.y));    
        }
        Gizmos.matrix = Matrix4x4.identity;
    }
    #endregion
}
