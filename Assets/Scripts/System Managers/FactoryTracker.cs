using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "FactoryTracker", menuName = "Managers/FactoryTracker")]
public class FactoryTracker : ScriptableObject
{
    public Action OnObjectCollected;
    public Action<GameObject> OnObjectDestroyed;

    public List<SpawnerModule> Spawners { get; private set; }
    public List<ContainerModule> Containers { get; private set; }

    private void Awake() {
        Reset();
    }

    private void Reset() {
        Spawners = new List<SpawnerModule>();
        Containers = new List<ContainerModule>();
    }

    public void RegisterSpawner(SpawnerModule spawner) {
        Spawners.Add(spawner);
    }

    public void DeregisterSpawner(SpawnerModule spawner) {
        Spawners.Remove(spawner);
    }

    public void RegisterContainer(ContainerModule container) {
        Containers.Add(container);
        container.ObjectArrivedAtContainer.AddListener(ObjectCollected);
    }

    public void DeregisterContainer(ContainerModule container) {
        Containers.Remove(container);
        container.ObjectArrivedAtContainer.RemoveListener(ObjectCollected);
    }

    private void ObjectCollected(bool isCorrect) {
        OnObjectCollected?.Invoke();
    }

    public int GetNumObjectsInLevel() {
        int total = 0;
        foreach(SpawnerModule sm in Spawners) {
            total += sm.GetTotalNumSpawns();
        }
        return total;
    }

    public int GetNumCorrectItemsCollected() {
        int total = 0;
        foreach (ContainerModule cm in Containers) {
            total += cm.NumCorrectItemsCollected;
        }
        return total;
    }

    public int GetNumWrongItemsCollected() {
        int total = 0;
        foreach (ContainerModule cm in Containers) {
            total += cm.NumWrongItemsCollected;
        }
        return total;
    }
}
