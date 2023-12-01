using Sirenix.OdinInspector;
using SOS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class ConfigurableModule : ModuleBase
{
    private UnityEvent<GameObject> onRemovedModule;
    [SerializeField] private ModuleTypeRef onModuleMenuOpen;
    [SerializeField] private ModuleTypeRef onModuleMenuClose;
    [SerializeField] private GameEvent onUIButtonPressed;
    [SerializeField] private int cost = 5;
    [SerializeField] private IntRef LevelCost;
    [Required] [SerializeField] protected Canvas UICanvas;
    [Required] [SerializeField] protected GameObject UI;
    protected Button deleteButton;
    protected Button saveButton;
    public abstract ModuleType ModuleType {get;}

    protected override void Awake() {
        base.Awake();
        Initialize();
    }

    void OnEnable () {
        SubscribeToButtons();
    }

    void OnDisable() {
        UnsubscribeFromButtons();

        if (UICanvas == null)
            return;

        UI.SetActive(true);
    }

    public void SubscribeToRemoveModule(UnityAction<GameObject> action) {
        onRemovedModule.AddListener(action);
    }

    public void UnsubscribeFromRemoveModule(UnityAction<GameObject> action) {
        onRemovedModule.RemoveListener(action);
    }

    protected virtual void Initialize() {
        LevelCost.Value += cost;
        SetUIElements();
        UICanvas.gameObject.SetActive(true);
    }

    public virtual void ApplySettings() {
        onUIButtonPressed.Invoke();
        onModuleMenuClose.Value = ModuleType;
        ModulesManager.Instance.DeselectModule();
        if(UI != null)
        {
            UI.SetActive(false);
        }
    }

    public override void SelectModule()
    {
        onModuleMenuOpen.Value = ModuleType;
        UI.transform.position = GetUIPositionByMouse();
        UI.SetActive(true);
    }
   
    protected virtual void SetUIElements() {
        Button[] buttons = UI.GetComponentsInChildren<Button>();
        deleteButton = buttons[0];
        saveButton = buttons[1];
    }

    private void SubscribeToButtons() {
        deleteButton.onClick.AddListener(DestroyModule);
        saveButton.onClick.AddListener(ApplySettings);
    }

    private void UnsubscribeFromButtons() {
        deleteButton.onClick.RemoveListener(DestroyModule);
        saveButton.onClick.RemoveListener(ApplySettings);
    }

    protected virtual void DestroyModule() {
        if (!ModuleIsRemoveable) {
            return;
        }

        onUIButtonPressed.Invoke();
        LevelCost.Value -= cost;
        moduleAssemblyController.DisconnectAllAssemblies();
        onRemovedModule?.Invoke(gameObject);
        ModulesManager.Instance.DeleteModule(this);
        ModulesManager.Instance.DeselectModule();
        Destroy(gameObject);

    }

    protected Vector2 GetUIPositionByMouse() {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        UICanvas.transform as RectTransform, Input.mousePosition,
        UICanvas.worldCamera,
        out Vector2 uiPos);
        return UICanvas.transform.TransformPoint(uiPos);
    }

    
}
