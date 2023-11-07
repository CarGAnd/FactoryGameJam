using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PropertyDropdownHelper
{
    public static bool IsColorAvailable(LevelProperties levelProperties)
    {
        return levelProperties != null && levelProperties.colorProperties.Count > 0;
    }


    public static bool IsRotationAvailable(LevelProperties levelProperties)
    {
        return levelProperties != null && levelProperties.rotationProperties.Count > 0;
    }
    public static IEnumerable<string> GetColorNames(LevelProperties levelProperties)
    {
        var names = new List<string> { "NONE" };
        if (levelProperties != null)
        {
            names.AddRange(levelProperties.colorProperties.Select(x => x.name));
        }
        return names;
    }

    public static IEnumerable<string> GetRotationNames(LevelProperties levelProperties)
    {
        var names = new List<string> { "NONE" };
        if (levelProperties != null)
        {
            names.AddRange(levelProperties.rotationProperties.Select(x => x.name));
        }
        return names;
    }

}

