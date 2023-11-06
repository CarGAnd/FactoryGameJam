using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPropertiesHolder : MonoBehaviour
{
    [SerializeField]
    private LevelProperties levelProperties;

    public LevelProperties Properties => levelProperties;

    public static LevelPropertiesHolder Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
