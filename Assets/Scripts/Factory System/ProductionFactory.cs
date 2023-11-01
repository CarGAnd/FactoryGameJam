using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionFactory : BaseFactory 
{
    float lastTimeProduced;
    float timeBetweenProduction = 2f;

    public override void Input(FactoryObject fObject) {
        throw new System.NotImplementedException();
    }

    private void Start() {
        lastTimeProduced = Time.time;
    }

    private void Update() {
        if(Time.time > lastTimeProduced + timeBetweenProduction) {
            Produce();
            lastTimeProduced = Time.time;
        }
    }

    private void Produce() {
        FactoryObject o = new FactoryObject();
        Output(o);
    }
}
