using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStorageTransportable : ITransportable
{
    int StorageCapacity {get;}
    void StoreObject(AssemblyTravelingObject travelingObject);
    AssemblyTravelingObject RetrieveObject();
    bool IsStorageFull();
}
