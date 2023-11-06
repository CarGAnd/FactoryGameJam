using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class TestModule : ModuleBase
{
    [SerializeField]
    private GameObject assemblyLinePrefab;
   
    [Button]
    private void CreateAndSendObject() {
        AssemblyObject AssemblyObject = Instantiate(assemblyLinePrefab, transform.position, quaternion.identity, transform).GetComponent<AssemblyObject>();
        SendObject(AssemblyObject.TravelAssemblyLine);
    }

    protected override void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject)
    {
        Debug.Log($"Object was received at {gameObject.name}.");
        SendObject(assemblyObject);
    }

    public override void SelectModule()
    {
        throw new NotImplementedException();
    }

}

