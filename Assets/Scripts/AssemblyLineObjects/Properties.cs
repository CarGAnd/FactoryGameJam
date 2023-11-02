using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Properties : IPropertyComparator
{
    public Color Color;
    public Quaternion Rotation;

    public bool CompareColor(Properties other)
    {
        return Color == other.Color;
    }

    public bool CompareRotation(Properties other)
    {
        return Rotation == other.Rotation;
    }
    // The idea here is only comparing the properties between a reference property and a other property found in the PropertyType List.
    public bool CompareProperties(Properties other, Properties reference, List<PropertyType> propertiesToCompare){
        foreach (var property in propertiesToCompare)
        {
            switch (property)
            {
                case PropertyType.Color:
                    if (!reference.CompareColor(other))
                        return false;
                    break;
                case PropertyType.Rotation:
                    if (!reference.CompareRotation(other))
                        return false;
                    break;
                default:
                    Debug.LogError("Unrecognized PropertyType: " + property);
                    break;

            }
        }
        return true;
    }
}

public enum PropertyType
{
    Color,
    Rotation
}
