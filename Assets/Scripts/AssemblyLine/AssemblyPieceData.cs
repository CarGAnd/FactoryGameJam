using UnityEngine;

[CreateAssetMenu(fileName = "AssemblyPieceData", menuName = "AssemblyPieces/AssemblyPiece", order = 1)]
public class AssemblyPieceData : GridObjectSO
{
    public int movementDistance;
    public int cost;
    public AssemblyPieceType type;
    public Facing Facing { get; private set; }
    public override IGridObject CreateInstance(Vector3 position, Quaternion rotation, int numRotations, AssemblyLineSystem assemblyLineSystem) {
        SetFacing(rotation);
        Instantiate(ModulePrefab, position, rotation);
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
                Facing = Facing.North;
                break;
            case 0:
                Facing = Facing.West;
                break;
            case 270:
                Facing = Facing.South;
                break;
            case 180:
                Facing = Facing.East;
                break;
            default:
                Debug.LogError("Invalid rotation");
                break;
        }
    }
}
