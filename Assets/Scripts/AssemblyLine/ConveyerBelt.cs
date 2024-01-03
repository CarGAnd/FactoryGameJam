using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : AssemblyPiece
{
    public ConveyerBelt(AssemblyPieceData data, Cell cell) : base(data, cell)
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
        MonoBehaviour.Instantiate(data.prefab, cell.GetCellCenter(), pieceRotation);
    }

    public override string ToString()
    {
        return "Conveyer Belt at : " + cell.GetCellCoordinates() + " Facing: " + facing;
    }
    //Maybe this class is more specific towards animation properties.
}
