using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using SOS;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

public class ModulesFeatureTests
{
    [UnityTest]
    public IEnumerator PlaceModuleTest()
    {
        //Setup
        SceneManager.LoadScene("ModuleFeatureTests");
        yield return null;
        Vector3 positionToPlaceModule = new(0, 0, 0);
        //Act
        foreach(ModuleTypes moduleType in System.Enum.GetValues(typeof(ModuleTypes)))
        {
            TestHelper.PlaceModuleAtPosition(moduleType, positionToPlaceModule);
            GameObject module = GameObject.FindObjectOfType<ModuleBase>().gameObject;
            //Assertion
            Assert.NotNull(module);
            Assert.That(module.transform.position, Is.EqualTo(positionToPlaceModule).Using(Vector3EqualityComparer.Instance));
            //Cleanup
            Object.Destroy(module);
        }
    }

    [UnityTest]
    public IEnumerator SelectModuleTest()
    {
        //Setup
        SceneManager.LoadScene("ModuleFeatureTests");
        yield return null;
        ModulesManager modulesManager = TestHelper.FindModulesManagerInScene();
        Vector3 modulePosition = new(0, 0, 0);
        TestHelper.PlaceModuleAtPosition(ModuleTypes.TurnModule, modulePosition);
        GameObject module = GameObject.FindObjectOfType<TurnModule>().gameObject;
        Assert.NotNull(module);

        //Act
        Vector2 mousePosition = TestHelper.GetMousePositionFromWorldPosition(modulePosition);
        modulesManager.SelectModule(mousePosition);
        //Assertion

        FieldInfo isModuleSelectedField = typeof(ModulesManager).GetField("isModuleSelected", BindingFlags.NonPublic | BindingFlags.Instance);
        bool isModuleSelected = (bool)isModuleSelectedField.GetValue(modulesManager);
        Assert.IsTrue(isModuleSelected, "isModuleSelected is false");

        //Cleanup

        Object.Destroy(module);

    }

    
}
