using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectTracker
{
    private int numSpawnedObjects;
    private int numDestroyedObjects;

    private FactoryTracker factoryTracker;
    private LevelManager levelManager;

    public void StartTracking(FactoryTracker factoryTracker, LevelManager levelManager) {
        this.factoryTracker = factoryTracker;
        this.levelManager = levelManager;
        numSpawnedObjects = factoryTracker.GetNumObjectsInLevel();
        factoryTracker.OnObjectDestroyed += OnObjectDestroyed;
    }

    private void OnObjectDestroyed(GameObject obj) {
        numDestroyedObjects += 1;
        if (AllObjectsDoneMoving()) {
            FinishLevel();
        }
    }

    private void FinishLevel() {
        levelManager.GoToLevelCompletedPhase();
        factoryTracker.OnObjectDestroyed -= OnObjectDestroyed;
    }

    private bool AllObjectsDoneMoving() {
        return numDestroyedObjects >= numSpawnedObjects;
    }
}
