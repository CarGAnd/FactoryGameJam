using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ModuleSO : GridObjectSO
{
    
    public List<Vector2Int> GetInputPositions() {
        return null;
    }

    public List<Vector2Int> GetOutputPositions() {
        return null;
    }

    public override IGridObject CreateInstance(Vector3 position, Quaternion rotation, int numRotations) {
        GameObject g = Instantiate(ModulePrefab, position, rotation);
        Module module = g.GetComponent<Module>();
        module.SetInitInfo(numRotations);
        return module;
    }
}
