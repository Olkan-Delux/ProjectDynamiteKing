using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NameEditor : EditorWindow
{
    private const string NameDataPath = "Assets/Scriptable Objects/NameScriptableObject.asset";
    public NameScriptableObject myNames;
    public List<NameRegion> myRegions = new List<NameRegion>();
    private string newRegionName = "";
    private string newName = "";
    private Vector2 scrollPos;
    private Vector2 scrollPosMale;
    private Vector2 scrollPosFemale;
    bool isInRegionMenu = false;
    private NameRegion selectedRegion;
    private int mySelectedMaleName = -1;
    private int mySelectedFemaleName = -1;
    private GameHub.Gender selectedGender = GameHub.Gender.Boy;
    private string[] myRegionDeleteWarningMessages = new string[] { "Delete", "Are you sure?", "Last warning, Are you really sure?" };  
    private int myRegionDeleteStatus = 0;
    private bool myRegionDeleteToggle = false;
    private bool shouldNullRegion = false;

    [MenuItem("Window/Name Window")]
    public static void ShowWindow()
    {
        GetWindow<NameEditor>("Name Window");
    }
    public void OnEnable()
    {
        myNames = AssetDatabase.LoadAssetAtPath<NameScriptableObject>(NameDataPath);
        myRegions = myNames.myRegions;
    }

    public void OnDisable()
    {
        Save();
    }

    public void OnGUI()
    {
        if(!isInRegionMenu)
        {
            RegionOverviewMenu();
        }
        else 
        {
            RegionMenu();
        }

    }

    void RegionOverviewMenu()
    {
        Color oldColor = GUI.backgroundColor;
        EditorGUILayout.LabelField("Regions", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.helpBox);
        for (int i = 0; i < myRegions.Count; i++)
        {
            if (i % 2 == 0)
            {
                GUI.backgroundColor = Color.gray;
            }
            if (GUILayout.Button(myRegions[i].myRegion))
            {
                isInRegionMenu = true;
                selectedRegion = myRegions[i];
            }
            GUI.backgroundColor = oldColor;

        }
        EditorGUILayout.EndScrollView();
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Create New Region"))
        {
            NameRegion newNameRegion = new NameRegion();
            newNameRegion.myRegion = newRegionName;
            newNameRegion.MaleNames = new List<string>();
            newNameRegion.FemaleNames = new List<string>();
            myRegions.Add(newNameRegion);
            newRegionName = "";
            Save();
        }
        GUI.backgroundColor = oldColor;
        newRegionName = EditorGUILayout.TextField("Region Name:", newRegionName);
        GUILayout.EndVertical();
    }

    void RegionMenu()
    {
        Color oldColor = GUI.backgroundColor;
        shouldNullRegion = false;
        EditorGUILayout.BeginHorizontal();
        scrollPosMale = EditorGUILayout.BeginScrollView(scrollPosMale, EditorStyles.helpBox);
        for (int i = 0; i < selectedRegion.MaleNames.Count; i++)
        {
            if(i%2 == 0)
            {
                GUI.backgroundColor = Color.gray;
            }
            if (i == mySelectedMaleName)
            {
                GUI.backgroundColor = Color.blue;
            }
            if (GUILayout.Button(selectedRegion.MaleNames[i]))
            {
                mySelectedMaleName = i;
            }
            if (i == mySelectedMaleName)
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Delete " + selectedRegion.MaleNames[i]))
                {
                    selectedRegion.MaleNames.RemoveAt(i);
                    Save();
                }
            }
            GUI.backgroundColor = oldColor;
        }
        EditorGUILayout.EndScrollView();
        scrollPosFemale = EditorGUILayout.BeginScrollView(scrollPosFemale, EditorStyles.helpBox);
        for (int i = 0; i < selectedRegion.FemaleNames.Count; i++)
        {
            if (i % 2 == 1)
            {
                GUI.backgroundColor = Color.gray;
            }
            if (i == mySelectedFemaleName)
            {
                GUI.backgroundColor = Color.blue;
            }
            if (GUILayout.Button(selectedRegion.FemaleNames[i]))
            {
                mySelectedFemaleName = i;
            }
            if (i == mySelectedFemaleName)
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Delete " + selectedRegion.FemaleNames[i]))
                {
                    selectedRegion.FemaleNames.RemoveAt(i);
                    Save();
                }
            }
            GUI.backgroundColor = oldColor;
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Create new Name"))
        {
            if(selectedGender == GameHub.Gender.Boy)
            {
                string aNewName = newName;
                selectedRegion.MaleNames.Add(aNewName);
            }
            else
            {
                string aNewName = newName;
                selectedRegion.FemaleNames.Add(aNewName);
            }
            newName = "";
            Save();
        }
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.BeginHorizontal();
        selectedGender = (GameHub.Gender)EditorGUILayout.EnumPopup("Gender :", selectedGender);
        newName = EditorGUILayout.TextField("Name :", newName);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Exit " + selectedRegion.myRegion + " Menu"))
        {
            shouldNullRegion = true;
            isInRegionMenu = false;
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("OBS! Deleting Region Will Delete all Names Connected to Region", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        myRegionDeleteToggle = GUILayout.Toggle(myRegionDeleteToggle, "I Want to Delete " + selectedRegion.myRegion);
        if (myRegionDeleteToggle)
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button(myRegionDeleteWarningMessages[myRegionDeleteStatus]))
            {
                myRegionDeleteStatus++;
                if (myRegionDeleteStatus > 2)
                {
                    myRegions.Remove(selectedRegion);
                    shouldNullRegion = true;
                    isInRegionMenu = false;
                    myRegionDeleteStatus = 0;
                    myRegionDeleteToggle = false;
                    Save();
                }
            }
            GUI.backgroundColor = oldColor;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        if(shouldNullRegion)
        {
            selectedRegion = null;
        }
    }

    public void Save()
    {
        myNames = AssetDatabase.LoadAssetAtPath<NameScriptableObject>(NameDataPath);
        myNames.myRegions = myRegions;
        EditorUtility.SetDirty(myNames);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
