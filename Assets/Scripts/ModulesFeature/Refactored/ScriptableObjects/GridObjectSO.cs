using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridObjectSO : ScriptableObject
{
    [field: SerializeField] public GameObject ModulePrefab { get; private set; }
    [field: SerializeField] public GameObject PreviewPrefab { get; private set; }

    [SerializeField] private BoolMatrix buildingLayout;
 
    public int Width { get { return buildingLayout.Width; } }
    public int Height { get { return buildingLayout.Height; } }

    public List<Vector2Int> GetLayoutShape(int numRotations) {
        return GetFancyLayoutShape(numRotations);
    }

    public List<Vector2Int> GetNormalLayoutShape(int numRotations) {
        List<Vector2Int> occupiedPositions = new List<Vector2Int>();
        Vector2Int rotatedDimensions = GetLayoutShapeDimensions(numRotations);
        int rotatedWidth = rotatedDimensions.x;
        int rotatedHeight = rotatedDimensions.y;
        for(int x = 0; x < rotatedWidth; x++) {
            for(int y = 0; y < rotatedHeight; y++) {
                occupiedPositions.Add(new Vector2Int(x, y));
            }
        }
        return occupiedPositions;
    }

    public List<Vector2Int> GetFancyLayoutShape(int numRotations) {
        List<Vector2Int> occupiedPositions = new List<Vector2Int>();
        List<Vector2Int> positions = buildingLayout.GetTrueValues();
        foreach (Vector2Int pos in positions) {
            occupiedPositions.Add(GetRotatedPosition(pos, numRotations));
        }
        return occupiedPositions;
    }

    public Vector2Int GetLayoutShapeDimensions(int numRotations) {
        int modRotations = numRotations % 4;
        
        if(modRotations < 0) {
            modRotations += 4;
        }

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

    private Vector2Int GetRotatedPosition(Vector2Int inputVector, int numRotations) {
        int modRotation = 4 - (numRotations % 4);
        
        if(modRotation >= 4) {
            modRotation -= 4;
        }

        switch (modRotation) {
            case 0:
                return inputVector;
            case 1:
                return new Vector2Int(-inputVector.y + Height - 1, inputVector.x);
            case 2:
                return new Vector2Int(-inputVector.x + Width - 1, -inputVector.y + Height - 1);
            case 3:
                return new Vector2Int(inputVector.y, -inputVector.x + Width - 1);
            default:
                return inputVector;
        }
    }

    public virtual IPlacementStrategy GetPlacementHandler() {
        return new ClickPlacer();
    }

    public abstract IGridObject CreateInstance(Vector3 position, Quaternion rotation, int numRotations);
}
