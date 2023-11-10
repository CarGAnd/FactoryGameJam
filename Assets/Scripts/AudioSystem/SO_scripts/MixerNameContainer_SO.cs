using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MixerNameContainer", menuName = "Audio/MixerNameContainer", order = 2)]
public class MixerNameContainer_SO : ScriptableObject
{
    [SerializeField]
    public List<string> busNameContainer = new List<string>();
    [SerializeField]
    public List<string> vcaNameContainer = new List<string>();
}