using System.Collections.Generic;

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
        if(endPiece.GetCell().GetCellCoordinates() + endPiece.Movement() == piece.GetCell().GetCellCoordinates())
        {
            return true;
        }
        return false;
    }
    public bool IsPieceNewStart(AssemblyPiece piece)
    {
        if(startPiece == null)
        {
            return true;
        }
        if(startPiece.GetCell().GetCellCoordinates() == piece.GetCell().GetCellCoordinates() + piece.Movement())
        {
            return true;
        }
        return false;
    }
    public void AddPiece(AssemblyPiece piece)
    {
        bool isEnd = IsPieceNewEnd(piece);
        bool isStart = IsPieceNewStart(piece);
        if(isEnd)
        {
            if(endPiece != null)
            {
                endPiece.NextPiece = piece;
                piece.PreviousPiece = endPiece;
            }
            endPiece = piece;
        }
        if(isStart)
        {
            if(startPiece != null)
            {
                startPiece.PreviousPiece = piece;
                piece.NextPiece = startPiece;
            }
            startPiece = piece;
        }
        if(isEnd || isStart)
        {
            pieces.Add(piece);
        }
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
            if(piece.NextPiece == null)
            {
                endPiece = piece;
            }
            if(piece.PreviousPiece == null)
            {
                startPiece = piece;
            }
        }
    }

}

public enum Direction
{
    Default = 0,
    Forwards = 10,
    Backwards = 20,
    UniDirectional = 30
}
