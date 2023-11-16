using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyObject : MonoBehaviour
{
    [SerializeField] private FactoryTracker factoryTracker;
    [SerializeField] private ObjectProperties objectProperties;
    [SerializeField] private Renderer lollipopRenderer;
    
    private Properties properties = null;
    public Properties Properties 
    { 
        get => properties; 
        set 
        {
            properties = value; 
            ApplyAllProperties();
        } 
    }

    public bool IsGhost { get; set; }

    private void Awake(){
        if (objectProperties != null && properties == null)
            Properties = objectProperties.CreateProperties();
        
    }

    //Call an event before the object is destroyed
    public void DestroyObject() {
        factoryTracker.OnObjectDestroyed?.Invoke(gameObject);
        Destroy(gameObject);
    }

    ITravelAssemblyLine<AssemblyObject> travelAssemblyLine;
    public ITravelAssemblyLine<AssemblyObject> TravelAssemblyLine { get => travelAssemblyLine; private set => travelAssemblyLine = value; }

    private void OnEnable() {
        InitializeITravelAssmblyLine();
    }

    private void InitializeITravelAssmblyLine()
    {
        if (TravelAssemblyLine != null)
            return;
            
        TravelAssemblyLine = new ObjectTravelAssemblyLine<AssemblyObject>(this);
    }
 
    void Update()
    {
        if (travelAssemblyLine == null)
            return;

        TravelAssemblyLine.UpdateTravel();
    }

    public void ApplyAllProperties()
    {
        foreach(PropertyType propertyType in System.Enum.GetValues(typeof(PropertyType)))
        {
            ApplyProperty(propertyType);
        }
    }

    public void ApplyProperty(PropertyType propertyType)
    {
        switch (propertyType)
        {
            case PropertyType.Rotation:
                transform.rotation = properties.GetProperty<Quaternion>(PropertyType.Rotation);
                break;
            case PropertyType.Color:
                if(lollipopRenderer == null)
                    return;
                Color color = properties.GetProperty<Color>(PropertyType.Color);
                MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
                lollipopRenderer.GetPropertyBlock(propBlock, 0);

                propBlock.SetColor("_BaseColor", color);

                lollipopRenderer.SetPropertyBlock(propBlock, 0);
                break;
        }
    }
    
}
