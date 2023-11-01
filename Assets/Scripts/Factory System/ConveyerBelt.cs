using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : BaseFactory {

    public override void Input(FactoryObject fObject) {
        Output(fObject);
    }

    private void Update() {
        //Move objects and output to next factory if they have moved far enough
    }
}
