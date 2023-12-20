using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
[CreateAssetMenu(fileName = "AssemblyLine", menuName = "AssemblyLine/Create New AssemblyLinePiece", order = 1)]
public class AssemblyLineScriptableObject : ScriptableObject
{
    public GameObject prefab;
    [Tooltip("The distance the object will move in the direction it is facing")]
    public int movementDistance;
    [Tooltip("The cost of the assemblyline piece")]
    public int cost;
}
