using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsTravelMethod<T> : ITravelAssemblyLine<T>
{
    private Action<GameObject> travelStarted;
    private Action<GameObject> travelStopped;
    private GameObject gameObject;
    private Queue<Vector3> travelPoints;
    private bool isTraveling = false;
    private float speed = 1.0f;
    private float minimumDistance = 0.01f;
    private Vector3 targetPosition;
    private T objectValue;
    public bool IsTraveling 
    { 
        get => isTraveling; 
        set 
        {
            if (value && !isTraveling)
                TravelStarted?.Invoke(gameObject);

            if (!value && isTraveling)
                TravelStopped?.Invoke(gameObject);

            isTraveling = value;
        }  
    }

    public Action<GameObject> TravelStarted { get => travelStarted; set => travelStarted = value; }
    public Action<GameObject> TravelStopped { get => travelStopped; set => travelStopped = value; }
    public T Value {get => objectValue; private set => objectValue = value; }

    public MoveTowardsTravelMethod (GameObject _gameObject, T _value, float _speed = 1f, float _minimumDistance = 0.01f) {
        Value = _value;
        gameObject = _gameObject;
        speed = _speed;
        minimumDistance = _minimumDistance;
    }

    public void StartTravel(List<Vector3> _travelPoints)
    {
        travelPoints = new Queue<Vector3>(_travelPoints);
        targetPosition = travelPoints.Dequeue();
        IsTraveling = true;
    }

    public void UpdateTravel()
    {   
        if (targetPosition == null)
            return;

        if (!IsTraveling)
            return;

        UpdatePosition();
        Move();
    }

    private void Move() {
        var step =  speed * Time.deltaTime; // calculate distance to move
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition, step);
    }

    private void UpdatePosition() {
        if (Vector3.Distance(gameObject.transform.position, targetPosition) < minimumDistance)
        {
            if (travelPoints == null || travelPoints.Count <= 0)
            {
                IsTraveling = false;
                OnTravelFinished();
                return;
            }

            targetPosition = travelPoints.Dequeue();
        }
    }

    public void OnTravelFinished()
    {
        IAssemblyController<T> assemblyController = GetAssemblyController();

        if (assemblyController == null)
            return;

        assemblyController.ReceivedAssemblyObject(this);
    }
    
    // TODO: Change to avoid using casts, and get direct reference instead.
    private IAssemblyController<T> GetAssemblyController() {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(gameObject.transform.position, Vector3.right, 100.0F);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject.TryGetComponent(out IAssemblyController<T> assemblyController)) {
                return assemblyController;
            }
        }

        Debug.LogWarning("No AssemblyController was found!");

        return null;
    }
}
