using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AssemblyLineCreator
{
    Grid grid;
    private Camera mainCamera;
    LayerMask groundLayer;

    AssemblyLineScriptableObject assemblyLineScriptableObject;
    List<Cell> currentPath = new();

    public AssemblyLineCreator(Grid grid, Camera camera, LayerMask groundLayer, AssemblyLineScriptableObject data)
    {
        this.grid = grid;
        this.mainCamera = camera;
        this.groundLayer = groundLayer;
        this.assemblyLineScriptableObject = data;
    }

    public void StartDrag(Vector3 worldPosition)
    {
        Cell startCell = grid.GetCell(GetWorldPositionFromMousePosition(worldPosition));
        if(startCell == null)
        {
            return;
        }
        else
        {
            currentPath.Clear();
            currentPath.Add(startCell);
            Debug.Log("Started Drag");
        }
    }

    public void UpdateDrag(Vector3 worldPosition)
    {
        Cell currentCell = grid.GetCell(GetWorldPositionFromMousePosition(worldPosition));
        if(currentCell == null)
        {
            return;
        }
        else if(!currentPath.Contains(currentCell))
        {
            currentPath.Add(currentCell);
            Debug.Log("Added another cell to the path. " + currentCell.GetCellCoordinates().ToString());
        }
    }

    public void EndDrag(Vector3 worldPosition)
    {
        Cell endCell = grid.GetCell(GetWorldPositionFromMousePosition(worldPosition));
        if(endCell == null)
        {
            return;
        }
        else if(!currentPath.Contains(endCell))
        {
            currentPath.Add(endCell);
        }
        CreateAssemblyLine(currentPath, assemblyLineScriptableObject);
        currentPath.Clear();
        Debug.Log("Ended Drag");

    }
    private void CreateAssemblyLinePiece(Cell cell, AssemblyLineScriptableObject data)
    {
        GameObject pieceObject = MonoBehaviour.Instantiate(data.prefab, cell.GetCellCenter(), quaternion.identity);
        AssemblyLinePiece pieceComponent = pieceObject.GetComponent<AssemblyLinePiece>();

        if(pieceComponent != null)
        {
            pieceComponent.Initialize(cell, data.movementDistance);
            grid.PlaceObject(pieceComponent, cell);
        }
        else
        {
            MonoBehaviour.Destroy(pieceObject);
        }
    }

    private void CreateAssemblyLine(List<Cell> path, AssemblyLineScriptableObject data)
    {
        foreach(Cell cell in path)
        {
            CreateAssemblyLinePiece(cell, data);
        }
    }

    private Vector3 GetWorldPositionFromMousePosition(Vector3 mousePosition)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            worldPosition = hit.point;
        }

        return worldPosition;
    }
}
