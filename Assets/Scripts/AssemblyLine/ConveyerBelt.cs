using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : AssemblyPiece
{
    public ConveyerBelt(AssemblyPieceData data, Cell cell) : base(data, cell)
    {
        
    }

    public override Vector2Int Movement()
    {
        throw new System.NotImplementedException();
    }

}
