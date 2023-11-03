using Unity.VisualScripting;
using UnityEngine;

public class ObjectTravelAssemblyLine<T> : ITravelAssemblyLine<T>
{
    private IAssemblyTravelMethod assemblyTravelMethod;
    private T objectValue;
    public T Value {get => objectValue; private set => objectValue = value; }
    public IAssemblyTravelMethod AssemblyTravelMethod { get => assemblyTravelMethod; set => assemblyTravelMethod = value; }

    public ObjectTravelAssemblyLine (T _value) {
        Value = _value;
    }

    public void OnTravelFinished()
    {
        AssemblyTravelMethod.TravelFinished -= OnTravelFinished;
        AssemblyTravelMethod.FinishTravel(this);
    }

    public void InitializeAtAssemblyLine(IAssembly _assembly, Transform _transform)
    {
        AssemblyTravelMethod = _assembly.AssemblyTravelMethod;
        AssemblyTravelMethod.StartTravel(_assembly, _transform);
        AssemblyTravelMethod.TravelFinished += OnTravelFinished;
    }

    public void UpdateTravel()
    {
        if (AssemblyTravelMethod == null)
            return;

        AssemblyTravelMethod.UpdateTravel();
    }
}
