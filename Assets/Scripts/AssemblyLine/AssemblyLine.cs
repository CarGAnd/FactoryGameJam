using System.Collections.Generic;
using UnityEngine;
public class AssemblyLine
{
    private LinkedList<ITransportable> pieces;
    private ITransportable startPiece;
    private ITransportable endPiece;
    private List<AssemblyLine> connectingAssemblyLines;

    private Dictionary<ITransportable, List<AssemblyLine>> connectingLinesPiece;
    public AssemblyLine()
    {
        pieces = new();
        connectingAssemblyLines = new List<AssemblyLine>();
        connectingLinesPiece = new();

        AssemblyLineSystem.Instance.SubscribeToTransportTick(OnTransportTick);
    }
    public AssemblyLine(IEnumerable<ITransportable> assemblyPieces)
    {
        pieces = new LinkedList<ITransportable>(assemblyPieces);
        connectingAssemblyLines = new List<AssemblyLine>();
        connectingLinesPiece = new();

        UpdateStartEndPieces();
        AssemblyLineSystem.Instance.SubscribeToTransportTick(OnTransportTick);
    }
    public List<AssemblyLine> GetAllConnections()
    {
        return connectingAssemblyLines;
    }
    
    //The dictionary logic will probably be useful when working on removal of assemblylines.
    public void AddConnectingAssemblyLine(AssemblyLine line, ITransportable endPiece)
    {
        connectingAssemblyLines.Add(line);
        line.SetConnectingEnd(endPiece);
        if(connectingLinesPiece.ContainsKey(endPiece))
        {
            if(!connectingLinesPiece[endPiece].Contains(line))
            {
                connectingLinesPiece[endPiece].Add(line);
            }
        }
        else
        {
            connectingLinesPiece.Add(endPiece, new List<AssemblyLine>(){line});
        }
    }
    public void AddConnectingAssemblyLine(List<AssemblyLine> lines, ITransportable endPiece)
    {
        foreach(AssemblyLine line in lines)
        {
            AddConnectingAssemblyLine(line, endPiece);
        }
    }

    public void AddConnectingAssemblyLine(List<AssemblyLine> lines)
    {
        connectingAssemblyLines.AddRange(lines);
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
        AssemblyLineSystem.Instance.UnsubscribeFromTransportTick(OnTransportTick);
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
        if(isEnd && isStart)
        {
            if(startPiece != null && endPiece != null)
            {
                startPiece.SetPreviousPiece(piece);
                endPiece.SetNextPiece(piece);
                piece.SetNextPiece(startPiece);
                piece.SetPreviousPiece(endPiece);
            }
            startPiece = piece;
            endPiece = piece;
            pieces.AddFirst(piece);
        }
        else
        {
            if(isEnd)
            {
                if(endPiece != null)
                {
                    endPiece.SetNextPiece(piece);
                    piece.SetPreviousPiece(endPiece);
                }
                endPiece = piece;
                pieces.AddLast(piece);
            }
            if(isStart)
            {
                if(startPiece != null)
                {
                    startPiece.SetPreviousPiece(piece);
                    piece.SetNextPiece(startPiece);
                }
                startPiece = piece;
                pieces.AddFirst(piece);
            }
        }
    }
    public List<ITransportable> AddAllPieces()
    {
        List<ITransportable> allPieces = new List<ITransportable>();
        allPieces.AddRange(pieces);
        return allPieces;
    }

    public void SetConnectingEnd(ITransportable piece)
    {
        if(endPiece != null)
        {
            endPiece.SetNextPiece(piece);
        }
    }

    
    public void MoveConnectingLines(AssemblyLine newLine)
    {
        foreach(var kvp in connectingLinesPiece)
        {
            newLine.AddConnectingAssemblyLine(kvp.Value, kvp.Key);
        }
    }

    public void ReplaceExistingLine(AssemblyLine currentLine, AssemblyLine newLine, ITransportable piece)
    {
        connectingAssemblyLines.Remove(currentLine);
        connectingAssemblyLines.Add(newLine);
        connectingLinesPiece[piece].Remove(currentLine);
        connectingLinesPiece[piece].Add(newLine);
    }

    public void RemoveConnection(AssemblyLine line)
    {
        connectingAssemblyLines.Remove(line);
        foreach(var kvp in connectingLinesPiece)
        {
            if(kvp.Value.Contains(line))
            {
                kvp.Value.Remove(line);
            }
        }
    }

    private void UpdateStartEndPieces()
    {
        foreach(ITransportable piece in pieces)
        {
            if(piece.NextPiece == null || !pieces.Contains(piece.NextPiece))
            {
                endPiece = piece;
            }
            if(piece.PreviousPiece == null || !pieces.Contains(piece.PreviousPiece))
            {
                startPiece = piece;
            }
        }
    }
    public AssemblyLine GetIntersectedAssemblyLine(Vector2Int coords, out ITransportable intersectedPiece)
    {
        AssemblyLine foundLine = null;
        foreach(ITransportable piece in pieces)
        {
            if(piece.GetGridCoords() == coords)
            {
                intersectedPiece = piece;
                return this;
            }
        }
        foreach(AssemblyLine line in connectingAssemblyLines)
        {
            foundLine = line.GetIntersectedAssemblyLine(coords, out intersectedPiece);
            if(foundLine != null)
            {
                return foundLine;
            }
        }
        intersectedPiece = null;
        return foundLine;
    }
    public void CleanUp()
    {
        RemoveLineFromTick();
        pieces.Clear();
        connectingLinesPiece.Clear();
        connectingAssemblyLines.Clear();
        startPiece = null;
        endPiece = null;
    }

////////////////DEBUGGING///////////////////////////
    public void DebugLine()
    {
        string mainLineStr = "Main Line: " + FormatLineString(this);
        Debug.Log(mainLineStr);

        // Debugging connecting lines
        foreach(var kvp in connectingLinesPiece)
        {
            foreach(var connectingLine in kvp.Value)
            {
                Debug.Log(DebugConnectingLine(connectingLine, kvp.Key, 1));
            }
        }
    }

    private string DebugConnectingLine(AssemblyLine line, ITransportable connectingPiece, int depth)
    {
        string connectingLineStr = "Connecting Line at depth " + depth + " at connecting piece: "+ connectingPiece.GetGridCoords() + "\n";
        connectingLineStr += FormatLineString(line);

        foreach (var kvp in line.connectingLinesPiece)
        {
            foreach(var connectingLine in kvp.Value)
            {
                connectingLineStr += "\n";
                connectingLineStr += DebugConnectingLine(connectingLine, kvp.Key, depth + 1);
            }
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
