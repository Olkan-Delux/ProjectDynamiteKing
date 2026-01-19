using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RelationsShips : MonoBehaviour
{
    public void DrawRelations(Character aCharacter)
    {
        aCharacter.UpdateOnClick();
        foreach(Relation relation in aCharacter.myRelations)
        {
            AddRelationToMenu(relation.relationType, relation.myRelation, aCharacter);
        }
    }

    public void AddRelationToMenu(GameHub.RelationType aRelationType, Character aCharater, Character aMenuOwner)
    {
        PlayerHub player = gameObject.GetComponent<PlayerHub>();
        UnityEngine.UI.Button.ButtonClickedEvent button = new UnityEngine.UI.Button.ButtonClickedEvent();
        if (aRelationType != GameHub.RelationType.Stranger && player.GetPlayerCharacter() == aMenuOwner)
        {
            button.AddListener(() => {
                player.SetInteraction(aCharater);
                UIHub.Instance.OpenMenu((int)UIHub.Instance.GetMenuFromRelationType(aRelationType));
            });
        }
        else
        {
            button.AddListener(() => {
                player.SetInteraction(aCharater);
                UIHub.Instance.OpenMenu((int)UIHub.Menus.Stranger);
            });
        }
        DynamicMenu menu = UIHub.Instance.GetMenu(UIHub.Menus.Relations).GetComponent<DynamicMenu>();
        //GameObject MenuButton = menu.AddButton(button, aCharater.myName + "", (int)aRelationType);
        //menu.CreateText(MenuButton, new Vector2(MenuButton.GetComponent<RectTransform>().localPosition.x + (MenuButton.GetComponent<RectTransform>().sizeDelta.x * 0.4f), MenuButton.GetComponent<RectTransform>().position.y),aCharater.myAge.ToString());
        //GameObject text;
        string characterJob;
        if (aCharater.myJob != null)
        {
            characterJob = aCharater.myJob.myJob;
        }
        else
        {
            characterJob = "";
        }
        string cleanText = aCharater.myName.ToString().Replace("\r", "");
        string buttonName = cleanText + ", " + aCharater.myAge.ToString() + ", " + System.Enum.GetName(typeof(GameHub.RelationType), aRelationType) + ", " + characterJob;
        GameObject MenuBottun = menu.AddButton(button, buttonName, (int)aRelationType);
        aCharater.myButtonName = buttonName;
        //if(aCharater.myJob != GameHub.Job.Nothing)
        //{
        //    text = menu.CreateText(MenuButton, new Vector2(MenuButton.GetComponent<RectTransform>().localPosition.x, MenuButton.GetComponent<RectTransform>().position.y - (MenuButton.GetComponent<RectTransform>().sizeDelta.y * 0.4f)), System.Enum.GetName(typeof(GameHub.Job),aCharater.myJob) + ", ");
        //}
        //else
        //{
        //    text = menu.CreateText(MenuButton, new Vector2(MenuButton.GetComponent<RectTransform>().localPosition.x, MenuButton.GetComponent<RectTransform>().position.y - (MenuButton.GetComponent<RectTransform>().sizeDelta.y * 0.4f)), "");
        //}
        //TextMeshProUGUI tmPro = text.GetComponent<TextMeshProUGUI>();
        //tmPro.SetText(tmPro.text + System.Enum.GetName(typeof(GameHub.RelationType), aRelationType));
        //tmPro.fontSize = 50;
    }

    public void UpdateRelationMenu()
    {
        PlayerHub player = gameObject.GetComponent<PlayerHub>();
        foreach (ButtonStruct button in UIHub.Instance.GetMenu(UIHub.Menus.Relations).GetComponent<DynamicMenu>().myButtons)
        {
            Relation character = player.GetPlayerCharacter().GetRelationFromButton(button.myName);
            string characterJob;

            if (character.myRelation.myJob != null)
            {
                characterJob = character.myRelation.myJob.myJob;
            }
            else
            {
                characterJob = "";
            }
            string cleanText = character.myRelation.myName.ToString().Replace("\r", "");
            string buttonName = cleanText + ", " + character.myRelation.myAge.ToString() + ", " + System.Enum.GetName(typeof(GameHub.RelationType), character.relationType) + ", " + characterJob;
            TextMeshProUGUI text = button.myButton.GetComponentInChildren<TextMeshProUGUI>();
            text.text = buttonName;
        }
    }

    public void RemoveRelationFromMenu(string CharacterName)
    {
        UIHub.Instance.GetMenu(UIHub.Menus.Relations).GetComponent<DynamicMenu>().RemoveButton(CharacterName);
    }

    public void ClearRelations()
    {
        UIHub.Instance.GetMenu(UIHub.Menus.Relations).GetComponent<DynamicMenu>().RemoveAllButtons();
    }
}
