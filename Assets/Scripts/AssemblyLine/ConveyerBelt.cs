using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : AssemblyPiece
{
    public ConveyerBelt(AssemblyPieceData data, Vector2Int cellCoords, Grid grid) : base(data, cellCoords, grid)
    {
        Quaternion pieceRotation = Quaternion.identity;
        switch(data.facing)
        {
            case Facing.North:
                pieceRotation = Quaternion.Euler(0, 90, 0);
                break;
            case Facing.East:
                pieceRotation = Quaternion.Euler(0, 180, 0);
                break;
            case Facing.South:
                pieceRotation = Quaternion.Euler(0, 270, 0);
                break;
            case Facing.West:
                pieceRotation = Quaternion.Euler(0, 0, 0);
                break;
        }
        GameObject newConveyerBelt = MonoBehaviour.Instantiate(data.prefab, grid.GetCellCenter(cellCoords), pieceRotation);
        
    }

    public override string ToString()
    {
        return "Conveyer Belt at : " + cellCoords + " Facing: " + facing;
    }
    //Maybe this class is more specific towards animation properties.
}
