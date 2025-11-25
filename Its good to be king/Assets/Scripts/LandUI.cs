using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandUI : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject Button;
    private GameObject myMapMenu;
    // Start is called before the first frame update
    public void Create()
    {
        myMapMenu = Instantiate(MenuPanel);
        myMapMenu.gameObject.transform.SetParent(gameObject.transform, false);
        myMapMenu.GetComponent<UIMenu>().CurrentPos = myMapMenu.transform.position;
        myMapMenu.GetComponent<UIMenu>().CameraPos = Camera.main.transform;
        myMapMenu.AddComponent<DynamicMenu>().Create(Button);
    }

    public void Update()
    {
        if(myMapMenu.GetComponent<UIMenu>().GetIsClosed())
        {
            CloseMenu();
        }
    }

    public void OpenMenu(Map.CellData someCellData)
    {
        DynamicMenu menu = myMapMenu.GetComponent<DynamicMenu>();
        {
            UnityEngine.UI.Button.ButtonClickedEvent button = new UnityEngine.UI.Button.ButtonClickedEvent();
            button.AddListener(()=> { 
            
            });
            menu.AddButton(button, "Buy Land", 3);
        }
        {
            UnityEngine.UI.Button.ButtonClickedEvent button = new UnityEngine.UI.Button.ButtonClickedEvent();
            button.AddListener(() => {

            });
            menu.AddButton(button, "Buy Fort", 3);
        }
        {
            UnityEngine.UI.Button.ButtonClickedEvent button = new UnityEngine.UI.Button.ButtonClickedEvent();
            button.AddListener(() => {

            });
            menu.AddButton(button, "Buy Road", 3);
        }
        myMapMenu.SetActive(true);
        myMapMenu.GetComponent<DynamicMenu>().OpenMenu(false);
    }

    public void CloseMenu()
    {
        DynamicMenu menu = myMapMenu.GetComponent<DynamicMenu>();
        menu.RemoveAllButtons();
        menu.CloseMenu(false);
    }

    public GameObject GetMenu()
    {
        return myMapMenu;
    }
}
