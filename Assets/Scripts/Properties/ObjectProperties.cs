using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

[CreateAssetMenu(fileName = "ObjectProperties", menuName = "Properties/ObjectProperties", order = 1)]
public class ObjectProperties : ScriptableObject
{
    [ShowIf("@PropertyDropdownHelper.IsColorAvailable(LevelDataGetter.GetCurrent().LevelProperties)")]
    [ValueDropdown("@PropertyDropdownHelper.GetColorNames(LevelDataGetter.GetCurrent().LevelProperties)")]
    public string colorName;
    
    [ShowIf("@PropertyDropdownHelper.IsRotationAvailable(LevelDataGetter.GetCurrent().LevelProperties)")]
    [ValueDropdown("@PropertyDropdownHelper.GetRotationNames(LevelDataGetter.GetCurrent().LevelProperties)")]
    public string rotationName;
    public Properties CreateProperties()
    {
        return Properties.CreateProperties(LevelDataGetter.GetCurrent().LevelProperties, colorName, rotationName);
    }
}
