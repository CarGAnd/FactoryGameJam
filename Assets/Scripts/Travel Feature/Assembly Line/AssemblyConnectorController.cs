using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssemblyConnectorController : MonoBehaviour
{
    public Action DragStarted, DragStopped, DragSuccessful, DragFailure;

    [SerializeField]
    private GameObject bezierLineRendererPrefab;

    private bool isDragging = false;

    private IAssembly startAssembly, endAssembly;
    private Vector3[] positions = new Vector3[4];

    public bool IsDragging { 
        get => isDragging; 
        set {
            if (value && !isDragging) {
                DragStarted?.Invoke();
                // Debug.Log("Drag started.");
            }

            if (!value && isDragging) {
                DragStopped?.Invoke();
                // Debug.Log("Drag ended.");
            }

            isDragging = value;
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetMouseButtonDown(0) ) {
            if (SaveAssembly(ref startAssembly, 0)) {
                IsDragging = true;
            }
        } 
        else if (Input.GetMouseButtonUp(0) && IsDragging) {
            EndDrag();
        }   
    }

    private IAssembly TryGetIAssembly() {
        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit, 100))
		{
            if (hit.transform.gameObject.TryGetComponent(out IAssembly assembly))
                return assembly;
        }

        return null;
    }

    private bool SaveAssembly(ref IAssembly assembly, int index) {
        assembly = TryGetIAssembly();

        if (assembly == null || 
            assembly.IsConnected) {
            DragFailed();
            return false;
        }

        positions[index] = assembly.GetTransformPosition();
        return true;
    }

    private bool AreAssembliesInAndOut()
    {
        return startAssembly.IsOutput != endAssembly.IsOutput;
    }

    private void DragFailed() {
        DragFailure?.Invoke();
        Debug.Log("Drag failed.");
        ResetParameters();
    }

    private void EndDrag() {
        if (!SaveAssembly(ref endAssembly, 3))
            return;

        if (!AreAssembliesInAndOut()) {
            DragFailed();
            return;
        }

        PreparePositionArray();
        CreateAndSetLineRenderer();
        DragSuccessful?.Invoke();
        Debug.Log("Drag succeeded.");
        ResetParameters();
    }

    private void SetAssemblies(BezierLineRenderer bezierLineRenderer)
    {
        startAssembly.Connect(bezierLineRenderer);
        endAssembly.Connect(bezierLineRenderer);
    }

    private void CreateAndSetLineRenderer()
    {
        BezierLineRenderer lineObject = Instantiate(bezierLineRendererPrefab, transform).GetComponent<BezierLineRenderer>();
        lineObject.InitializeBezierCurve(positions, 0.01f, 20);
        SetAssemblies(lineObject);
    }

    private void PreparePositionArray()
    {
        float midX = Mathf.Abs(positions[0].x - positions[3].x) / 2;
        float midY = (positions[0].y - positions[3].y) / 2;

        if (positions[0].x > positions[3].x)
            midX = -midX;

        positions[1] = new Vector3(positions[0].x + midX, midY, positions[0].z);
        positions[2] = new Vector3(positions[3].x - midX, midY, positions[3].z);

        if (endAssembly.IsOutput)
            Array.Reverse(positions);
    }

    private void ResetParameters()
    {
        positions = new Vector3[4];
        startAssembly = null;
        endAssembly = null;
        IsDragging = false;
    }
}
