using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnModule : Module
{
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private AssemblyTravelingObject objectToSpawn;

    private float counter;

    private void Start() {
        counter = 0;
    }

    private void Update() {
        counter += Time.deltaTime;
        if(counter > timeBetweenSpawns) {
            counter -= timeBetweenSpawns;
            SpawnObject();
        }
    }

    private void SpawnObject() {
        GameObject obj = Instantiate(objectToSpawn.gameObject);
        AssemblyTravelingObject travelingObject = obj.GetComponent<AssemblyTravelingObject>();
        obj.SetActive(false);
        SendObjectOut(travelingObject);
    }

}
