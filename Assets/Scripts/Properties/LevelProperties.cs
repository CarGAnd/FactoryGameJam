using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelProperties", menuName = "Properties/LevelProperties", order = 0)]
public class LevelProperties : ScriptableObject
{
    [System.Serializable]
    public class ColorProperty : IProperty
    {
        public string name; // for editor Identification
        public Color color;

        public string Name => name;

        public object GetValue()
        {
            return color;
        }
    }
    [System.Serializable]
    public class RotationProperty : IProperty
    {
        public string name; // for editor Identification
        public Vector3 rotation;

        public string Name { get => name; private set => name = value;}

        public object GetValue()
        {
            return Quaternion.Euler(rotation);
        }
    }
    public List<ColorProperty> colorProperties;
    public List<RotationProperty> rotationProperties;

    // This list is maintained for runtime use
    [HideInInspector]
    public List<IProperty> properties = new List<IProperty>();

    private void OnValidate()
    {
        properties.Clear(); // Clear the list to avoid duplicates
        
        // Add all ColorProperty instances to the properties list
        foreach (var colorProperty in colorProperties)
        {
            properties.Add(colorProperty);
        }
        
        // Add all RotationProperty instances to the properties list
        foreach (var rotationProperty in rotationProperties)
        {
            properties.Add(rotationProperty);
        }
    }

    
    public object GetPropertyByName(string name)
    {
        foreach (IProperty property in properties)
        {
            //We don't care about case sensitivity
            if (String.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return property.GetValue();
            }
        }
        return null;
    }  
}
