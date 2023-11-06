using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProperty
{
    string Name { get; }
    object GetValue();
}
    
