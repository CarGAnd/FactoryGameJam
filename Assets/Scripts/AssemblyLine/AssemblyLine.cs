using System.Collections.Generic;
using UnityEngine;
public class AssemblyLine
{
    private List<AssemblyPiece> pieces;
    private AssemblyPiece startPiece;
    private AssemblyPiece endPiece;
    private List<AssemblyLine> connectingAssemblyLines;

    public AssemblyLine()
    {
        pieces = new List<AssemblyPiece>();
        connectingAssemblyLines = new List<AssemblyLine>();

        AssemblyLineSystem.Instance.SubscribeToTick(OnTick);
    }
    public AssemblyLine(List<AssemblyPiece> assemblyPieces)
    {
        pieces = assemblyPieces;
        connectingAssemblyLines = new List<AssemblyLine>();

        UpdateStartEndPieces();
        AssemblyLineSystem.Instance.SubscribeToTick(OnTick);
    }

    public AssemblyLine GetAssemblyLineFromPiece(AssemblyPiece piece)
    {
        if (piece == null)
            return null;

        foreach(AssemblyPiece heldPiece in pieces)
        {
            if (heldPiece == piece)
                return this;
        }
        foreach(AssemblyLine connectingLine in connectingAssemblyLines)
        {
            if(connectingLine.GetAssemblyLineFromPiece(piece) != null)
            return connectingLine.GetAssemblyLineFromPiece(piece);
        }
        return null;
    }

    public List<AssemblyLine> GetAllConnections()
    {
        return connectingAssemblyLines;
    }
    
    public void AddConnectingAssemblyLine(AssemblyLine line)
    {
        connectingAssemblyLines.Add(line);
    }

    public void AddConnectingAssemblyLine(List<AssemblyLine> lines)
    {
        connectingAssemblyLines.AddRange(lines);
    }

    public void OnTick()
    {
        for(int i = pieces.Count-1; i >= 0; i--)
        {
            pieces[i].OnTick();
        }
    }

    public void RemoveLineFromTick()
    {
        AssemblyLineSystem.Instance.UnsubscribeFromTick(OnTick);
    }

    public AssemblyPiece GetEndPiece()
    {
        return endPiece;
    }
    public AssemblyPiece GetStartPiece()
    {
        return startPiece;
    }
    public bool IsPieceNewEnd(AssemblyPiece piece)
    {
        if(endPiece == null)
        {
            return true;
        }
        return endPiece.GetCell().GetCellCoordinates() + endPiece.Movement() == piece.GetCell().GetCellCoordinates();
    }
    public bool IsPieceNewStart(AssemblyPiece piece)
    {
        if(startPiece == null)
        {
            return true;
        }
        return startPiece.GetCell().GetCellCoordinates() == piece.GetCell().GetCellCoordinates() + piece.Movement();
    }
    public void AddPiece(AssemblyPiece piece)
    {
        bool isEnd = IsPieceNewEnd(piece);
        bool isStart = IsPieceNewStart(piece);


        if(isEnd)
        {
            if(endPiece != null)
            {
                endPiece.nextPiece = piece;
                piece.previousPiece = endPiece;
            }
            endPiece = piece;
        }
        if(isStart)
        {
            if(startPiece != null)
            {
                startPiece.previousPiece = piece;
                piece.nextPiece = startPiece;
            }
            startPiece = piece;
        }
        pieces.Add(piece);
    }
    public List<AssemblyPiece> AddAllPieces()
    {
        List<AssemblyPiece> allPieces = new List<AssemblyPiece>();
        allPieces.AddRange(pieces);
        return allPieces;
    }

    public void CleanUp()
    {
        RemoveLineFromTick();
        pieces.Clear();
        connectingAssemblyLines.Clear();
        startPiece = null;
        endPiece = null;
    }

    private void UpdateStartEndPieces()
    {
        foreach(AssemblyPiece piece in pieces)
        {
            if(piece.nextPiece == null)
            {
                endPiece = piece;
            }
            if(piece.previousPiece == null)
            {
                startPiece = piece;
            }
        }
    }

    public void DebugLine()
    {
        string line = "";
        line += startPiece.GetCell().GetCellCoordinates() + " -> ";
        foreach(AssemblyPiece piece in pieces)
        {
            if(piece == startPiece || piece == endPiece)
            {
                continue;
            }
            line += piece.GetCell().GetCellCoordinates() + " ";
        }
        line += " -> "+ endPiece.GetCell().GetCellCoordinates();
        Debug.Log(line);
    }

}

public enum Direction
{
    Default = 0,
    Forwards = 10,
    Backwards = 20,
    UniDirectional = 30
}
