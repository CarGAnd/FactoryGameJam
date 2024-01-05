using System.Collections.Generic;
using UnityEngine;
public class AssemblyLine
{
    private LinkedList<AssemblyPiece> pieces;
    private AssemblyPiece startPiece;
    private AssemblyPiece endPiece;
    private List<AssemblyLine> connectingAssemblyLines;

    private Dictionary<AssemblyPiece, List<AssemblyLine>> connectingLinesPiece;
    public AssemblyLine()
    {
        pieces = new();
        connectingAssemblyLines = new List<AssemblyLine>();
        connectingLinesPiece = new();

        AssemblyLineSystem.Instance.SubscribeToTick(OnTick);
    }
    public AssemblyLine(IEnumerable<AssemblyPiece> assemblyPieces)
    {
        pieces = new LinkedList<AssemblyPiece>(assemblyPieces);
        connectingAssemblyLines = new List<AssemblyLine>();
        connectingLinesPiece = new();

        UpdateStartEndPieces();
        AssemblyLineSystem.Instance.SubscribeToTick(OnTick);
    }
    public List<AssemblyLine> GetAllConnections()
    {
        return connectingAssemblyLines;
    }
    
    //The dictionary logic will probably be useful when working on removal of assemblylines.
    public void AddConnectingAssemblyLine(AssemblyLine line, AssemblyPiece endPiece)
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
    public void AddConnectingAssemblyLine(List<AssemblyLine> lines, AssemblyPiece endPiece)
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

    public void OnTick()
    {
        var node = pieces.Last;
        while(node != null)
        {
            node.Value.OnTick();
            node = node.Previous;
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
        return endPiece.GetGridCoords() + endPiece.Movement() == piece.GetGridCoords();
    }
    public bool IsPieceNewStart(AssemblyPiece piece)
    {
        if(startPiece == null)
        {
            return true;
        }
        return startPiece.GetGridCoords() == piece.GetGridCoords() + piece.Movement();
    }
    public void AddPiece(AssemblyPiece piece)
    {
        bool isEnd = IsPieceNewEnd(piece);
        bool isStart = IsPieceNewStart(piece);
        if(isEnd && isStart)
        {
            if(startPiece != null && endPiece != null)
            {
                startPiece.previousPiece = piece;
                endPiece.nextPiece = piece;
                piece.nextPiece = startPiece;
                piece.previousPiece = endPiece;
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
                    endPiece.nextPiece = piece;
                    piece.previousPiece = endPiece;
                }
                endPiece = piece;
                pieces.AddLast(piece);
            }
            if(isStart)
            {
                if(startPiece != null)
                {
                    startPiece.previousPiece = piece;
                    piece.nextPiece = startPiece;
                }
                startPiece = piece;
                pieces.AddFirst(piece);
            }
        }

        
    }
    public List<AssemblyPiece> AddAllPieces()
    {
        List<AssemblyPiece> allPieces = new List<AssemblyPiece>();
        allPieces.AddRange(pieces);
        return allPieces;
    }

    public void SetConnectingEnd(AssemblyPiece piece)
    {
        if(endPiece != null)
        {
            endPiece.nextPiece = piece;
        }
    }

    
    public void MoveConnectingLines(AssemblyLine newLine)
    {
        foreach(var kvp in connectingLinesPiece)
        {
            newLine.AddConnectingAssemblyLine(kvp.Value, kvp.Key);
        }
    }

    public void ReplaceExistingLine(AssemblyLine currentLine, AssemblyLine newLine, AssemblyPiece piece)
    {
        connectingAssemblyLines.Remove(currentLine);
        connectingAssemblyLines.Add(newLine);
        connectingLinesPiece[piece].Remove(currentLine);
        connectingLinesPiece[piece].Add(newLine);
    }

    public void RemoveConnection(AssemblyLine line)
    {
        connectingAssemblyLines.Remove(line);
    }

    private void UpdateStartEndPieces()
    {
        foreach(AssemblyPiece piece in pieces)
        {
            if(piece.nextPiece == null || !pieces.Contains(piece.nextPiece))
            {
                endPiece = piece;
            }
            if(piece.previousPiece == null || !pieces.Contains(piece.previousPiece))
            {
                startPiece = piece;
            }
        }
    }
    public AssemblyLine GetIntersectedAssemblyLine(Vector2Int coords, out AssemblyPiece intersectedPiece)
    {
        AssemblyLine foundLine = null;
        foreach(AssemblyPiece piece in pieces)
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

    private string DebugConnectingLine(AssemblyLine line, AssemblyPiece connectingPiece, int depth)
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
        foreach (AssemblyPiece piece in line.pieces)
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
