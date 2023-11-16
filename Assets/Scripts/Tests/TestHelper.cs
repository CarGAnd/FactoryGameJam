using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class TestHelper
{
    public static ModulesManager FindModulesManagerInScene()
    {
        ModulesManager modulesManager = GameObject.FindObjectOfType<ModulesManager>();
        if(modulesManager == null)
        {
            Debug.LogError("ModulesManager not found in scene.");
        }
        return modulesManager;
    }
    public static void PlaceModuleAtPosition(ModuleTypes moduleType, Vector3 positionToPlaceModule)
    {
        var modulesManager = GameObject.FindObjectOfType<ModulesManager>();
        modulesManager.ToggleGizmos(true);

        if(modulesManager == null)
        {
            Debug.LogError("ModulesManager not found in scene.");
            return;
        }

        PropertyInfo canPlaceProperty = typeof(ModulesManager).GetProperty("CanPlaceModule", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        MethodInfo setMethod = canPlaceProperty.GetSetMethod(true);
        setMethod.Invoke(modulesManager, new object[] { true });

        modulesManager.PlaceModule(moduleType, positionToPlaceModule);
    }

    public static Vector2 GetMousePositionFromWorldPosition(Vector3 worldPosition)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        Vector2 mousePosition = new Vector2(screenPosition.x, screenPosition.y);
        return mousePosition;
    }

    public static ModulesManager SetupModulesManagerEditMode()
    {
        ModulesManager modulesManager = new GameObject().AddComponent<ModulesManager>();

        MethodInfo awakeMethod = typeof(ModulesManager).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        awakeMethod?.Invoke(modulesManager, null);

        Camera cam = new GameObject().AddComponent<Camera>();
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        FieldInfo rangeIndicatorField = typeof(ModulesManager).GetField("rangeIndicatorPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
        rangeIndicatorField.SetValue(modulesManager, sphere);

        return modulesManager;
    }

    public static TurnModule SetupTurnModuleWithRotation(bool isClockwise)
    {
        GameObject module = new();
        TurnModule turnModule = module.AddComponent<TurnModule>();
        FieldInfo rotationToApply = typeof(TurnModule).GetField("rotationToApply", BindingFlags.NonPublic | BindingFlags.Instance);
        Vector3 rotationSet;
        
        if(isClockwise)
        {
            rotationSet = new Vector3(0, 45, 0);
        }
        else
        {
            rotationSet = new Vector3(0, -45, 0);
        }
        
        rotationToApply.SetValue(turnModule, rotationSet);
        return turnModule;
    }

    public static AssemblyObject CreateAssemblyObjectWithProperties(Vector3 rotation, Color color)
    {
        GameObject testObject = new();
        AssemblyObject assemblyTest = testObject.AddComponent<AssemblyObject>();
        assemblyTest.Properties = new Properties();
        assemblyTest.Properties.SetProperty(PropertyType.Rotation, Quaternion.Euler(rotation));
        assemblyTest.Properties.SetProperty(PropertyType.Color, color);
        return assemblyTest;
    }
}
