using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyLineTester : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AssemblyLineScriptableObject assemblyLineScriptableObject;
    private AssemblyLineCreator assemblyLineCreator;
    private bool isDragging = false;
    private void Start()
    {
        assemblyLineCreator = new AssemblyLineCreator(grid, mainCamera, groundLayer, assemblyLineScriptableObject);
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            assemblyLineCreator.StartDrag(Input.mousePosition);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            assemblyLineCreator.EndDrag(Input.mousePosition);
        }
        else if(isDragging)
        assemblyLineCreator.UpdateDrag(Input.mousePosition);
    }
}
