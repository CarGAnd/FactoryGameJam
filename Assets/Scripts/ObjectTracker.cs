using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectTracker
{
    private int numSpawnedObjects;
    private int numCollectedObjects;
    private int numDestroyedObject;

    private FactoryTracker factoryTracker;
    private LevelManager levelManager;

    public void StartTracking(FactoryTracker factoryTracker, LevelManager levelManager) {
        this.factoryTracker = factoryTracker;
        this.levelManager = levelManager;
        numSpawnedObjects = factoryTracker.GetNumObjectsInLevel();
        factoryTracker.OnObjectCollected += OnObjectCollected;
        factoryTracker.OnStuckObjectDestroyed += OnObjectDestroyed;
        numCollectedObjects = 0;
    }

    private void OnObjectCollected() {
        numCollectedObjects += 1;
        if (AllObjectsDoneMoving()) {
            FinishLevel();
        }
    }

    private void OnObjectDestroyed() {
        numDestroyedObject += 1;
        if (AllObjectsDoneMoving()) {
            FinishLevel();
        }
    }

    private void FinishLevel() {
        levelManager.GoToLevelCompletedPhase();
        factoryTracker.OnObjectCollected -= OnObjectCollected;
        factoryTracker.OnStuckObjectDestroyed -= OnObjectDestroyed;
    }

    private bool AllObjectsDoneMoving() {
        return numCollectedObjects + numDestroyedObject >= numSpawnedObjects;
    }
}
