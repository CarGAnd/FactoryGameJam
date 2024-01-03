using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ModuleSO : ScriptableObject
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private GameObject modulePrefab;

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
