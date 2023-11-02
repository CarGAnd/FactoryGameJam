using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyObject : MonoBehaviour
{
    [SerializeField]
    private float speed = 5;
    [SerializeField]
    Properties properties;
    public Properties Properties 
    { 
        get => properties; 
        private set 
        {
            properties = value; 
            ApplyProperties(); 
        } 
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
    private void OnValidate(){
        ApplyProperties();
    }

    private void ApplyProperties()
    {
        //Update rotation
        transform.rotation = Properties.Rotation;
        //Update color
        Renderer renderer = GetComponent<Renderer>();
        if(renderer != null)
        {
            renderer.sharedMaterial.color = Properties.Color;
        }
    }
}
