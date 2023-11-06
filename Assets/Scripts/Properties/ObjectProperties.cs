using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

[CreateAssetMenu(fileName = "ObjectProperties", menuName = "Properties/ObjectProperties", order = 1)]
public class ObjectProperties : ScriptableObject
{
    public LevelProperties levelProperties;

    [ShowIf("@PropertyDropdownHelper.IsColorAvailable(levelProperties)")]
    [ValueDropdown("@PropertyDropdownHelper.GetColorNames(levelProperties)")]
    public string colorName;
    
    [ShowIf("@PropertyDropdownHelper.IsRotationAvailable(levelProperties)")]
    [ValueDropdown("@PropertyDropdownHelper.GetRotationNames(levelProperties)")]
    public string rotationName;
    public Properties CreateProperties()
    {
        return Properties.CreateProperties(levelProperties, colorName, rotationName);
    }
}
