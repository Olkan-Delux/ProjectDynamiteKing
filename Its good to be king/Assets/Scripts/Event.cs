using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public struct EventDesicion
{
    public string DecisionName;
    public UnityAction myEventType;
}

public class Event : MonoBehaviour
{
    public GameObject canvas;
    public GameObject button;
    public GameObject panel;
    
    string EventTitle;
    string EventDescription;
    List<EventDesicion> myDecisions;
    bool hasBeenCreated = false;

    public void CreateEvent(string eventTitle, string eventDescription)
    {
        myDecisions = new List<EventDesicion>();
        EventTitle = eventTitle;
        EventDescription = eventDescription;
    }

    public EventDesicion AddEventDecision(string eventName, UnityAction anAction)
    {
        EventDesicion eventDesicion = new EventDesicion();
        eventDesicion.DecisionName = eventName;
        eventDesicion.myEventType = anAction;
        myDecisions.Add(eventDesicion);
        return eventDesicion;
    }

    public void Create()
    {

        GameObject aPanel = Instantiate(panel);
        panel = aPanel;
        panel.transform.SetParent(canvas.transform, false);
        TextMeshProUGUI title = panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        title.SetText(EventTitle);
        TextMeshProUGUI description = panel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        description.SetText(EventDescription);
        Canvas.ForceUpdateCanvases();

        title.GetComponent<RectTransform>().localPosition = new Vector2(panel.transform.position.x, title.GetComponent<RectTransform>().localPosition.y + ((title.fontSize *0.85f) * (title.textInfo.lineCount - 1)));

        int DescriptionSpaces = description.text.Split('\n').Length;
        Vector2 firstButtonPos = new Vector3(0, ((description.transform.position.y + (description.GetComponent<RectTransform>().sizeDelta.y * 0.5f))
                                              - (((description.fontSize + description.lineSpacing) * (description.textInfo.lineCount + 1)) + ((button.GetComponent<RectTransform>().sizeDelta.y) * 0.5f))));
        int index = 0;
        foreach(EventDesicion decision in myDecisions)
        {
            GameObject aButton = Instantiate(button);
            aButton.transform.position = firstButtonPos - new Vector2(0, ((aButton.GetComponent<RectTransform>().sizeDelta.y) * index));

            aButton.transform.SetParent(panel.transform, false);
            aButton.GetComponent<Button>().onClick.AddListener(decision.myEventType);
            TextMeshProUGUI text = aButton.GetComponentInChildren<TextMeshProUGUI>();
            text.SetText(decision.DecisionName);
            index++;
        }
        panel.SetActive(false);
        hasBeenCreated = true;
    }

    public void Activate()
    {
        if(!hasBeenCreated)
        {
            Create();
        }
        panel.SetActive(true);
    }

    public void SetCanvasAndButton(GameObject aCanvas, GameObject aButton, GameObject aPanel)
    {
        canvas = aCanvas;
        button = aButton;
        panel = aPanel;
    }

    public void DeActivate()
    {
        panel.SetActive(false);
    }
}
