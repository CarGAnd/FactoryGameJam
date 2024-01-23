using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ModuleSO : GridObjectSO
{
    public PortLayout portLayout;

    public List<PortSettings> GetInputs(Facing facing) { 
        List<PortSettings> portSettings = portLayout.GetInputPorts();
        foreach(PortSettings ps in portSettings) {
            ps.relativePosition = GetRotatedPosition(ps.relativePosition, facing.GetNumRotations());
            ps.direction = ps.direction.RotatedDirection(facing.GetNumRotations());
        }
        return portSettings;
    }

    public List<PortSettings> GetOutputs(Facing facing) {
        List<PortSettings> portSettings = portLayout.GetOutputPorts();
        foreach(PortSettings ps in portSettings) {
            ps.relativePosition = GetRotatedPosition(ps.relativePosition, facing.GetNumRotations());
            ps.direction = ps.direction.RotatedDirection(facing.GetNumRotations());
        }
        return portSettings;
    }

    public override IGridObject CreateInstance(Vector3 position, Quaternion rotation, Facing facing, AssemblyLineSystem assemblyLineSystem) {
        GameObject g = Instantiate(ModulePrefab, position, rotation);
        ModuleInputOutput inputOutput = g.GetComponent<ModuleInputOutput>();
        inputOutput.Initialize(this, facing, assemblyLineSystem);
        return inputOutput;
    }
}
