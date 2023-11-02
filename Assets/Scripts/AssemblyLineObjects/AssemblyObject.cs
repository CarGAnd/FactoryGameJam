using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyObject : MonoBehaviour
{
    [SerializeField]
    private float speed = 5;
    [SerializeField]
    AssetProperties assetProperties;
    
    Properties properties = null;
    public Properties Properties 
    { 
        get => properties; 
        private set 
        {
            properties = value; 
            //ApplyProperties();
        } 
    }
    private void Awake(){
        if(assetProperties != null)
            Properties = assetProperties.CreateProperties();
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
            
        TravelAssemblyLine = new MoveTowardsTravelMethod<AssemblyObject>(gameObject, this, speed);
    }
 
    void Update()
    {
        if (travelAssemblyLine == null)
            return;

        TravelAssemblyLine.UpdateTravel();
    }

    private void ApplyProperties()
    {
        //Update rotation
        Quaternion rotation = properties.GetProperty<Quaternion>(PropertyType.Rotation);

        if(rotation != null)
        {
            transform.rotation = rotation;
        }
        //Update color
        Color color = properties.GetProperty<Color>(PropertyType.Color);
        if(color != null)
        {
            Renderer renderer = GetComponent<Renderer>();
            if(renderer != null)
            {
                renderer.sharedMaterial.color = color;
            }
        }
        
    }
    
}
