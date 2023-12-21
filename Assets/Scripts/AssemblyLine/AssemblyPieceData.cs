using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AssemblyPieceData : ScriptableObject
{
    [Tooltip("The prefab that is specific to this piece.")]
    public GameObject prefab;
    public int movementDistance;
    public int cost;
    public AssemblyPieceType type;
}
