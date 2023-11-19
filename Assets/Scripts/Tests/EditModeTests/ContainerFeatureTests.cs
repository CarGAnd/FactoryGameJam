using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System.Reflection;
using System;
using UnityEngine.TestTools;
using SOS;

public class ContainerFeatureTests
{
    [Test]
    public void ContainerDoesNotCountGhosts() {
        ContainerModule cModule = CreateContainerObject();

        AssemblyObject assemblyTest = TestHelper.CreateAssemblyObjectWithProperties(new Vector3(0, 0, 0), Color.red);
        assemblyTest.IsGhost = true;

        MethodInfo collectMethod = typeof(ContainerModule).GetMethod("CollectObject", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.True(cModule.NumItemsCollected == 0);
        collectMethod?.Invoke(cModule, new object[] { assemblyTest });
        Assert.True(cModule.NumItemsCollected == 0);
    }

    [Test]
    public void ContainerCountsCorrectItems() {
        Vector3 rot = new Vector3(0, 45, 0);
        Color col = Color.red;

        ContainerModule cModule = TestHelper.CreateContainerModuleWithProperties(rot, col);

        AssemblyObject assemblyTest = TestHelper.CreateAssemblyObjectWithProperties(rot, col);
        MethodInfo collectMethod = typeof(ContainerModule).GetMethod("CollectObject", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.True(cModule.NumCorrectItemsCollected == 0);
        collectMethod.Invoke(cModule, new object[] { assemblyTest });
        Assert.True(cModule.NumCorrectItemsCollected == 1);
    }

    [Test]
    public void ContainerDoesNotCountWrongItems() {
        Vector3 rot = new Vector3(0, 45, 0);
        Color col = Color.red;

        ContainerModule cModule = TestHelper.CreateContainerModuleWithProperties(rot, col);

        AssemblyObject assemblyTest = TestHelper.CreateAssemblyObjectWithProperties(new Vector3(0,0,0), Color.blue);
        MethodInfo collectMethod = typeof(ContainerModule).GetMethod("CollectObject", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.True(cModule.NumWrongItemsCollected == 0);
        collectMethod.Invoke(cModule, new object[] { assemblyTest });
        Assert.True(cModule.NumWrongItemsCollected == 1);
    }

    [Test]
    public void ContainerInvokesOnItemCollected() {
        Vector3 rot = new Vector3(0, 45, 0);
        Color col = Color.red;

        ContainerModule cModule = TestHelper.CreateContainerModuleWithProperties(rot, col);

        AssemblyObject assemblyTest = TestHelper.CreateAssemblyObjectWithProperties(rot, col);
        MethodInfo collectMethod = typeof(ContainerModule).GetMethod("CollectObject", BindingFlags.NonPublic | BindingFlags.Instance);
        bool eventWasInvoked = false;
        cModule.OnItemCollected += () => eventWasInvoked = true;

        Assert.True(cModule.NumItemsCollected == 0);
        collectMethod.Invoke(cModule, new object[] { assemblyTest });
        Assert.True(cModule.NumItemsCollected == 1);
        Assert.True(eventWasInvoked);
    }

    private ContainerModule CreateContainerObject() {
        GameObject g = new GameObject();
        ContainerModule module = g.AddComponent<ContainerModule>();
        return module;
    }
}
