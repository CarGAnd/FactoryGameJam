using System.Collections.Generic;
using System.Linq;
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

    //To Be Removed -- It's part of debugging
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
        AssemblyPiece intersectingPiece;
        AssemblyLine intersectedLine = GetIntersectedAssemblyLine(piece, out intersectingPiece);
        List<AssemblyLine> endLines = FindLinesPieceIsEndOf(piece);
        

        //Should cover all scenarios where we're combining two or more lines.
        if(intersectedLine != null && endLines.Count > 0)
        {
            //We start by considering the scenario where the two lines are facing the same way.
            AssemblyLine correctEndLine = FindCorrectEndLine(endLines, intersectingPiece);
            if(correctEndLine != null)
            {
                AssemblyLine resultingLine = MergeLines(intersectedLine, correctEndLine, piece);
                resultingLine.DebugLine();
                HandleConnectingLines(resultingLine, endLines.Except(new[]{correctEndLine}).ToList(), piece);
                return;
            }
            //Alright we assume the endlines do not face the same way as the intersected line. 
            else
            {
                //In which case we first need to see if the new piece is part of an existing endline.
                correctEndLine = FindCorrectEndLine(endLines, piece);
                if(correctEndLine != null)
                {
                    //We add the piece to our end line.
                    correctEndLine.AddPiece(piece);
                    //And now we need to ensure that all endlines are connecting lines to this line.
                    HandleConnectingLines(correctEndLine, endLines.Except(new[]{correctEndLine}).ToList(), piece);
                    //And now we need to add this line as a connecting line to the intersected line, at intersected piece.
                    HandleConnectingLines(intersectedLine, new List<AssemblyLine>{correctEndLine}, intersectingPiece);
                    return;
                }
                //In case they don't face the same way as the piece, but still intersect with the piece.
                else
                {
                    AssemblyLine newLine = CreateNewLine(piece);
                    //We add the endlines as connecting lines to this line
                    HandleConnectingLines(newLine, endLines, piece);
                    //And now we need to add this line as a connecting line to the intersected line, at intersected piece.
                    HandleConnectingLines(intersectedLine, new List<AssemblyLine>{newLine}, intersectingPiece);
                }
            }
        }
        else if(intersectedLine != null)
        {
            if(intersectingPiece.facing == piece.facing)
            {
                intersectedLine.AddPiece(piece);
            }
            else
            {
                AssemblyLine newLine = CreateNewLine(piece);
                HandleConnectingLines(intersectedLine, new List<AssemblyLine>{newLine}, intersectingPiece);
            }
        }
        else if(endLines.Count > 0)
        {
            AssemblyLine correctEndLine = FindCorrectEndLine(endLines, piece);
            if(correctEndLine != null)
            {
                correctEndLine.AddPiece(piece);
                HandleConnectingLines(correctEndLine, endLines.Except(new[]{correctEndLine}).ToList(), piece);
            }
            else
            {
                AssemblyLine newLine = CreateNewLine(piece);
                HandleConnectingLines(newLine, endLines, piece);
            }
        }
        else
        {
            CreateNewLine(piece);
        }
    }
    private AssemblyLine GetIntersectedAssemblyLine(AssemblyPiece piece, out AssemblyPiece intersectedPiece)
    {
        foreach(AssemblyLine line in assemblyLines)
        {
            if(line.GetIntersectedAssemblyLine(piece.GetGridCoords() + piece.Movement(), out intersectedPiece) != null)
            {
                return line;
            }
        }
        intersectedPiece = null;
        return null;
    }

    private AssemblyLine CreateNewLine(AssemblyPiece piece)
    {
        AssemblyLine newLine = new AssemblyLine();
        newLine.AddPiece(piece);
        assemblyLines.Add(newLine);
        return newLine;
    }

    private AssemblyLine MergeLines(AssemblyLine startLine, AssemblyLine endLine, AssemblyPiece piece)
    {
        //We add the piece to startline, which also means it becomes the new start piece of that line.
        startLine.AddPiece(piece);
        //Now we need to align the two lines' pieces.

        startLine.GetStartPiece().previousPiece = endLine.GetEndPiece();
        endLine.GetEndPiece().nextPiece = startLine.GetStartPiece();

        //we now create a list of assemblyPieces, where we should add the endline first, as it's the furthest back.

        List<AssemblyPiece> combinedPieces = new();
        combinedPieces.AddRange(endLine.AddAllPieces());
        combinedPieces.AddRange(startLine.AddAllPieces());

        //We create a new AssemblyLine using these pieces. We also need to make sure we're moving the connecting lines.

        AssemblyLine newLine = new AssemblyLine(combinedPieces);
        startLine.MoveConnectingLines(newLine);
        endLine.MoveConnectingLines(newLine);

        //We ensure the new lines are recorded and the old ones are disposed of.

        RemoveAssemblyLine(startLine);
        RemoveAssemblyLine(endLine);
        assemblyLines.Add(newLine);

        return newLine;
    }

    private void HandleConnectingLines(AssemblyLine mainLine, List<AssemblyLine> connectingLines, AssemblyPiece connectingPiece)
    {
        foreach(AssemblyLine line in connectingLines)
        {
            mainLine.AddConnectingAssemblyLine(line, connectingPiece);
            assemblyLines.Remove(line);
        }
    }

    private List<AssemblyLine> FindLinesPieceIsEndOf(AssemblyPiece piece)
    {
        //Linq returns an empty list if no lines are found and not null.
        return assemblyLines.Where(line => line.IsPieceNewEnd(piece)).ToList();
    }

    private AssemblyLine FindCorrectEndLine(List<AssemblyLine> endLines, AssemblyPiece piece)
    {
        return endLines.FirstOrDefault(line => line.GetEndPiece().facing == piece.facing);
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
