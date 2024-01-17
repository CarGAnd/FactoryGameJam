using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwarderModule : Module
{
    private void Update() {
        AssemblyTravelingObject inputObj = GetObjectIn();
        if(inputObj != null) {
            SendObjectOut(inputObj);
        }
    }
}
