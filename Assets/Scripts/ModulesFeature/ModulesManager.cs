using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using SOS;
using UnityEngine;

public enum ModuleTypes
{
    TurnModule,
    ElseGateModule
}

public class ModulesManager : MonoBehaviour
{
    public static ModulesManager Instance { get; private set; }

    [SerializeField] private LevelStateRef currentLevelStateRef;
    [SerializeField]
    private GameObject turnModulePrefab;
    [SerializeField]
    private GameObject elseGateModulePrefab;
    [SerializeField]
    private Camera cam;
    public LayerMask groundLayer;
    public LayerMask moduleLayer;
    [SerializeField]
    private GameObject rangeIndicatorPrefab;
    [SerializeField]
    private float checkRadius = 0.65f; 

    public bool ShowGizmos {get; private set;}
    public Vector3 LastHitPoint { get; private set; }
    public bool CanPlaceModule { get; private set; }
    private bool isModuleSelected = false;

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
        rangeIndicatorPrefab = Instantiate(rangeIndicatorPrefab, Vector3.zero, Quaternion.identity);
        rangeIndicatorPrefab.SetActive(false);
    }

    public void ToggleGizmos(bool show){
        if(currentLevelStateRef.Value != LevelState.BuildPhase)
        {
            return;
        }
        ShowGizmos = show;
        if(ShowGizmos){
            rangeIndicatorPrefab.SetActive(true);
        }
        else{
            rangeIndicatorPrefab.SetActive(false);
        }
    }
    public void PlaceModule(ModuleTypes moduleType)
    {
        GameObject modulePrefab = GetModulePrefab(moduleType);
        if (modulePrefab != null && CanPlaceModule && currentLevelStateRef.Value == LevelState.BuildPhase)
        {
            Instantiate(modulePrefab, LastHitPoint, Quaternion.identity);
        }
    }

    void Update()
    {
        if (ShowGizmos)
        {
            UpdatePlacementInfo();
            UpdateSpherePosition();
        }
    }

    public void SelectModule()
    {
        if(isModuleSelected) return;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Cast the ray and check if it hits a module directly
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, moduleLayer))
        {
            ModuleBase module = hit.collider.gameObject.GetComponent<ModuleBase>();
            if (module != null)
            {
                module.SelectModule();
                isModuleSelected = true;
            }
        }
    }

    public void DeselectModule()
    {
        isModuleSelected = false;
    }

    private void UpdatePlacementInfo()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 position = hit.point;
            LastHitPoint = position;
            float checkRadius = 0.65f;  // Set this value to an appropriate radius for your modules

            Collider[] colliders = Physics.OverlapSphere(position, checkRadius);
            foreach (Collider collider in colliders)
            {
                // If the collider is not on the ground layer
                if ((groundLayer.value & 1 << collider.gameObject.layer) == 0)
                {
                    CanPlaceModule = false;
                    return;  // Exit the method if an obstruction is found
                }
            }
            CanPlaceModule = true;
        }
    }
    private void UpdateSpherePosition(){
        rangeIndicatorPrefab.transform.position = LastHitPoint;
        Color color = CanPlaceModule ? Color.green : Color.red;
        color = new Vector4(color.r, color.g, color.b, 0.3f);
        rangeIndicatorPrefab.GetComponent<MeshRenderer>().material.color = color;
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
            default:
                Debug.LogError("Unsupported module type: " + moduleType);
                return null;
        }
    }
}
