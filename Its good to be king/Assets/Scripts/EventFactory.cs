using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFactory : MonoBehaviour
{
    public Event CreateEvent(string eventName, string eventDescription)
    {
        Event vent = gameObject.AddComponent<Event>();
        vent.CreateEvent(eventName, eventDescription);
        return vent;
    }
}
