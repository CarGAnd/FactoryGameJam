using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System.Linq;


#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif
//
//
//
// ------------------ DEPRECATED - Use ObjectProperties ------------------
//
//


[CreateAssetMenu(fileName = "NewProperties", menuName = "Properties/Properties Asset", order = 1)]
public class AssetProperties : ScriptableObject
{
    public LevelProperties levelProperties;

    [System.Serializable]
    public class PropertyEntry
    {
        public PropertyType propertyType;
        [ValueDropdown("GetPropertyNames")]
        public string propertyName; // Index for the selected color or rotation
        private AssetProperties assetProperties;
        public PropertyEntry(AssetProperties assetProperties)
        {
            this.assetProperties = assetProperties;
        }
        private IEnumerable<string> GetPropertyNames()
        {
            if(assetProperties == null || assetProperties.levelProperties == null)
                return Enumerable.Empty<string>();
                
            switch(propertyType)
            {
                case PropertyType.Color:
                    return assetProperties.levelProperties.colorProperties.Select(x => x.name);
                case PropertyType.Rotation:
                    return assetProperties.levelProperties.rotationProperties.Select(x => x.name);
                default:
                    return null;
            }
        }
    }
    public List<PropertyEntry> selectedProperties = new List<PropertyEntry>();

}

