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
            Debug.LogError("PropertyType: " + propertyType + " not found.");
            return false;
        }
    }

    //Function to compare to property classes
    public bool CompareProperties(Properties properties)
    {
        foreach (KeyValuePair<PropertyType, object> property in properties.properties)
        {
            if (!CompareProperty(property.Key, property.Value))
            {
                return false;
            }
        }
        return true;
    }
}


