using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PropertyType
{
    NONE,
    Color,
    Rotation,
}

[System.Serializable]
public class Properties
{
    private Dictionary<PropertyType, object> properties = new Dictionary<PropertyType, object>();
    
    public void SetProperty(PropertyType propertyType, object value)
    {
        if (properties.ContainsKey(propertyType))
        {
            properties[propertyType] = value;
        }
        else
        {
            properties.Add(propertyType, value);
        }
    }

    public T GetProperty<T>(PropertyType propertyType)
    {
        if (properties.ContainsKey(propertyType))
        {
            return (T)properties[propertyType];
        }
        else
        {
            throw new PropertyNotFoundException("PropertyType: " + propertyType + " not found.");
        }
    }

    //Function to compare a given property to another property.
    public bool CompareProperty(PropertyType propertyType, object value)
    {
        if (properties.ContainsKey(propertyType))
        {
            if(value is Quaternion && properties[propertyType] is Quaternion)
            {
                Quaternion quaternion1 = (Quaternion)value;
                Quaternion quaternion2 = (Quaternion)properties[propertyType];
                return Quaternion.Angle(quaternion1, quaternion2) == 0f;                
            }
            return properties[propertyType].Equals(value);
        }
        else
        {
            // This can happen if an object has the property value of NONE
            return false;
        }
    }

    //Function to compare to property classes
    public bool CompareProperties(Properties other)
    {
        if(other == null){
        }
        foreach (KeyValuePair<PropertyType, object> property in other.properties)
        {
            if (!CompareProperty(property.Key, property.Value))
            {
                return false;
            }
        }
        return true;
    }

    public static Properties CreateProperties(LevelProperties levelProperties, string colorName, string rotationName)
    {
        var properties = new Properties();

        if(!string.IsNullOrEmpty(colorName))
        {
            var colorValue = levelProperties.GetPropertyByName(colorName);
            if(colorValue != null)
            {
                properties.SetProperty(PropertyType.Color, colorValue);
            }
        }

        if(!string.IsNullOrEmpty(rotationName))
        {
            var rotationValue = levelProperties.GetPropertyByName(rotationName);
            if(rotationValue != null)
            {
                properties.SetProperty(PropertyType.Rotation, rotationValue);
            }
        }

        return properties;
    }
}


