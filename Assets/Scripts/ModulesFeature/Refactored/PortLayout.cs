using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PortLayout 
{
    private static Facing[] directionMap = new Facing[] { Facing.North, Facing.East, Facing.South, Facing.West };

    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private int[] inputs;
    [SerializeField] private int[] outputs;

    private List<PortSettings> GetPorts(int[] portSettings) {
        List<PortSettings> ports = new List<PortSettings>();
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                //Flip the y axis so 0 is at the bottom
                int arrayIndex = (height - y - 1) * width + x;
                int arrayValue = portSettings[arrayIndex];
                if(arrayValue <= 0) {
                    continue;
                }
                arrayValue -= 1;
                PortSettings newPort = new PortSettings()
                {
                    relativePosition = new Vector2Int(x, y),
                    direction = directionMap[arrayValue]
                };
                ports.Add(newPort);
            }
        }
        return ports;
    }

    public List<PortSettings> GetInputPorts() {
        return GetPorts(inputs);
    }

    public List<PortSettings> GetOutputPorts() {
        return GetPorts(outputs);
    }
}

public class PortSettings {
    public Vector2Int relativePosition;
    public Facing direction;
}
