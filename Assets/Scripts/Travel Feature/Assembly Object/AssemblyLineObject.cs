using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AssemblyLineObject : MonoBehaviour
{
    [SerializeField]
    private float speed = 5;
    public string TestName = "Testing";
    ITravelAssemblyLine<AssemblyLineObject> travelAssemblyLine;
    public ITravelAssemblyLine<AssemblyLineObject> TravelAssemblyLine { get => travelAssemblyLine; private set => travelAssemblyLine = value; }

    private void OnEnable() {
        InitializeITravelAssmblyLine();
    }

    private void InitializeITravelAssmblyLine()
    {
        if (TravelAssemblyLine != null)
            return;
            
        TravelAssemblyLine = new MoveTowardsTravelMethod<AssemblyLineObject>(gameObject, this, speed);
    }

    // Update is called once per frame 
    void Update()
    {
        if (travelAssemblyLine == null)
            return;

        TravelAssemblyLine.UpdateTravel();
    }
}
