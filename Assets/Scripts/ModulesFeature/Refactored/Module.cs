using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ModuleInputOutput))]
public abstract class Module : MonoBehaviour
{
    protected ModuleInputOutput inputOutput;

    private void Awake() {
        inputOutput = GetComponent<ModuleInputOutput>();
    }

    protected void SendObjectOut(AssemblyTravelingObject obj) {
        inputOutput.SendToOutput(obj);
    }

    protected AssemblyTravelingObject GetObjectIn() {
        return inputOutput.ReceiveFromInput();
    }

    private void OnEnable() {
        inputOutput.receivedObject.AddListener(OnObjectReceived);
    }

    private void OnDisable() {
        inputOutput.receivedObject.RemoveListener(OnObjectReceived);
    }

    protected virtual void OnObjectReceived(AssemblyTravelingObject newObj) {

    }
}
