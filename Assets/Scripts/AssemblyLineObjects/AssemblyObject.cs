using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyObject : MonoBehaviour
{
    [SerializeField] private FactoryTracker factoryTracker;
    [SerializeField]
    ObjectProperties objectProperties;
    [SerializeField]
    Renderer lollipopRenderer;
    
    private Properties properties = null;
    public Properties Properties 
    { 
        get => properties; 
        set 
        {
            properties = value; 
            ApplyProperties();
        } 
    }

    public bool IsGhost { get; set; }

    private void Awake(){
        if (objectProperties != null && properties == null)
            Properties = objectProperties.CreateProperties();
        
    }

    public void DestroyStuckObject() {
        if (IsGhost) {
            Destroy(gameObject);
            return;
        }
        factoryTracker.OnStuckObjectDestroyed.Invoke();
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

    public void ApplyProperties()
    {
        //Update rotation
        Quaternion rotation = properties.GetProperty<Quaternion>(PropertyType.Rotation);

        transform.rotation = rotation;


        Color color = properties.GetProperty<Color>(PropertyType.Color);
        
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        lollipopRenderer.GetPropertyBlock(propBlock, 0);

        propBlock.SetColor("_BaseColor", color);

        lollipopRenderer.SetPropertyBlock(propBlock, 0);
    }
    
}
