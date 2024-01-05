using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridObjectSO : ScriptableObject
{
    [field: SerializeField] public int Width { get; private set; }
    [field: SerializeField] public int Height { get; private set; }
    [field: SerializeField] public GameObject ModulePrefab { get; private set; }
    [field: SerializeField] public GameObject PreviewPrefab { get; private set; }

    public List<Vector2Int> GetLayoutShape(int numRotations) {
        Vector2Int rotatedDimensions = GetLayoutShapeDimensions(numRotations);
        int rotatedWidth = rotatedDimensions.x;
        int rotatedHeight = rotatedDimensions.y;
        List<Vector2Int> occupiedPositions = new List<Vector2Int>();
        for(int x = 0; x < rotatedWidth; x++) {
            for(int y = 0; y < rotatedHeight; y++) {
                occupiedPositions.Add(new Vector2Int(x, y));
            }
        }
        return occupiedPositions;
    }

    public Vector2Int GetLayoutShapeDimensions(int numRotations) {
        int modRotations = numRotations % 4;
        switch (modRotations) {
            case 0:
            case 2:
                return new Vector2Int(Width, Height);
            case 1:
            case 3:
                return new Vector2Int(Height, Width);
            default:
                return new Vector2Int(Width, Height);
        }
    }

    public abstract IGridObject CreateInstance(Vector3 position, Quaternion rotation, int numRotations);
}
