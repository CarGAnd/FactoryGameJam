using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FactoryTracker", menuName = "Managers/FactoryTracker")]
public class FactoryTracker : ScriptableObject
{
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
    }

    public void DeregisterContainer(ContainerModule container) {
        containers.Remove(container);
    }

    public int GetNumObjectsInLevel() {
        int total = 0;
        foreach(SpawnerModule sm in spawners) {
            total += sm.TotalNumSpawns;
        }
        return total;
    }
}
