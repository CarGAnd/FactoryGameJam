using System.Collections.Generic;
using UnityEngine;

public static class GridInteraction
{
    private static Dictionary<string, NewGrid> grids = new();

    public static void RegisterGrid(NewGrid grid)
    {
        if (grid != null && !grids.ContainsKey(grid.GetGridID()))
        {
            grids.Add(grid.GetGridID(), grid);
        }
    }

    public static void UnregisterGrid(NewGrid grid)
    {
        if (grid != null && grids.ContainsKey(grid.GetGridID()))
        {
            grids.Remove(grid.GetGridID());
        }
    }

    public static NewGrid GetGridById(string gridId)
    {
        if (grids.TryGetValue(gridId, out NewGrid grid))
        {
            return grid;
        }
        return null;
    }
}

