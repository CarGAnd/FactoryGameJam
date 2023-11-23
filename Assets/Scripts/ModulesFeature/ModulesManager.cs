using SOS;
using UnityEngine;
/*
    ------------------- Review -------------------
    Revivewed by: CarGAnd 14/11/2023 [Accepted]

    ----------------- ModulesManager -----------------
    This class takes care of the placement of modules. It is responsible for:
    - Checking if a module can be placed at a given location
    - Placing a module at a given location
    - Selecting a module
    - Deselecting a module
    - Showing a range indicator for the module
    - Showing a transparent version of the module at the location it will be placed
    - Showing a line from the camera to the location the module will be placed

    Functionality should be updated to migrate selection functionality to a SelectionManager.
    The SelectionManager will mostly only provide locations and information about the selected module.
*/
public class ModulesManager : MonoBehaviour
{
    [SerializeField] private GameEvent modulePlacedEvent;
    [SerializeField] private GameEvent moduleSelectedEvent;
    [SerializeField] private LevelStateRef currentLevelStateRef;
    [SerializeField] private GameObject turnModulePrefab;
    [SerializeField] private GameObject elseGateModulePrefab;
    [SerializeField] private GameObject mergeModulePrefab;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject rangeIndicatorPrefab;
    [SerializeField] private float checkRadius = 0.65f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask moduleLayer;
    [SerializeField] private Grid<GameObject> buildGrid;
    [SerializeField] private GameObject visualGridObject;
    private bool isModuleSelected = false;
    public static ModulesManager Instance { get; private set; }
    public bool ShowGizmos {get; private set;}
    public Vector3 LastHitPoint { get; private set; }
    public bool CanPlaceModule { get; private set; }

    private GameObject rangeIndicatorObject;
    private Material gridMaterial;
    
    
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        cam = Camera.main;
        //This should be removed
        if(rangeIndicatorPrefab != null){
            SetRangeIndicatorPrefab(rangeIndicatorPrefab);
        }
        buildGrid = new Grid<GameObject>(buildGrid.Width, buildGrid.Height, buildGrid.Origin, buildGrid.CellSize);
    }
    private void SetRangeIndicatorPrefab(GameObject prefab)
    {
        rangeIndicatorObject = Instantiate(prefab);
        rangeIndicatorObject.SetActive(false);
    }
    void Update()
    {
        // If the player is in build mode, update the placement info
        if (ShowGizmos)
        {
            UpdatePlacementInfo();
            UpdateSpherePosition();
        }
    }
    
    // Toggle gizmos on and off Accessible via player, should be moved to a SelectionManager
    public void ToggleGizmos(bool show){
        if(currentLevelStateRef.Value != LevelState.BuildPhase)
        {
            return;
        }
        ShowGizmos = show;
        rangeIndicatorObject.SetActive(show);
        visualGridObject.SetActive(show);
    }
    
    // If possible, place a module at the mouse Position.
    public void PlaceModule(ModuleTypes moduleType, Vector3 position)
    {
        GameObject modulePrefab = GetModulePrefab(moduleType);
        if (modulePrefab != null && CanPlaceModule && currentLevelStateRef.Value == LevelState.BuildPhase)
        {
            Vector2Int gridPos = buildGrid.WorldToGrid(position);
            Vector3 gridAdjustedPosition = buildGrid.GridCellCenterWorldPos(gridPos.x, gridPos.y);
            GameObject module = Instantiate(modulePrefab, gridAdjustedPosition, Quaternion.identity);
            buildGrid.SetObjectAtCoordinates(gridPos.x, gridPos.y, module);
            modulePlacedEvent?.Invoke();
        }
    }
    
    // Select a module if the cursor is over it.
    public void SelectModule(Vector2 mousePosition)
    {
        if(isModuleSelected) return;
        Ray ray = cam.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, moduleLayer))
        {
            if (hit.collider.gameObject.TryGetComponent<ModuleBase>(out var module))
            {
                module.SelectModule();
                moduleSelectedEvent?.Invoke();
                isModuleSelected = true;
            }
        }
    }
    
    // Deselct a module to allow for a new selection. Functionality should be expanded and sent to SelectionManager.
    public void DeselectModule()
    {
        isModuleSelected = false;
    }

    // Should be updated to just be given a point from a SelectionManager. Functionality should remain largely the same. 
    private void UpdatePlacementInfo()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 position = hit.point;
            LastHitPoint = position;
            float checkRadius = 0.65f;

            Collider[] colliders = Physics.OverlapSphere(position, checkRadius);
            CanPlaceModule = AllCollidersAreGroundLayer(colliders);
            gridMaterial.SetVector("_CenterPos", new Vector4(position.x, position.y, position.z));
            
        }
    }

    private bool AllCollidersAreGroundLayer(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            if ((groundLayer.value & 1 << collider.gameObject.layer) == 0)
            {
                return false;
            }
        }
        return true;
    }

    // Should be updated to provide a transparent version of the module attempted placement.
    private void UpdateSpherePosition(){
        Vector3 gridAdjustedPosition = buildGrid.GridCellCenterWorldPos(LastHitPoint);
        rangeIndicatorObject.transform.position = gridAdjustedPosition;
        Color color = CanPlaceModule ? Color.green : Color.red;
        color = new Vector4(color.r, color.g, color.b, 0.3f);
        rangeIndicatorObject.GetComponent<MeshRenderer>().material.color = color;
    }
    
    void OnDrawGizmos()
    {
        if(ShowGizmos)
        {
            Gizmos.color = CanPlaceModule ? Color.green : Color.red;
            Gizmos.DrawLine(cam.transform.position, LastHitPoint);
            Gizmos.color = CanPlaceModule ? Color.green : Color.red;
            Gizmos.DrawWireSphere(LastHitPoint, checkRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(buildGrid.Origin, buildGrid.CellSize * new Vector3(buildGrid.Width, 1, buildGrid.Height) );
        }
    }

    private GameObject GetModulePrefab(ModuleTypes moduleType)
    {
        switch(moduleType)
        {
            case ModuleTypes.TurnModule:
                return turnModulePrefab;
            case ModuleTypes.ElseGateModule:
                return elseGateModulePrefab;
            case ModuleTypes.MergeModule:
                return mergeModulePrefab;
            default:
                Debug.LogError("Unsupported module type: " + moduleType);
                return null;
        }
    }

    private void SetGridParameters() {
        gridMaterial.SetVector("_TileSize", Vector4.one * buildGrid.CellSize);
        gridMaterial.SetVector("_GridOffset", new Vector4(buildGrid.Origin.x, buildGrid.Origin.y, buildGrid.Origin.z, 0));
        visualGridObject.transform.position = Vector3.up * 0.01f + new Vector3(buildGrid.Width, 0, buildGrid.Height) / 2 * buildGrid.CellSize + buildGrid.Origin;
        visualGridObject.transform.localScale = new Vector3(buildGrid.Width, 1, buildGrid.Height) * buildGrid.CellSize / 10f;
    }

    private void OnValidate() {
        gridMaterial = visualGridObject.GetComponent<Renderer>().sharedMaterial;
        SetGridParameters();
    }
}
