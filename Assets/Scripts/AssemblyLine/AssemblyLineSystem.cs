using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class AssemblyLineSystem : MonoBehaviour
{
    public static AssemblyLineSystem Instance;
    private UnityEvent Tick;
    [SerializeField] private Grid grid;
    [SerializeField] private float tickRate = 0.200f;
    private float currentTick = 0f;
    private List<AssemblyLine> assemblyLines;
    private AssemblyLineManager assemblyLineManager;

    //To Be Removed -- It's part of debugging
    public GameObject travelingObjectPrefab;

    private void Awake()
    {
        Instance = this;
        assemblyLines = new List<AssemblyLine>();
        Tick = new UnityEvent();
        assemblyLineManager = new AssemblyLineManager(assemblyLines);
    }

    private void Update()
    {
        currentTick += Time.deltaTime;
        if(currentTick >= tickRate)
        {
            Tick.Invoke();
            currentTick = 0f;
        }
    }
    [Button]
    public void DebugAssemblyLines()
    {
        foreach(AssemblyLine line in assemblyLines)
        {
            line.DebugLine();
        }
    }
    [Button]
    public void DebugNumberOfAssemblyLines()
    {
        Debug.Log("Number of Assembly Lines: " + assemblyLines.Count);
    }
    [Button]
    public void AddTravelingAssemblyPiece()
    {
        foreach(AssemblyLine line in assemblyLines)
        {
            SpawnAtAllConnectingLines(line);
        }

    }

    private void SpawnAtAllConnectingLines(AssemblyLine line)
    {
        List<AssemblyLine> connectingLines = line.GetAllConnections();

        foreach(AssemblyLine connectingLine in connectingLines)
        {
            SpawnAtAllConnectingLines(connectingLine);
        }

        SpawnTravelingPieceAtPiece(line.GetStartPiece());
    }

    private void SpawnTravelingPieceAtPiece(AssemblyPiece piece)
    {
        AssemblyTravelingObject travelingObject = Instantiate(travelingObjectPrefab, grid.GetCellCenter(piece.GetGridCoords()), Quaternion.identity).GetComponent<AssemblyTravelingObject>();
        piece.ReceiveTravellingObject(travelingObject);
    }

    public AssemblyPiece CreateAssemblyPiece(AssemblyPieceData assemblyPieceData)
    {
        AssemblyPieceType type = assemblyPieceData.type;
        AssemblyPiece newPiece = null;

        switch (type)
        {
            case AssemblyPieceType.ConveyerBelt:
                newPiece = new ConveyerBelt(assemblyPieceData);
                break;
            case AssemblyPieceType.Cannon:
                //newPiece = new Cannon(data);
                break;
        }
        return newPiece;
    }

    public void PlaceAssemblyPiece(AssemblyPiece piece)
    {
        assemblyLineManager.PlaceAssemblyPiece(piece);
    }

    public void SubscribeToTick(UnityAction action)
    {
        Tick.AddListener(action);
    }

    public void UnsubscribeFromTick(UnityAction action)
    {
        Tick.RemoveListener(action);
    }
}
