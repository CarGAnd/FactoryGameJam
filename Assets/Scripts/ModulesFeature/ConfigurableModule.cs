using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SOS;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class ConfigurableModule : ModuleBase
{
    [SerializeField]
    private int cost = 5;
    [SerializeField]
    private IntRef LevelCost;
    [Required]
    [SerializeField]
    protected Canvas UICanvas;
    [Required]
    [SerializeField]
    protected GameObject UI;
    protected Button deleteButton;
    protected Button saveButton;

    protected override void Awake() {
        base.Awake();
        Initialize();
    }

    protected virtual void Initialize() {
        LevelCost.Value += cost;
        SetUIElements();
        UICanvas.gameObject.SetActive(true);
    }

    public virtual void ApplySettings() {
        ModulesManager.Instance.DeselectModule();
        if(UI != null)
        {
            UI.SetActive(false);
        }
    }

    public override void SelectModule()
    {
        UI.transform.position = GetUIPositionByMouse();
        UI.SetActive(true);
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

        LevelCost.Value -= cost;
        moduleAssemblyController.DisconnectAllAssemblies();
        Destroy(gameObject);
        ModulesManager.Instance.DeselectModule();
    }

    protected Vector2 GetUIPositionByMouse() {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        UICanvas.transform as RectTransform, Input.mousePosition,
        UICanvas.worldCamera,
        out Vector2 uiPos);
        return UICanvas.transform.TransformPoint(uiPos);
    }
}
