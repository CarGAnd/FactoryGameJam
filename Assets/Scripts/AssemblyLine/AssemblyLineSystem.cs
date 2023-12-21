using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AssemblyLineSystem : MonoBehaviour
{
    public static AssemblyLineSystem Instance;
    public UnityEvent Tick;
    [SerializeField] private Grid grid;

    private List<AssemblyLine> assemblyLines;

    private void Awake()
    {
        Instance = this;
        assemblyLines = new List<AssemblyLine>();
    }

    public void PlaceAssemblyPiece(Vector3 worldPosition, AssemblyPieceData data)
    {
        if(grid.PositionIsOccupied(worldPosition))
        {
            return;
        }

        AssemblyPieceType type = data.type;
        AssemblyPiece newPiece = null;

        switch(type)
        {
            case AssemblyPieceType.ConveyerBelt:
                newPiece = new ConveyerBelt(data, grid.GetCell(worldPosition));
                break;
            case AssemblyPieceType.Cannon:
                //newPiece = new Cannon(data);
                break;
        }

        if(newPiece != null)
        {
            grid.PlaceObject(newPiece, grid.GetCell(worldPosition));
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
                HandleMerging(piece, startLine, endLines);
            }
            startLine.AddPiece(piece);
        }
        else if(endLines.Count > 0)
        {
            AssemblyLine newStartLine = new AssemblyLine();
            HandleMerging(piece, newStartLine, endLines);
            newStartLine.AddPiece(piece);
        }
        else
        {
            AssemblyLine newLine = new AssemblyLine();
            newLine.AddPiece(piece);
            assemblyLines.Add(newLine);
        }
    }

    private void HandleMerging(AssemblyPiece piece, AssemblyLine startLine, List<AssemblyLine> endLines)
    {
        AssemblyLine newLine = ConnectCorrectLine(startLine, endLines, piece);
        foreach (AssemblyLine line in endLines)
        {
            if (newLine != null)
            {
                newLine.AddConnectingAssemblyLine(line);
            }
            else
            {
                startLine.AddConnectingAssemblyLine(line);
            }
        }
    }

    private AssemblyLine ConnectCorrectLine(AssemblyLine start, List<AssemblyLine> endLines, AssemblyPiece piece)
    {
        // the function also removes the usage of startLine meaning the addition of the piece above, only works if the start isn't replaced by new line, but will not toss an error.
        foreach(AssemblyLine line in endLines)
        {
            if(line.GetEndPiece().facing == piece.facing)
            {
                endLines.Remove(line);
                AssemblyLine newAssemblyLine = CombineAssemblyLines(start, line, piece);
                return newAssemblyLine;
            } 
        }
        return null;
    }

    //Handles removal of previous lines, and the addition of the new one.
    //Handles addition of the new piece to the new line.
    private AssemblyLine CombineAssemblyLines(AssemblyLine line1, AssemblyLine line2, AssemblyPiece piece)
    {
        List<AssemblyPiece> newLinePieces = new();

        AssemblyPiece startLine1 = line1.GetStartPiece();
        AssemblyPiece endLine2 = line2.GetEndPiece();

        endLine2.NextPiece = piece;
        piece.PreviousPiece = endLine2;

        piece.NextPiece = startLine1;
        startLine1.PreviousPiece = piece;
        

        newLinePieces.AddRange(line1.AddAllPieces());
        newLinePieces.Add(piece);
        newLinePieces.AddRange(line2.AddAllPieces());

        AssemblyLine newLine = new AssemblyLine(newLinePieces);
        newLine.AddConnectingAssemblyLine(line1.GetAllConnections());
        newLine.AddConnectingAssemblyLine(line2.GetAllConnections());


        RemoveAssemblyLine(line1);
        RemoveAssemblyLine(line2);
        assemblyLines.Add(newLine);
        
        return newLine;
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
