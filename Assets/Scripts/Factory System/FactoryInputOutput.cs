using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryInputOutput : BaseFactory {

    [SerializeField] private bool isInput;

    public override void Input(FactoryObject fObject) {
        Output(fObject);
    }

}
