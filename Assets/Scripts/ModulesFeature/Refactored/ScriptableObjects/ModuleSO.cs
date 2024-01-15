using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ModuleSO : GridObjectSO
{
    public PortLayout portLayout;

    public List<PortSettings> GetInputs(int numRotations) { 
        List<PortSettings> portSettings = portLayout.GetInputPorts();
        foreach(PortSettings ps in portSettings) {
            ps.position = GetRotatedPosition(ps.position, numRotations);
            ps.direction = ps.direction.RotatedDirection(numRotations);
        }
        return portSettings;
    }

    public List<PortSettings> GetOutputs(int numRotations) {
        List<PortSettings> portSettings = portLayout.GetOutputPorts();
        foreach(PortSettings ps in portSettings) {
            ps.position = GetRotatedPosition(ps.position, numRotations);
            ps.direction = ps.direction.RotatedDirection(numRotations);
        }
        return portSettings;
    }

    public override IGridObject CreateInstance(Vector3 position, Quaternion rotation, int numRotations) {
        GameObject g = Instantiate(ModulePrefab, position, rotation);
        ModuleInputOutput inputOutput = g.GetComponent<ModuleInputOutput>();
        inputOutput.Initialize(this, numRotations);
        return inputOutput;
    }
}
