using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ModuleSO : GridObjectSO
{
    public override IGridObject CreateInstance(Vector3 position, Quaternion rotation, int numRotations) {
        GameObject g = Instantiate(ModulePrefab, position, rotation);
        Module module = g.GetComponent<Module>();
        module.SetInitInfo(numRotations);
        IGridObject gridObject = g.GetComponent<IGridObject>();
        return gridObject;
    }
}
