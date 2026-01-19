using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NameData", menuName = "Tools/Name Data")]
public class NameScriptableObject : ScriptableObject
{
    public List<NameRegion> myRegions = new List<NameRegion>();
}
[System.Serializable]
public class NameRegion
{
    public string myRegion;
    public List<string> MaleNames;
    public List<string> FemaleNames;
    public List<string> LastNames;
}
