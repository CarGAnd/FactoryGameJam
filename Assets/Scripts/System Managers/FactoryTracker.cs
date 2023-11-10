using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "FactoryTracker", menuName = "Managers/FactoryTracker")]
public class FactoryTracker : ScriptableObject
{
    public Action OnObjectCollected;
    public Action OnObjectDestroyed;

    public List<SpawnerModule> Spawners { get { return spawners; } }
    public List<ContainerModule> Containers { get { return containers; } }

    private List<SpawnerModule> spawners;
    private List<ContainerModule> containers;

    private void Awake() {
        Reset();
    }

    private void Reset() {
        spawners = new List<SpawnerModule>();
        containers = new List<ContainerModule>();
    }

    public void RegisterSpawner(SpawnerModule spawner) {
        spawners.Add(spawner);
    }

    public void DeregisterSpawner(SpawnerModule spawner) {
        spawners.Remove(spawner);
    }

    public void RegisterContainer(ContainerModule container) {
        containers.Add(container);
        container.OnItemCollected += ObjectCollected;
    }

    public void DeregisterContainer(ContainerModule container) {
        containers.Remove(container);
        container.OnItemCollected -= ObjectCollected;
    }

    private void ObjectCollected() {
        OnObjectCollected?.Invoke();
    }

    public int GetNumObjectsInLevel() {
        int total = 0;
        foreach(SpawnerModule sm in spawners) {
            total += sm.GetTotalNumSpawns();
        }
        return total;
    }

    public int GetNumCorrectItemsCollected() {
        int total = 0;
        foreach (ContainerModule cm in containers) {
            total += cm.NumCorrectItemsCollected;
        }
        return total;
    }

    public int GetNumWrongItemsCollected() {
        int total = 0;
        foreach (ContainerModule cm in containers) {
            total += cm.NumWrongItemsCollected;
        }
        return total;
    }
}
