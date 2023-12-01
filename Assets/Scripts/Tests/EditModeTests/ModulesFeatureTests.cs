using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using TMPro;
using UnityEngine.TestTools;
using NUnit.Framework.Internal;


public class ModulePlacementTests
{    
    
    [Test]
    public void TurnModuleTest()
    {
        //Setup
        TurnModule turnModule = TestHelper.SetupTurnModuleWithRotation(true);
        AssemblyObject assemblyTest = TestHelper.CreateAssemblyObjectWithProperties(new Vector3(0,0,0), Color.red);

        //Act
        MethodInfo rotateMethod = typeof(TurnModule).GetMethod("RotateProperty", BindingFlags.NonPublic | BindingFlags.Instance);
        rotateMethod?.Invoke(turnModule, new object[] { assemblyTest });

        //Assertion
        Quaternion expectedQuaternion = Quaternion.Euler(0, 45, 0);
        Quaternion actualQuaternion = assemblyTest.Properties.GetProperty<Quaternion>(PropertyType.Rotation);
        Assert.AreEqual(Quaternion.Angle(expectedQuaternion, actualQuaternion), 0);

        //Cleanup
        Object.DestroyImmediate(turnModule.gameObject);
        Object.DestroyImmediate(assemblyTest.gameObject);
    }

    

}
