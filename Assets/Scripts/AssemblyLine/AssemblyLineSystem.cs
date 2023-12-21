using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AssemblyLineSystem : MonoBehaviour
{
    public static AssemblyLineSystem Instance;
    public UnityEvent Tick;
    [SerializeField] private Grid grid;

    private List<AssemblyLine> assemblyLines;

    private void Awake()
    {
        Instance = this;
        assemblyLines = new List<AssemblyLine>();
    }

    public void PlaceAssemblyPiece()
    {
        /*
        if(grid.PlaceObject())
        {

        }
        */
    }

    private void CombineAssemblyLines(AssemblyLine line1, AssemblyLine line2)
    {

    }

    public void SubscribeToTick(UnityAction action)
    {
        Tick.AddListener(action);
    }

    public void UnsubscribeFromTick(UnityAction action)
    {
        Tick.RemoveListener(action);
    }
}
