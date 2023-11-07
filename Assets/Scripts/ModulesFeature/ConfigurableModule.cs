using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public abstract class ConfigurableModule : ModuleBase
{
    protected Canvas UICanvas;
    protected GameObject UI;
    protected Button deleteButton;
    protected Button saveButton;

    public virtual void ApplySettings() {
        UnsubscribeFromButtons();
    }

    public override void SelectModule()
    {
        UI.transform.position = GetUIPositionByMouse();
        SubscribeToButtons();
        UI.SetActive(true);
    }

    void OnDisable(){
        UnsubscribeFromButtons();

        if (UI == null)
            return;

        UI.SetActive(false);
    }

    protected virtual void Initialize() {
        SetUIElements();
    }

    protected virtual void SetUIElements() {
        if (UICanvas == null) {
            UICanvas = GameObject.Find("UI").GetComponent<Canvas>();
        }

        if (UICanvas == null) {
            Debug.LogError("No UI canvas was found!");
            return;
        }
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

        UnsubscribeFromButtons();
        moduleAssemblyController.DisconnectAllAssemblies();
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
