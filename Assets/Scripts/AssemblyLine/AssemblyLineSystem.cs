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

    //To Be Removed
    public GameObject travelingObjectPrefab;

    private void Awake()
    {
        Instance = this;
        assemblyLines = new List<AssemblyLine>();
        Tick = new UnityEvent();
        Tick.AddListener(DebugTick);
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
    public void DebugTick()
    {
        Debug.Log("Tick");
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
            AssemblyPiece piece = line.GetStartPiece();

            AssemblyTravelingObject travelingObject = Instantiate(travelingObjectPrefab, grid.GetCellCenter(piece.GetGridCoords()), Quaternion.identity).GetComponent<AssemblyTravelingObject>();
            piece.ReceiveTravellingObject(travelingObject);
        }
    }

    public void PlaceAssemblyPiece(Vector3 worldPosition, AssemblyPieceData data)
    {
        if(grid.PositionIsOccupied(worldPosition))
        {
            Debug.Log("Position is occupied");
            return;
        }

        AssemblyPieceType type = data.type;
        AssemblyPiece newPiece = null;

        switch(type)
        {
            case AssemblyPieceType.ConveyerBelt:
                newPiece = new ConveyerBelt(data, grid.GetCellCoords(worldPosition), grid);
                break;
            case AssemblyPieceType.Cannon:
                //newPiece = new Cannon(data);
                break;
        }

        if(newPiece != null)
        {
            grid.PlaceObject(newPiece, grid.GetCellCoords(worldPosition));
            AddPieceToAssemblyLine(newPiece);

        }
    }

    private void AddPieceToAssemblyLine(AssemblyPiece piece)
    {
        AssemblyLine startLine = null;
        List<AssemblyLine> endLines = new();
        foreach(AssemblyLine line in assemblyLines)
        {
            if(line.IsPieceNewStart(piece))
            {
                startLine = line;
            }
            else if(line.IsPieceNewEnd(piece))
            {
                endLines.Add(line);
            }
        }
        
        if(startLine != null)
        {
            if(endLines.Count > 0)
            {
                HandleMerging(piece, endLines, startLine);
            }
            else
            {
                startLine.AddPiece(piece);
            }
        }
        else if(endLines.Count > 0)
        {
            HandleMerging(piece, endLines);
        }
        else
        {
            AssemblyLine newLine = new AssemblyLine();
            newLine.AddPiece(piece);
            assemblyLines.Add(newLine);
        }
    }

    private void HandleMerging(AssemblyPiece piece, List<AssemblyLine> endLines, AssemblyLine startLine = null)
    {
        //Handles merging if startline is not null, meaning the piece is connecting 2 lines.
        if(startLine != null)
        {
            startLine.AddPiece(piece);
            AssemblyLine newLine = ConnectCorrectLine(endLines,piece,startLine);
            foreach(AssemblyLine line in endLines)
            {
                if(newLine != null)
                {
                    newLine.AddConnectingAssemblyLine(line);
                }
                else
                {
                    startLine.AddConnectingAssemblyLine(line);
                }
                RemoveAssemblyLine(line);
            }
        }
        //Handles merging if startline is null, meaning the piece is the end of line(s).
        else
        {
            AssemblyLine newLine = ConnectCorrectEndLine(endLines,piece);
            if(newLine == null)
            {
                newLine = new AssemblyLine();
                newLine.AddPiece(piece);
                assemblyLines.Add(newLine);
            }
            foreach(AssemblyLine line in endLines)
            {
                newLine.AddConnectingAssemblyLine(line);
                RemoveAssemblyLine(line);
            }
        }
    }

    private AssemblyLine ConnectCorrectLine(List<AssemblyLine> endLines, AssemblyPiece piece, AssemblyLine start = null)
    {
        // the function also removes the usage of startLine meaning the addition of the piece above, only works if the start isn't replaced by new line, but will not toss an error.
        foreach(AssemblyLine line in endLines)
        {
            if(line.GetEndPiece().facing == piece.facing)
            {
                endLines.Remove(line);
                return CombineAssemblyLines(start, line, piece);
            } 
        }
        return null;
    }

    //Handles removal of previous lines, and the addition of the new one.
    private AssemblyLine CombineAssemblyLines(AssemblyLine line1, AssemblyLine line2, AssemblyPiece piece)
    {
        List<AssemblyPiece> newLinePieces = new();

        AssemblyPiece startLine1 = line1.GetStartPiece();
        AssemblyPiece endLine2 = line2.GetEndPiece();

        endLine2.nextPiece = piece;
        piece.previousPiece = endLine2;

        piece.nextPiece = startLine1;
        startLine1.previousPiece = piece;
        

        newLinePieces.AddRange(line1.AddAllPieces());
        newLinePieces.AddRange(line2.AddAllPieces());

        AssemblyLine newLine = new AssemblyLine(newLinePieces);
        newLine.AddConnectingAssemblyLine(line1.GetAllConnections());
        newLine.AddConnectingAssemblyLine(line2.GetAllConnections());


        RemoveAssemblyLine(line1);
        RemoveAssemblyLine(line2);
        assemblyLines.Add(newLine);
        
        return newLine;
    }

    private AssemblyLine ConnectCorrectEndLine(List<AssemblyLine> endLines, AssemblyPiece piece)
    {
        foreach(AssemblyLine line in endLines)
        {
            if(line.GetEndPiece().facing == piece.facing)
            {
                line.AddPiece(piece);
                endLines.Remove(line);
                return line;
            } 
        }
        return null;
    }

    public void SubscribeToTick(UnityAction action)
    {
        Tick.AddListener(action);
    }

    public void UnsubscribeFromTick(UnityAction action)
    {
        Tick.RemoveListener(action);
    }

    private void RemoveAssemblyLine(AssemblyLine line)
    {
        line.CleanUp();
        assemblyLines.Remove(line);
    }


}
