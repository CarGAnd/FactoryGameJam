using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryModule : BaseFactory {

    public override void Input(FactoryObject fObject) {
        FactoryObject processed = Process(fObject);
        if(output1 != null) {
            Output(processed);
        } 
    }

    private FactoryObject Process(FactoryObject fObject) {
        fObject.test += 1;
        Debug.Log("Processed " + fObject.name);
        return fObject;
    }
}
