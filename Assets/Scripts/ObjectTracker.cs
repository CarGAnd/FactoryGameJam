using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectTracker
{
    private int numSpawnedObjects;
    private int numCollectedObjects;

    private FactoryTracker factoryTracker;
    private LevelManager levelManager;

    public void StartTracking(FactoryTracker factoryTracker, LevelManager levelManager) {
        this.factoryTracker = factoryTracker;
        this.levelManager = levelManager;
        numSpawnedObjects = factoryTracker.GetNumObjectsInLevel();
        factoryTracker.OnObjectCollected += OnObjectCollected;
        numCollectedObjects = 0;
    }

    private void OnObjectCollected() {
        numCollectedObjects += 1;
        if(numCollectedObjects >= numSpawnedObjects) {
            levelManager.GoToLevelCompletedPhase();
            factoryTracker.OnObjectCollected -= OnObjectCollected;
        }
    }
}
