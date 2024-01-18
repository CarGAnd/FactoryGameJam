using System.Collections.Generic;
using UnityEngine;
public class AssemblyLine
{
    private LinkedList<ITransportable> pieces;
    private ITransportable startPiece;
    private ITransportable endPiece;
    private AssemblyLineSystem assemblyLineSystem;

    private Dictionary<AssemblyLine, ITransportable> connectingAssemblyLines;
    public AssemblyLine ParentLine { get; private set; }
    public AssemblyLine(AssemblyLineSystem assemblyLineSystem)
    {
        this.assemblyLineSystem = assemblyLineSystem;
        pieces = new();
        connectingAssemblyLines = new();
        assemblyLineSystem.SubscribeToTransportTick(OnTransportTick);
    }
    public List<AssemblyLine> GetAllConnections()
    {
        return new List<AssemblyLine>(connectingAssemblyLines.Keys);
    }
    
    //The dictionary logic will probably be useful when working on removal of assemblylines.
    public void AddConnectingAssemblyLine(AssemblyLine line, ITransportable endPiece)
    {
        line.ParentLine = this;
        if(connectingAssemblyLines.ContainsKey(line))
        {
            //This means that the line is already in the dictionary, it could be that it got changed.
            connectingAssemblyLines.Remove(line);
            //In that case we just move it to the new endpiece
            connectingAssemblyLines.Add(line, endPiece);
        }
        else
        {
            connectingAssemblyLines.Add(line, endPiece);
        }
        line.GetEndPiece().SetNextTransportable(endPiece);
    }
    public void OnTransportTick()
    {
        var node = pieces.Last;
        while(node != null)
        {
            node.Value.TransportTick();
            node = node.Previous;
        }
    }

    public void RemoveLineFromTick()
    {
        assemblyLineSystem.UnsubscribeFromTransportTick(OnTransportTick);
    }

    public ITransportable GetEndPiece()
    {
        return endPiece;
    }
    public ITransportable GetStartPiece()
    {
        return startPiece;
    }
    public bool IsPieceNewEnd(ITransportable piece)
    {
        if(endPiece == null)
        {
            return true;
        }
        return endPiece.GetNextCellCoords() == piece.GetGridCoords();
    }
    public bool IsPieceNewStart(ITransportable piece)
    {
        if(startPiece == null)
        {
            return true;
        }
        return startPiece.GetGridCoords() == piece.GetNextCellCoords();    
    }
    public void AddPiece(ITransportable piece)
    {
        bool isEnd = IsPieceNewEnd(piece);
        bool isStart = IsPieceNewStart(piece);

        if (isEnd && isStart)
        {
            startPiece = piece;
            endPiece = piece;
            pieces.Clear();
            pieces.AddFirst(piece);
        }
        else if (isEnd)
        {
            endPiece.SetNextTransportable(piece);
            endPiece = piece;
            pieces.AddLast(piece);
        }
        else if (isStart)
        {
            piece.SetNextTransportable(startPiece);
            startPiece = piece;
            pieces.AddFirst(piece);
        }
    }
    public void MoveConnectingLines(AssemblyLine newLine)
    {
        foreach(var kvp in connectingAssemblyLines)
        {
            newLine.AddConnectingAssemblyLine(kvp.Key, kvp.Value);
        }
    }

    public void ReplaceExistingLine(AssemblyLine currentLine, AssemblyLine newLine, ITransportable piece)
    {
        RemoveConnection(currentLine);
        connectingAssemblyLines.Add(newLine, piece);
        newLine.ParentLine = this;
    }

    public void RemoveConnection(AssemblyLine line)
    {
        connectingAssemblyLines.Remove(line);
    }

    private void UpdateStartEndPieces()
    {
        if (pieces.Count > 0)
        {
            startPiece = pieces.First.Value;
            endPiece = pieces.Last.Value;
        }
    }
    public void AppendLine(AssemblyLine newLine)
    {
        foreach (var node in pieces)
        {
            newLine.pieces.Last.Value.SetNextTransportable(node);
            newLine.pieces.AddLast(node);
        }
        newLine.UpdateStartEndPieces();
    }
    public AssemblyLine GetIntersectedAssemblyLine(Vector2Int coords, HashSet<AssemblyLine> visitedLines, out ITransportable intersectedPiece)
    {
        if(visitedLines.Contains(this))
        {
            intersectedPiece = null;
            return null;
        }
        visitedLines.Add(this);
        AssemblyLine foundLine = null;
        foreach(ITransportable piece in pieces)
        {
            if(piece.GetGridCoords() == coords)
            {
                intersectedPiece = piece;
                return this;
            }
        }
        foreach(AssemblyLine line in connectingAssemblyLines.Keys)
        {
            foundLine = line.GetIntersectedAssemblyLine(coords, visitedLines,  out intersectedPiece);
            if(foundLine != null)
            {
                return foundLine;
            }
        }
        intersectedPiece = null;
        return foundLine;
    }
    public void HandleLoop(AssemblyLine line, ITransportable piece)
    {
        AddConnectingAssemblyLine(line, piece);
    }
    public void CleanUp()
    {
        RemoveLineFromTick();
        pieces.Clear();
        connectingAssemblyLines.Clear();
        startPiece = null;
        endPiece = null;
    }

////////////////DEBUGGING///////////////////////////
    public void DebugLine()
    {
        string mainLineStr = "Main Line: " + FormatLineString(this);
        Debug.Log(mainLineStr);
        HashSet<AssemblyLine> visitedLines = new HashSet<AssemblyLine>();
        // Debugging connecting lines
        foreach(var kvp in connectingAssemblyLines)
        {
            visitedLines.Clear();
            visitedLines.Add(this);
            Debug.Log(DebugConnectingLine(kvp.Key, kvp.Value, 1, visitedLines));
        }
    }

    private string DebugConnectingLine(AssemblyLine line, ITransportable connectingPiece, int depth, HashSet<AssemblyLine> visitedLines)
    {
        if (visitedLines.Contains(line))
            return "";
        visitedLines.Add(line);
        string connectingLineStr = "Connecting Line at depth " + depth + " at connecting piece: "+ connectingPiece.GetGridCoords() + "\n";
        connectingLineStr += FormatLineString(line);
        foreach(var kvp in line.connectingAssemblyLines)
        {
            connectingLineStr += "\n";
            connectingLineStr += DebugConnectingLine(kvp.Key, kvp.Value, depth + 1, visitedLines);
        }
        return connectingLineStr;
    }

    private string FormatLineString(AssemblyLine line)
    {
        string lineStr = "";
        lineStr += line.GetStartPiece().GetGridCoords() + " -> ";
        foreach (ITransportable piece in line.pieces)
        {
            if (piece == line.GetStartPiece() || piece == line.GetEndPiece())
            {
                continue;
            }
            lineStr += piece.GetGridCoords() + " ";
        }
        lineStr += " -> " + line.GetEndPiece().GetGridCoords();
        return lineStr;
    }
}

public enum Direction
{
    Default = 0,
    Forwards = 10,
    Backwards = 20,
    UniDirectional = 30
}
