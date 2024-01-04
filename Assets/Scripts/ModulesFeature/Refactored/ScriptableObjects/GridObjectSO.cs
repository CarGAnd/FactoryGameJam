using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GridObjectSO : ScriptableObject
{
    [field: SerializeField] public int width { get; private set; }
    [field: SerializeField] public int height { get; private set; }
    [field: SerializeField] public GameObject modulePrefab { get; private set; }

    public List<Vector2Int> GetLayoutShape() {
        List<Vector2Int> occupiedPositions = new List<Vector2Int>();
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                occupiedPositions.Add(new Vector2Int(x, y));
            }
        }
        return occupiedPositions;
    }

    public Vector2Int GetLayoutShapeDimensions() {
        return new Vector2Int(width, height);
    }


}
