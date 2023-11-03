using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsTravelMethod : IAssemblyTravelMethod
{
    public MoveTowardsTravelMethod(float _moveSpeed = 5f, float _minimumDistance = 0.01f) {
        minimumDistance = _minimumDistance;
        moveSpeed = _moveSpeed;
    }
    private Action travelStarted, travelFinished, travelTick;
    private IAssembly assemblyStart, assemblyDestination;
    private float moveSpeed, minimumDistance;
    private Transform transformTraveling;
    private Vector3 targetPosition;
    private Queue<Vector3> travelPoints;

    public Action TravelStarted { get => travelStarted; set => travelStarted = value; }
    public Action TravelFinished { get => travelFinished; set => travelFinished = value; }
    public Action TravelTick { get => travelTick; set => travelTick = value; }
    public IAssembly AssemblyStart { get => assemblyStart; set => assemblyStart = value; }
    public IAssembly AssemblyDestination { get => assemblyDestination; set => assemblyDestination = value; }

    public void StartTravel(IAssembly _assemblyStart, Transform _transform)
    {
        transformTraveling = _transform;
        AssemblyStart = _assemblyStart;
        AssemblyDestination = _assemblyStart.ConnectedTo;
        travelPoints = new Queue<Vector3>(AssemblyStart.GetTravelPositions());
        targetPosition = travelPoints.Dequeue();
    }

    public void UpdateTravel()
    {
        if (targetPosition == null)
            return;
        
        travelTick?.Invoke();

        UpdatePosition();
        Move();
    }

    private void Move() {
        var step =  moveSpeed * Time.deltaTime; // calculate distance to move
        transformTraveling.position = Vector3.MoveTowards(transformTraveling.position, targetPosition, step);
    }

    private void UpdatePosition() {
        if (Vector3.Distance(transformTraveling.position, targetPosition) < minimumDistance)
        {
            if (travelPoints == null || travelPoints.Count <= 0)
            {
                // IsTraveling = false;
                TravelFinished?.Invoke();
                return;
            }

            targetPosition = travelPoints.Dequeue();
        }
    }

    public void FinishTravel<T>(ITravelAssemblyLine<T> assemblyLine)
    {
        AssemblyDestination.OnObjectArrived(assemblyLine);
    }
}
