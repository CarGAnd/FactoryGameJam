using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadMillAssemblyPiece : AssemblyLinePiece
{
    public override void ConnectToNeighbor(AssemblyLinePiece neighbor)
    {
        Direction = neighbor.Direction;
    }

    public override void MoveObject()
    {
        
    }
}
