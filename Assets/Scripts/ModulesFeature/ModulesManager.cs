using SOS;
using UnityEngine;
using UnityEngine.Events;
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
    public UnityEvent<Vector3> MouseOverGridSpace;
    public UnityEvent<bool> BuildModeToggled;
    private UnityEvent<GameObject> onPlacedModule;
    [SerializeField] private GameEvent moduleSelectedEvent;
    [SerializeField] private LevelStateRef currentLevelStateRef;
    [SerializeField] private GameObject turnModulePrefab;
    [SerializeField] private GameObject elseGateModulePrefab;
    [SerializeField] private GameObject mergeModulePrefab;
    [SerializeField] private Camera cam;
    [SerializeField] private float checkRadius = 0.65f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask moduleLayer;
    [field: SerializeField] public FactoryGrid BuildGrid { get; private set; }
    private bool isModuleSelected = false;
    public static ModulesManager Instance { get; private set; }
    public bool ShowGizmos {get; private set;}
    public Vector3 LastHitPoint { get; private set; }
    public bool CanPlaceModule { get; private set; }

    private Vector2Int lastMouseGridPos;
    
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
    }

    void Update()
    {
        // If the player is in build mode, update the placement info
        if (ShowGizmos)
        {
            UpdatePlacementInfo();
        }
    }

    public void DeleteModule(ModuleBase module) {
        Vector3 modulePosition = module.transform.position;
        Vector2Int gridCell = BuildGrid.GetCellCoords(modulePosition);
        BuildGrid.RemoveObject(gridCell);
    }


    public void SubscribeToOnPlacedModule(UnityAction<GameObject> action)
    {
        onPlacedModule.AddListener(action);
    }

    public void UnsubscribeFromOnPlacedModule(UnityAction<GameObject> action)
    {
        onPlacedModule.RemoveListener(action);
    }
    
    // Toggle gizmos on and off Accessible via player, should be moved to a SelectionManager
    public void ToggleGizmos(bool show){
        /*if(currentLevelStateRef.Value != LevelState.BuildPhase)
        {
            return;
        }*/
        ShowGizmos = show;
        BuildModeToggled?.Invoke(show);
    }
    
    // If possible, place a module at the mouse Position.
    public void PlaceModule(ModuleTypes moduleType, Vector3 position)
    {
        GameObject modulePrefab = GetModulePrefab(moduleType);
        if (modulePrefab != null)
        {
            Vector2Int gridCell = BuildGrid.GetCellCoords(position);
            Vector3 gridCellCenter = BuildGrid.GetCellCenter(gridCell);
            GameObject module = Instantiate(modulePrefab, gridCellCenter, Quaternion.identity);
            IGridObject gridObject = module.GetComponent<IGridObject>();
            BuildGrid.PlaceObject(gridObject, gridCell, null);
            onPlacedModule?.Invoke(modulePrefab);
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
            Vector3 gridCellCenter = BuildGrid.GetCellCenter(position);
            LastHitPoint = position;
            float checkRadius = 0.65f;

            Collider[] colliders = Physics.OverlapSphere(gridCellCenter, checkRadius);
            CanPlaceModule = AllCollidersAreGroundLayer(colliders) && !BuildGrid.PositionIsOccupied(position);
            if(BuildGrid.GetCellCoords(position) != lastMouseGridPos) {
                lastMouseGridPos = BuildGrid.GetCellCoords(position);
                MouseOverGridSpace?.Invoke(gridCellCenter);
            }
            
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
    
    void OnDrawGizmos()
    {
        if(ShowGizmos)
        {
            Gizmos.color = CanPlaceModule ? Color.green : Color.red;
            Gizmos.DrawLine(cam.transform.position, LastHitPoint);
            Gizmos.color = CanPlaceModule ? Color.green : Color.red;
            Gizmos.DrawWireSphere(LastHitPoint, checkRadius);
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

   /* private void SetGridParameters() {
        if(gridMaterial == null) {
            return;
        }
        gridMaterial.SetVector("_TileSize", Vector4.one * BuildGrid.CellSize);
        gridMaterial.SetVector("_GridOffset", -new Vector4(BuildGrid.Origin.x, BuildGrid.Origin.z, 0, 0) / BuildGrid.CellSize);
        gridMaterial.SetFloat("_Rotation", BuildGrid.Rotation);
        
        visualGridObject.transform.position = Vector3.up * 0.01f + new Vector3(BuildGrid.Width, 0, BuildGrid.Height) / 2 * BuildGrid.CellSize + BuildGrid.Origin;
        visualGridObject.transform.rotation = Quaternion.Euler(0, BuildGrid.Rotation, 0);
        visualGridObject.transform.localScale = new Vector3(BuildGrid.Width, 1, BuildGrid.Height) * BuildGrid.CellSize / 10f;
    }

    private void OnValidate() {
        Renderer rend = visualGridObject.GetComponent<Renderer>();
        if(rend == null) {
            return;
        }
        gridMaterial = rend.sharedMaterial;
        SetGridParameters();
    }*/
}
