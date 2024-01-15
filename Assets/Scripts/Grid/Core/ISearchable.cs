using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISearchable
{
    List<Vector2Int> GetNeighbors(Vector2Int coord);
}
