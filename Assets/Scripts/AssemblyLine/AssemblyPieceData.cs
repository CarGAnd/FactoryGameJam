using UnityEngine;

[CreateAssetMenu(fileName = "AssemblyPieceData", menuName = "AssemblyPieces/AssemblyPiece", order = 1)]
public class AssemblyPieceData : GridObjectSO
{
    [Tooltip("The prefab that is specific to this piece.")]
    public GameObject prefab;
    public int movementDistance;
    public int cost;
    public AssemblyPieceType type;
    public Facing facing;

    public override IGridObject CreateInstance(Vector3 position, Quaternion rotation, int numRotations) {
        throw new System.NotImplementedException();
    }

    public void SetFacing(Facing facing)
    {
        this.facing = facing;
    }
}
