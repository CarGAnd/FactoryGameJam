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
        SceneManager.LoadScene("ModuleFeatureTests");
        yield return null;
        var modulesManager = GameObject.FindObjectOfType<ModulesManager>();
        var moduleType = ModuleTypes.TurnModule;
        modulesManager.ToggleGizmos(true);

        Assert.NotNull(modulesManager);

        yield return null;

        var camera = Camera.main;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        FieldInfo groundLayerField = typeof(ModulesManager).GetField("groundLayer", BindingFlags.NonPublic | BindingFlags.Instance);
        LayerMask groundLayer = (LayerMask)groundLayerField.GetValue(modulesManager);

        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer);
        

        PropertyInfo canPlaceProperty = typeof(ModulesManager).GetProperty("CanPlaceModule", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        MethodInfo setMethod = canPlaceProperty.GetSetMethod(true);
        setMethod.Invoke(modulesManager, new object[] { true });

        var canPlaceModule = (bool)canPlaceProperty.GetValue(modulesManager);

        Assert.IsTrue(canPlaceModule, "CanPlaceModule is false");

        modulesManager.PlaceModule(moduleType);

        GameObject module = GameObject.FindObjectOfType<TurnModule>().gameObject;

        //Assertion
        Assert.NotNull(module);

        //Vector3 and other floating point values might need to be compared with a tolerance.
        Assert.That(module.transform.position, Is.EqualTo(hit.point).Using(Vector3EqualityComparer.Instance));

        //Cleanup
        
        Object.DestroyImmediate(module);
    }

    [UnityTest]
    public IEnumerator SelectModuleTest()
    {
        SceneManager.LoadScene("ModuleFeatureTests");
        yield return null;
        var modulesManager = GameObject.FindObjectOfType<ModulesManager>();
        var moduleType = ModuleTypes.TurnModule;
        modulesManager.ToggleGizmos(true);

        Assert.NotNull(modulesManager);

        yield return null;

        PropertyInfo canPlaceProperty = typeof(ModulesManager).GetProperty("CanPlaceModule", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        MethodInfo setMethod = canPlaceProperty.GetSetMethod(true);
        setMethod.Invoke(modulesManager, new object[] { true });

        var canPlaceModule = (bool)canPlaceProperty.GetValue(modulesManager);

        Assert.IsTrue(canPlaceModule, "CanPlaceModule is false");

        modulesManager.PlaceModule(moduleType);

        GameObject module = GameObject.FindObjectOfType<TurnModule>().gameObject;

        Assert.NotNull(module);

        modulesManager.SelectModule();

        //Assertion

        FieldInfo isModuleSelectedField = typeof(ModulesManager).GetField("isModuleSelected", BindingFlags.NonPublic | BindingFlags.Instance);
        bool isModuleSelected = (bool)isModuleSelectedField.GetValue(modulesManager);

        Assert.IsTrue(isModuleSelected, "isModuleSelected is false");

    }
}
