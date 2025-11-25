using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class UIHub : MonoBehaviour
{
    public enum Menus
    {
        Relations,
        JobMenu,
        Peasant,
        Smith,
        Fisher,
        Baker,
        ShoeMaker,
        Carpenter,
        Merchant,
        Hunter,
        Miner,
        Mercenary,
        Guard,
        Sailor,
        Soldier,
        Monk,
        Priest,
        Bishop,
        Knight,
        Noblemen,
        King,
        Child,
        Wife,
        Illigal,
        War,
        Religion,
        FindLove,
        Father,
        Mother,
        Sibling,
        Stranger,
        Count
    }

    public TextMeshProUGUI age;
    public TextMeshProUGUI Money;
    public TextMeshProUGUI Job;
    public TextMeshProUGUI Name;
    public Transform myCamera;
    public GameObject OverviewMenu;
    public GameObject ActionsMenu;
    public GameObject MenuPanel;
    public GameObject MenuButton;
    public GameObject DebugObject;
    private LandUI myLandUI;
    private GameObject[] myMenus;

    private static UIHub instance;
    public static UIHub Instance { get { return instance; } }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        myMenus = new GameObject[(int)Menus.Count];
        Workshop();
        CreateMenus();
    }


    public void OpenMenu(int index)
    {
        myMenus[index].SetActive(true);
        myMenus[index].GetComponent<DynamicMenu>().OpenMenu(true);
    }

    public GameObject GetMenu(Menus aMenu)
    {
        return myMenus[(int)aMenu];
    }

    public void UpdateUI()
    {
        Character playerCharacter = GameHub.Instance.GetPlayer().GetPlayerCharacter();
        age.SetText("Age : " + playerCharacter.myAge);
        Money.SetText("G : " + playerCharacter.myMoney.Gold + ", S : " + playerCharacter.myMoney.Silver + ", C : " + playerCharacter.myMoney.Copper);
        Name.SetText(playerCharacter.myName);
        Job.SetText(System.Enum.GetName(typeof(GameHub.Job), playerCharacter.myJob));
    }

    public void CloseMenus()
    {
        foreach(GameObject gameobject in myMenus)
        {
            gameobject.GetComponent<UIMenu>().Close(false);
        }
        OverviewMenu.GetComponent<UIMenu>().Close(false);
        ActionsMenu.GetComponent<UIMenu>().Close(false);
    }
    private GameObject CreateMenu(Menus myType)
    {
        GameObject panel = Instantiate(MenuPanel);
        panel.gameObject.transform.SetParent(gameObject.transform, false);
        panel.GetComponent<UIMenu>().CurrentPos = panel.transform.position;
        panel.GetComponent<UIMenu>().CameraPos = myCamera;
        panel.AddComponent<DynamicMenu>().Create(MenuButton);
        myMenus[(int)myType] = panel;
        return panel;
    }

    public LandUI GetLandUI()
    {
        return myLandUI;
    }

    private void Workshop()
    {
        CreateMenu(Menus.Relations);
        CreateMenu(Menus.JobMenu);
        CreateMenu(Menus.Peasant);
        CreateMenu(Menus.Smith);
        CreateMenu(Menus.Fisher);
        CreateMenu(Menus.Baker);
        CreateMenu(Menus.ShoeMaker);
        CreateMenu(Menus.Carpenter);
        CreateMenu(Menus.Merchant);
        CreateMenu(Menus.Hunter);
        CreateMenu(Menus.Miner);
        CreateMenu(Menus.Mercenary);
        CreateMenu(Menus.Guard);
        CreateMenu(Menus.Sailor);
        CreateMenu(Menus.Soldier);
        CreateMenu(Menus.Monk);
        CreateMenu(Menus.Priest);
        CreateMenu(Menus.Bishop);
        CreateMenu(Menus.Knight);
        CreateMenu(Menus.Noblemen);
        CreateMenu(Menus.King);
        CreateMenu(Menus.Child);
        CreateMenu(Menus.Wife);
        CreateMenu(Menus.Illigal);
        CreateMenu(Menus.War);
        CreateMenu(Menus.Religion);
        CreateMenu(Menus.FindLove);
        CreateMenu(Menus.Father);
        CreateMenu(Menus.Mother);
        CreateMenu(Menus.Sibling);
        CreateMenu(Menus.Stranger);
        myLandUI = gameObject.GetComponent<LandUI>();
        myLandUI.Create();
        myLandUI.CloseMenu();
    }

    private void CreateMenus()
    {
        UnityAction action = () => {
            PlayerHub player = GameHub.Instance.GetPlayer();
            player.GetRelationShipMenu().ClearRelations();
            player.GetRelationShipMenu().DrawRelations(player.GetPlayerCharacter());
            GetMenu(Menus.Relations).SetActive(true);
            GetMenu(Menus.Relations).GetComponent<UIMenu>().Open(false);
        };

        UnityEngine.UI.Button.ButtonClickedEvent relationsMenu = new UnityEngine.UI.Button.ButtonClickedEvent();
        relationsMenu.AddListener(() =>
        {
            GetMenu(0).GetComponent<DynamicMenu>().CloseMenu(false);
            PlayerHub player = GameHub.Instance.GetPlayer();
            player.GetRelationShipMenu().ClearRelations();
            player.GetRelationShipMenu().DrawRelations(player.GetCurrentInteraction());
            OpenMenu(0);
        });

        {
            DynamicMenu WifeMenu = myMenus[(int)Menus.Wife].GetComponent<DynamicMenu>();
            UnityEngine.UI.Button.ButtonClickedEvent clickEvent = new UnityEngine.UI.Button.ButtonClickedEvent();
            clickEvent.AddListener(() =>
            {
                GameHub.Instance.ActivateEvent((int)GameHub.EventType.Child);
            });
            WifeMenu.SetOnExit(action);
            WifeMenu.AddButton(clickEvent, "Try For Child", 1);
            WifeMenu.AddButton(relationsMenu,"Relations",2);
        }

        {
            DynamicMenu strangerMenu = myMenus[(int)Menus.Stranger].GetComponent<DynamicMenu>();
            strangerMenu.SetOnExit(action);
            strangerMenu.AddButton(relationsMenu, "Relations", 2);
        }

        {
            DynamicMenu ChildMenu = myMenus[(int)Menus.Child].GetComponent<DynamicMenu>();
            UnityEngine.UI.Button.ButtonClickedEvent clickEvent = new UnityEngine.UI.Button.ButtonClickedEvent();
            clickEvent.AddListener(() => {
                GameHub.Instance.GetPlayer().SetHeir(GameHub.Instance.GetPlayer().GetCurrentInteraction());
            });
            ChildMenu.SetOnExit(action);
            ChildMenu.AddButton(clickEvent, "Chose As Heir", 1);
            ChildMenu.AddButton(relationsMenu, "Relations", 2);
        }

        {
            DynamicMenu jobMenu = myMenus[(int)Menus.JobMenu].GetComponent<DynamicMenu>();
            for (int i = 0; i < (int)GameHub.Job.King; i++)
            {
                if(i != (int)GameHub.Job.Knight  || i != (int)GameHub.Job.Noblemen || i != (int)GameHub.Job.King || i != (int)GameHub.Job.Nothing || i !=(int)GameHub.Job.Bishop)
                {
                    int index = i;
                    UnityEngine.UI.Button.ButtonClickedEvent clickEvent = new UnityEngine.UI.Button.ButtonClickedEvent();
                    clickEvent.AddListener(() => {
                        GameHub.Instance.ActivateEvent((int)GameHub.Instance.GetEventFromJob((GameHub.Job)index));
                    });
                    jobMenu.AddButton(clickEvent, System.Enum.GetName(typeof(GameHub.Job), (GameHub.Job)i), i);
                }
            }


        }

    }

    public Menus GetMenuFromJob(GameHub.Job aJob)
    {
        switch(aJob)
        {
            case GameHub.Job.Baker:
                {
                    return Menus.Baker;
                }
            case GameHub.Job.Bishop:
                {
                    return Menus.Bishop;
                }
            case GameHub.Job.Carpenter:
                {
                    return Menus.Carpenter;
                }
            case GameHub.Job.Fisher:
                {
                    return Menus.Fisher;
                }
            case GameHub.Job.Guard:
                {
                    return Menus.Guard;
                }
            case GameHub.Job.Hunter:
                {
                    return Menus.Hunter;
                }
            case GameHub.Job.King:
                {
                    return Menus.King;
                }
            case GameHub.Job.Knight:
                {
                    return Menus.Knight;
                }
            case GameHub.Job.Mercenary:
                {
                    return Menus.Mercenary;
                }
            case GameHub.Job.Merchant:
                {
                    return Menus.Merchant;
                }
            case GameHub.Job.Miner:
                {
                    return Menus.Miner;
                }
            case GameHub.Job.Monk:
                {
                    return Menus.Monk;
                }
            case GameHub.Job.Noblemen:
                {
                    return Menus.Noblemen;
                }
            case GameHub.Job.Peasant:
                {
                    return Menus.Peasant;
                }
            case GameHub.Job.Priest:
                {
                    return Menus.Priest;
                }
            case GameHub.Job.Sailor:
                {
                    return Menus.Sailor;
                }
            case GameHub.Job.ShoeMaker:
                {
                    return Menus.ShoeMaker;
                }
            case GameHub.Job.Smith:
                {
                    return Menus.Smith;
                }
            case GameHub.Job.Soldier:
                {
                    return Menus.Soldier;
                }
        }

        return Menus.War;
    }

    public Menus GetMenuFromRelationType(GameHub.RelationType aRelationType)
    {
        switch (aRelationType)
        {
            case GameHub.RelationType.Wife:
                {
                    return Menus.Wife;
                }
            case GameHub.RelationType.Father:
                {
                    return Menus.Father;
                }
            case GameHub.RelationType.Mother:
                {
                    return Menus.Mother;
                }
            case GameHub.RelationType.Child:
                {
                    return Menus.Child;
                }
            case GameHub.RelationType.Sister:
                {
                    return Menus.Sibling;
                }
            case GameHub.RelationType.Brother:
                {
                    return Menus.Sibling;
                }
        }
        return Menus.War;
    }

    public void CreateDebugObject(Vector2 pos)
    {
        GameObject newGO = Instantiate(DebugObject);
        newGO.transform.position = pos;
    }

    public void CreateDebugObject(Vector2 pos, Color color, string name)
    {
        GameObject newGO = Instantiate(DebugObject);
        newGO.GetComponent<Renderer>().material.color = color;
        newGO.transform.position = new Vector3(pos.x, pos.y, -3.0f);
        newGO.name = name;
    }
}
