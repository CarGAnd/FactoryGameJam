using UnityEngine;

[CreateAssetMenu(fileName = "AssemblyPieceData", menuName = "AssemblyPieces/AssemblyPiece", order = 1)]
public class AssemblyPieceData : GridObjectSO
{
    public int movementDistance;
    public int cost;
    public AssemblyPieceType type;
    public Facing ObjectFacing { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public override IGridObject CreateInstance(Vector3 position, Quaternion rotation, int numRotations, AssemblyLineSystem assemblyLineSystem) {
        SetFacing(rotation);
        Position = position;
        Rotation = rotation;
        
        AssemblyPiece assemblyPiece = assemblyLineSystem.CreateAssemblyPiece(this);
        return assemblyPiece;
    }

    public override IPlacementStrategy GetPlacementHandler() {
        return new ClickAndDragPlacer();
    }

    private void SetFacing(Quaternion rotation) {
        float yRotation = rotation.eulerAngles.y;
        switch(yRotation)
        {
            case 90:
                ObjectFacing = Facing.North;
                break;
            case 0:
                ObjectFacing = Facing.West;
                break;
            case 270:
                ObjectFacing = Facing.South;
                break;
            case 180:
                ObjectFacing = Facing.East;
                break;
            default:
                Debug.LogError("Invalid rotation");
                break;
        }
    }
}
