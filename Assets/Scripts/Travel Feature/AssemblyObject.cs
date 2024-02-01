using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AssemblyObject : MonoBehaviour
{
    [SerializeField] private UnityEvent<bool> OnMovingOnAssemblyLine = new UnityEvent<bool>();
    [SerializeField] private Renderer lollipopRenderer;

    public bool IsGhost { get; set; }

    //Call an event before the object is destroyed
    public void DestroyObject() {
        TravelAssemblyLine.OnTraveling.RemoveListener(OnTravelingInvokeOnMovingOnAssemblyLine);
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
        TravelAssemblyLine.OnTraveling.AddListener(OnTravelingInvokeOnMovingOnAssemblyLine);
    }

    private void OnTravelingInvokeOnMovingOnAssemblyLine(bool isMoving)
    {
        OnMovingOnAssemblyLine?.Invoke(isMoving);
    }

    void Update()
    {
        if (travelAssemblyLine == null)
            return;

        TravelAssemblyLine.UpdateTravel();
    }
    
}
