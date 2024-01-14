using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ModuleInputOutput))]
public abstract class Module : MonoBehaviour
{
    [SerializeField] private GridObjectSO moduleData;

    private ModuleInputOutput inputOutput;

    private void Start() {
        inputOutput = GetComponent<ModuleInputOutput>();
    }

    public void DestroyModule() {
        inputOutput.Destroy();
        Destroy(gameObject);
    }
}
