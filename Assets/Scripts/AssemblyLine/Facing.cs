using UnityEngine;

public enum Facing
{
    North = 0,
    East = 10,
    South = 20,
    West = 30
}

public static class FacingExtentions {

    private static Facing[] facingMap = new Facing[] { Facing.North, Facing.East, Facing.South, Facing.West };

    public static Vector2Int GetIntDirection(this Facing facing) {
        switch (facing) {
            case Facing.North:
                return new Vector2Int(0, 1);
            case Facing.East:
                return new Vector2Int(1, 0);
            case Facing.South:
                return new Vector2Int(0, -1);
            case Facing.West:
                return new Vector2Int(-1, 0);
            default:
                return new Vector2Int(0, 1);
        }
    }

    public static Facing RotatedDirection(this Facing inputDirection, int numRotations) {
        int startNumRotations = 0;
        switch (inputDirection) {
            case Facing.North:
                startNumRotations = 0;
                break;
            case Facing.East:
                startNumRotations = 1;
                break;
            case Facing.South:
                startNumRotations = 2;
                break;
            case Facing.West:
                startNumRotations = 3;
                break;
        }
        int newNumRotations = (startNumRotations + numRotations) % 4;
        if(newNumRotations < 0) {
            newNumRotations += 4;
        }

        return facingMap[newNumRotations];
    }
}
