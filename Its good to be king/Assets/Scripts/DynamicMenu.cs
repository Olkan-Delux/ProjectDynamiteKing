using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public struct ButtonStruct
{
    public GameObject myButton;
    public int myPriority;
    public string myName;
}

public class DynamicMenu : MonoBehaviour
{
    public GameObject MenuButton;
    public float distanceBetweenButtons = 200;
    public List<ButtonStruct> myButtons;

    public void Create(GameObject aButton)
    {
        myButtons = new List<ButtonStruct>();
        MenuButton = aButton;
        CloseMenu(false);
    }

    public GameObject AddButton(UnityEngine.UI.Button.ButtonClickedEvent aButtonFUnction, string buttonName, int Priority)
    {
        ButtonStruct buttonstruct = new ButtonStruct();
        GameObject aButton = Instantiate(MenuButton);
        aButton.transform.SetParent(gameObject.transform.GetChild(0).transform, false);
        aButton.GetComponent<Button>().onClick = aButtonFUnction;
        TextMeshProUGUI text = aButton.GetComponentInChildren<TextMeshProUGUI>();
        text.SetText(buttonName);
        buttonstruct.myName = buttonName;
        buttonstruct.myPriority = Priority;
        buttonstruct.myButton = aButton;
        myButtons.Add(buttonstruct);
        text.ForceMeshUpdate();
        
        ArrangeMenu();
        return aButton;
    }

    public GameObject CreateText(GameObject aButton, Vector2 aPos, string atext)
    {
        GameObject text = Instantiate(aButton.gameObject.transform.GetChild(0).gameObject);
        TextMeshProUGUI tmPro = text.GetComponent<TextMeshProUGUI>();
        text.transform.SetParent(aButton.transform, false);
        tmPro.SetText(atext);
        text.GetComponent<RectTransform>().localPosition = aPos;
        return text;
    }

    public void RemoveButton(string buttonName)
    {
        int index = 0;
        foreach(ButtonStruct button in myButtons)
        {
            if(button.myName == buttonName)
            {
                Destroy(button.myButton);
                break;
            }
            index++;
        }
        myButtons.Remove(myButtons[index]);
    }

    public void RemoveAllButtons()
    {
        foreach (ButtonStruct button in myButtons)
        {
            Destroy(button.myButton);
        }
        myButtons.Clear();
    }

    public void OpenMenu(bool withAnimation)
    {
        gameObject.GetComponent<UIMenu>().Open(withAnimation);
        ArrangeMenu();
    }

    public void SetFront()
    {
        GetComponent<RectTransform>().SetAsLastSibling();
    }

    public void CloseMenu(bool withAnimation)
    {
        gameObject.GetComponent<UIMenu>().Close(withAnimation);
    }

    public void ArrangeMenu()
    {
        myButtons.Sort(SortByPriority);
        int index = 0;
        foreach(ButtonStruct button in myButtons)
        {
            button.myButton.GetComponent<RectTransform>().localPosition = MenuButton.GetComponent<RectTransform>().position - new Vector3(0, (((button.myButton.GetComponent<RectTransform>().sizeDelta.y * 0.5f) + distanceBetweenButtons) * index), 0);
            index++;
        }
    }

    static int SortByPriority(ButtonStruct event1, ButtonStruct event2)
    {
        return event1.myPriority.CompareTo(event2.myPriority);
    }

    public void SetOnExit(UnityAction anEvent)
    {
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(anEvent);
    }
}
