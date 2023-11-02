using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewProperties", menuName = "Properties/Properties Asset", order = 0)]
public class AssetProperties : ScriptableObject
{

    [System.Serializable]
    public class PropertyEntry
    {
        [SerializeField, HideInInspector]
        private PropertyType propertyType;

        public PropertyType PropertyType => propertyType;

        [ShowIf("PropertyType", PropertyType.Color)]
        public Color colorValue;

        [ShowIf("PropertyType", PropertyType.Rotation)]
        public Quaternion rotationValue;

        public PropertyEntry(PropertyType propertyType)
        {
            this.propertyType = propertyType;
        }

        public object GetValue()
        {
            switch (propertyType)
            {
                case PropertyType.Color:
                    return colorValue;
                case PropertyType.Rotation:
                    return rotationValue;
                default:
                    Debug.LogError("Unsupported property type: " + propertyType);
                    return null;
            }
        }
    }

    [ListDrawerSettings(CustomAddFunction = "DisableAddFunction", CustomRemoveIndexFunction = "RemoveAt"),  ]
    public List<PropertyEntry> properties = new List<PropertyEntry>();

    private void DisableAddFunction()
    {
        // Disable the default add functionality
    }
    private void RemoveAt(int index)
    {
        properties.RemoveAt(index);
    }

    public Properties CreateProperties()
    {
        Properties newProperties = new Properties();
        foreach (PropertyEntry entry in properties)
        {
            newProperties.SetProperty(entry.PropertyType, entry.GetValue());
        }
        return newProperties;
    }
    [Button("Add Property")]
    public void AddNewProperty(PropertyType propertyType)
    {
        PropertyEntry newEntry = new PropertyEntry(propertyType);
        properties.Add(newEntry);
        OnValidate();
    }

    private void OnValidate()
    {
        HashSet<PropertyType> propertyTypes = new HashSet<PropertyType>();
        List<PropertyEntry> uniqueEntries = new List<PropertyEntry>();

        for (int i = 0; i < properties.Count; i++)
        {
            PropertyType propertyType = properties[i].PropertyType;

            if (!propertyTypes.Contains(propertyType))
            {
                propertyTypes.Add(propertyType);
                uniqueEntries.Add(properties[i]);
            }
        }
        properties = uniqueEntries;
    }

}
