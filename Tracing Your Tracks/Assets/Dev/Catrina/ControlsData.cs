using System;
using UnityEngine;

[Serializable]
public class ControlEntry
{
    public string key;
    public string action;
}

[CreateAssetMenu(fileName = "ControlsData", menuName = "Menu/Controls List")]
public class ControlsData : ScriptableObject
{
    public ControlEntry[] entries;
}