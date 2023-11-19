using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class SpawnerFeatureTests
{
    /*[Test]
    public void SpawnerSetsCorrectProperties() {
        GameObject g = new GameObject();
        g.AddComponent<AssemblyObject>();
        SpawnWave wave1 = new SpawnWave(1);
        wave1.rotationName = "45 Degrees";
        wave1.colorName = "Yellow";
        List<SpawnWave> waves = new List<SpawnWave>();
        waves.Add(wave1);
        Spawner spawner = new Spawner(waves, g);

        AssemblyObject spawnedObject = spawner.GetNextObject();

        Assert.True(spawnedObject.Properties.CompareProperties(wave1.CreateProperties()));
    }

    [Test]
    public void SpawnerSpawnsCorrectAmount() {
        GameObject g = new GameObject();
        g.AddComponent<AssemblyObject>();

        int objectsPerWave = 3;

        SpawnWave wave1 = new SpawnWave(objectsPerWave);
        SpawnWave wave2 = new SpawnWave(objectsPerWave);

        wave1.rotationName = "45 Degrees";
        wave1.colorName = "Yellow";

        wave1.rotationName = "90 Degrees";
        wave1.colorName = "Purple";

        List<SpawnWave> waves = new List<SpawnWave>();
        waves.Add(wave1);
        waves.Add(wave2);

        Spawner spawner = new Spawner(waves, g);

        for(int i = 0; i < waves.Count; i++) {
            for (int j = 0; j < objectsPerWave; j++) {
                AssemblyObject spawnedObject = spawner.GetNextObject();
                Assert.True(spawnedObject.Properties.CompareProperties(waves[i].CreateProperties()));
            }
        }
        Assert.True(spawner.IsFinished);  
    }*/
}
