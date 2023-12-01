using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ObjectTravelAssemblyLine<T> : ITravelAssemblyLine<T>
{
    private UnityEvent<bool> onTraveling = new UnityEvent<bool>();
    private IAssemblyTravelMethod assemblyTravelMethod;
    private T objectValue;
    public T Value {get => objectValue; private set => objectValue = value; }
    public IAssemblyTravelMethod AssemblyTravelMethod { get => assemblyTravelMethod; set => assemblyTravelMethod = value; }

    public UnityEvent<bool> OnTraveling {get {return onTraveling;} set { onTraveling = value;} }

    public ObjectTravelAssemblyLine (T _value) {
        Value = _value;
    }

    public void OnTravelFinished()
    {
        OnTraveling?.Invoke(false);
        AssemblyTravelMethod.TravelFinished -= OnTravelFinished;
        AssemblyTravelMethod.FinishTravel(this);
    }

    public void InitializeAtAssemblyLine(IAssembly _assembly, Transform _transform)
    {
        AssemblyTravelMethod = _assembly.AssemblyTravelMethod;
        OnTraveling?.Invoke(true);
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
