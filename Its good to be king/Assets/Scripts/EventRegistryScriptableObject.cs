using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventDataRegistry", menuName = "Tools/Event Data Registry")]
public class EventRegistryScriptableObject : ScriptableObject
{
    public List<EventScriptableObject> Events = new List<EventScriptableObject>();
}
