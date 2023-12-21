using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyLine
{
    List<AssemblyPiece> pieces;
    List<AssemblyPiece> startPieces;
    List<AssemblyPiece> endPieces;

    public AssemblyLine()
    {
        pieces = new List<AssemblyPiece>();
        startPieces = new List<AssemblyPiece>();
        endPieces = new List<AssemblyPiece>();

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
        return null;
    }
    
    public void AddPiece(AssemblyPiece piece)
    {
        pieces.Add(piece);
    }

    public void OnTick()
    {
        for(int i = pieces.Count-1; i >= 0; i--)
        {
            pieces[i].OnTick();
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
