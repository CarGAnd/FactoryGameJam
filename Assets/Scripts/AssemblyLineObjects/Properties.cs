using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PropertyType
{
    Color,
    Rotation
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

    public object GetProperty(PropertyType propertyType)
    {
        if (properties.ContainsKey(propertyType))
        {
            return properties[propertyType];
        }
        else
        {
            Debug.LogError("PropertyType: " + propertyType + " not found.");
            return null;
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
            Debug.LogError("PropertyType: " + propertyType + " not found.");
            return default(T);
        }
    }

    //Function to compare a given property to another property.
    public bool CompareProperty(PropertyType propertyType, object value)
    {
        if (properties.ContainsKey(propertyType))
        {
            return properties[propertyType].Equals(value);
        }
        else
        {
            Debug.LogError("PropertyType: " + propertyType + " not found.");
            return false;
        }
    }

    //Function to compare to property classes
    public bool CompareProperties(Properties properties)
    {
        foreach (KeyValuePair<PropertyType, object> property in properties.properties)
        {
            Debug.Log($"Property: {property.Key}, Expected Value: {property.Value}, Actual Value: {this.properties[property.Key]}");
            Debug.Log(CompareProperty(property.Key, property.Value));
            if (!CompareProperty(property.Key, property.Value))
            {
                return false;
            }
        }
        return true;
    }
}


