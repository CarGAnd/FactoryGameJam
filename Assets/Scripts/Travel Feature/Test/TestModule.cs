using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(ModuleAssemblyController))]
public class TestModule : MonoBehaviour
{
    [SerializeField]
    private GameObject assemblyLinePrefab;
    private ModuleAssemblyController moduleAssemblyController;
    // Start is called before the first frame update
    void Awake()
    {
        moduleAssemblyController = GetComponent<ModuleAssemblyController>();
        moduleAssemblyController.Initialize();
        moduleAssemblyController.RecievedObject += OnRecievedObject;
    }

    [Button]
    private void CreateAndSendObject() {
        AssemblyObject AssemblyObject = Instantiate(assemblyLinePrefab, transform.position, quaternion.identity, transform).GetComponent<AssemblyObject>();
        SendObject(AssemblyObject.TravelAssemblyLine);
    }

    private void OnRecievedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject)
    {
        //Debug.Log($"Received an Object with {assemblyObject.Value.TestName}.");
        SendObject(assemblyObject);
    }

    private void SendObject(ITravelAssemblyLine<AssemblyObject> AssemblyObject) {
        IAssembly assembly = moduleAssemblyController.GetOutputAssemblies()[0];

        if (!assembly.IsConnected)
            return;

        AssemblyObject.StartTravel(assembly.GetTravelPositions());
    }
}
