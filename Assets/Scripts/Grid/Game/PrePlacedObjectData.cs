using UnityEngine;


[System.Serializable]
public class PrePlacedObjectData : MonoBehaviour {
    public FactoryGrid grid;
    public Facing facing;
    public GridObjectSO objectDefinition;
    public Vector2Int gridPosition;

    private void OnDestroy() {
        GridInitializer gridInitializer = FindObjectOfType<GridInitializer>();
        if(gridInitializer != null) {
            gridInitializer.RemoveObject(this);
        }
    }

}

