using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using System.Linq;

public class ElseGateModule : ModuleBase
{
    [SerializeField]
    private PropertyType propertyToCompare;

    [ShowIf("propertyToCompare", PropertyType.Color)]
    [SerializeField]
    private Color colorToCompare;

    [ShowIf("propertyToCompare", PropertyType.Rotation)]
    [SerializeField]
    private Vector3 rotationToCompare;

    private Quaternion rotationToCompareQuat;

    [Header("UI")]
    [SerializeField]
    private GameObject UI;

    [SerializeField]
    private TMP_Dropdown propertyTypeDropdown;
    [SerializeField]
    private TMP_Dropdown propertyValueDropdown;

    [SerializeField]
    private TMP_Text comparisonText;

    void Start(){
        if(propertyToCompare == PropertyType.Rotation){
            rotationToCompareQuat = Quaternion.Euler(rotationToCompare);
        }
        
    }
    protected override void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject)
    {
        switch(propertyToCompare)
        {
            case PropertyType.NONE:
                SendObject(assemblyObject);
                break;
            case PropertyType.Color:
                if(CompareProperty(assemblyObject.Value.Properties, colorToCompare))
                {
                    SendObject(assemblyObject);
                }
                else
                {
                    SendObject(assemblyObject, 1);
                }
                break;
            case PropertyType.Rotation:
                if(CompareProperty(assemblyObject.Value.Properties, rotationToCompareQuat))
                {
                    SendObject(assemblyObject);
                }
                else
                {
                    SendObject(assemblyObject, 1);
                }
                break;
            default:
                Debug.LogError("Unsupported property type: " + propertyToCompare);
                return;
        }
    }
    public void SetPropertyToCompare(PropertyType propertyType)
    {
        propertyToCompare = propertyType;
    }
    
    public void SetColorToCompare(Color color)
    {
        colorToCompare = color;
    }

    public void SetRotationToCompare(Vector3 rotation)
    {
        rotationToCompareQuat = Quaternion.Euler(rotation);
    }

    private bool CompareProperty(Properties property, object value)
    {
        return property.CompareProperty(propertyToCompare, value);
    }

    void OnDisable(){
        UI.SetActive(false);
    }

    public override void SelectModule()
    {
        UI.SetActive(true);
        PopulateDropdowns();
    }
    
    public void ApplySettings()
    {
        switch(propertyTypeDropdown.value)
        {
            case 0:
                propertyToCompare = PropertyType.Color;
                break;
            case 1:
                propertyToCompare = PropertyType.Rotation;
                break;
            default:
                Debug.LogError("Unsupported property type: " + propertyTypeDropdown.value);
                return;
        }

        switch(propertyToCompare)
        {
            case PropertyType.Color:
                string colorName = propertyValueDropdown.options[propertyValueDropdown.value].text;
                
                if(colorName == "NONE"){
                    propertyToCompare = PropertyType.NONE;
                    break;
                }
                comparisonText.text = colorName;
                Color colorToCompare = (Color) LevelPropertiesHolder.Instance.Properties.GetPropertyByName(colorName);
                SetColorToCompare(colorToCompare);
                break;
            case PropertyType.Rotation:
                string rotationName = propertyValueDropdown.options[propertyValueDropdown.value].text;
                
                if(rotationName == "NONE"){
                    propertyToCompare = PropertyType.NONE;
                    break;
                }
                comparisonText.text = rotationName;
                Quaternion rotationToCompareQuat = (Quaternion) LevelPropertiesHolder.Instance.Properties.GetPropertyByName(rotationName);
                rotationToCompare = rotationToCompareQuat.eulerAngles;
                SetRotationToCompare(rotationToCompare);
                break;
            default:
                Debug.LogError("Unsupported property type: " + propertyToCompare);
                return;
        }
        UI.SetActive(false);
        ModulesManager.Instance.DeselectModule();
    }

    public void PopulateDropdowns()
    {
        LevelProperties levelProperties = LevelPropertiesHolder.Instance.Properties;

        propertyTypeDropdown.ClearOptions();
        propertyTypeDropdown.AddOptions(new List<string> { "Color", "Rotation" });

        propertyTypeDropdown.onValueChanged.AddListener(delegate {
            UpdatePropertyValueDropdown(levelProperties);
        });

        UpdatePropertyValueDropdown(levelProperties);
    }

    private void UpdatePropertyValueDropdown(LevelProperties levelProperties)
    {
        propertyValueDropdown.ClearOptions();

        if (propertyTypeDropdown.value == 0) // Assuming 'Color' is the first option
        {
            propertyValueDropdown.AddOptions(PropertyDropdownHelper.GetColorNames(levelProperties).ToList());
        }
        else // Assuming 'Rotation' is the second option
        {
            propertyValueDropdown.AddOptions(PropertyDropdownHelper.GetRotationNames(levelProperties).ToList());
        }
    }
}
