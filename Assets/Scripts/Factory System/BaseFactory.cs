using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFactory : MonoBehaviour
{
    [SerializeField] protected BaseFactory output1;

    public abstract void Input(FactoryObject fObject);

    protected virtual void Output(FactoryObject fObject) {
        output1.Input(fObject);
    }
}
