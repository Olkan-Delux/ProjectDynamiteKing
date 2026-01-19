using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VariableEditor : EditorWindow
{
    public enum VariablePages
    {
        AgeGroups,
        Characteristics,
        Jobbs,
    }
    private const string OriginalAssetPath = "Assets/Scriptable Objects/DataScriptableObject.asset";
    public DataScriptableObject myData;
    private VariablePages myCurrentPage = VariablePages.AgeGroups;
    private Vector2 ageGroupScrollPos;
    private Vector2 characteristicScrollPos;
    private Vector2 JobbScrollPos;
    private AgeGroup mySelectedAgeGroup = null;
    private Characteristic mySelectedCharacteristic = null;
    private Jobb mySelectedJobb = null;
    private int mySelectedJobbIndex = 0;
    private string[] tabNames;


    [MenuItem("Window/Variable Editor Window")]
    public static void ShowWindow()
    {
        GetWindow<VariableEditor>("Variable Editor");
    }

    public void OnEnable()
    {
        myData = AssetDatabase.LoadAssetAtPath<DataScriptableObject>(OriginalAssetPath);
        tabNames = System.Enum.GetNames(typeof(VariablePages));
    }

    void OnDisable()
    {
        if (myData == null)
            return;

        EditorUtility.SetDirty(myData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void OnGUI()
    {
        DrawTabs();
        GUILayout.Space(10);
        switch (myCurrentPage)
        {
            case VariablePages.AgeGroups:
                {
                    AgeGroupPage();
                    break;
                }
            case VariablePages.Characteristics:
                {
                    CharacteristicsPage();
                    break;
                }
            case VariablePages.Jobbs:
                {
                    JobbPage();
                    break;
                }
        }
    }

    void DrawTabs()
    {
        myCurrentPage = (VariablePages)GUILayout.Toolbar((int)myCurrentPage, tabNames);
    }

    public void JobbPage()
    {
        Color oldColor = GUI.backgroundColor;
        GUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Jobbs", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Add Jobb"))
        {
            myData.Jobbs.Add(new Jobb());
        }
        GUI.backgroundColor = oldColor;
        JobbScrollPos = EditorGUILayout.BeginScrollView(JobbScrollPos, EditorStyles.helpBox);
        for (int i = 0; i < myData.Jobbs.Count; i++)
        {
            if (myData.Jobbs[i] == mySelectedJobb)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
            }
            if (GUILayout.Button(myData.Jobbs[i].myJob))
            {
                mySelectedJobb = myData.Jobbs[i];
            }
            if (myData.Jobbs[i] == mySelectedJobb)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Jobb Name: ");
                mySelectedJobb.myJob = EditorGUILayout.TextField(mySelectedJobb.myJob);
                GUILayout.EndHorizontal();
                mySelectedJobb.myBaseLowSalary = EditorGUILayout.IntField("Jobb low Salary", mySelectedJobb.myBaseLowSalary);
                mySelectedJobb.myBaseHighSalary = EditorGUILayout.IntField("Jobb High Salary", mySelectedJobb.myBaseHighSalary);

                mySelectedJobb.myAcceptanceRate = EditorGUILayout.FloatField("Jobb Acceptance Rate (%)", mySelectedJobb.myAcceptanceRate);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Has sequence Job (Apprentice)");
                mySelectedJobb.HasSequenceJob = EditorGUILayout.Toggle(mySelectedJobb.HasSequenceJob);
                GUILayout.EndHorizontal();
                if(mySelectedJobb.HasSequenceJob)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    string[] Jobbs = myData.Jobbs.ConvertAll(p => p.myJob).ToArray();
                    mySelectedJobbIndex = EditorGUILayout.Popup(mySelectedJobbIndex, Jobbs);
                    mySelectedJobb.myPromotionalJob = myData.Jobbs[mySelectedJobbIndex];
                    GUILayout.BeginHorizontal();
                    mySelectedJobb.canGetPromoted = EditorGUILayout.Toggle("Can Get Promoted", mySelectedJobb.canGetPromoted);
                    if(mySelectedJobb.canGetPromoted)
                    {
                        mySelectedJobb.MyYearsTilPromotion = EditorGUILayout.IntField("Years Til Promotion", mySelectedJobb.MyYearsTilPromotion);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
                mySelectedJobb.IsVisibleInJobMenu = EditorGUILayout.Toggle("Is Visible In JobMenu", mySelectedJobb.IsVisibleInJobMenu);
                mySelectedJobb.deathChanceIncrease = EditorGUILayout.FloatField("Death Chance Increase", mySelectedJobb.deathChanceIncrease);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Can Characters Randomly Get");
                mySelectedJobb.CanCharactersRandomlyGet = EditorGUILayout.Toggle(mySelectedJobb.CanCharactersRandomlyGet);
                GUILayout.EndHorizontal();
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Delete " + myData.Jobbs[i].myJob))
                {
                    myData.Jobbs.RemoveAt(i);
                }
                GUI.backgroundColor = oldColor;
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    public void AgeGroupPage()
    {
        Color oldColor = GUI.backgroundColor;
        GUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("AgrGroups", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Add Age Group"))
        {
            myData.AgeGroups.Add(new AgeGroup());
        }
        GUI.backgroundColor = oldColor;
        ageGroupScrollPos = EditorGUILayout.BeginScrollView(ageGroupScrollPos, EditorStyles.helpBox);
        for (int i = 0; i < myData.AgeGroups.Count; i++)
        {
            if (myData.AgeGroups[i] == mySelectedAgeGroup)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
            }
            if (GUILayout.Button(myData.AgeGroups[i].myAgeGroupName))
            {
                mySelectedAgeGroup = myData.AgeGroups[i];
            }
            if(myData.AgeGroups[i] == mySelectedAgeGroup)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Age Group Name: ");
                mySelectedAgeGroup.myAgeGroupName = EditorGUILayout.TextField(mySelectedAgeGroup.myAgeGroupName);
                GUILayout.EndHorizontal();
                mySelectedAgeGroup.myAgeGroupLowAge = EditorGUILayout.IntField("Age Group Start age", mySelectedAgeGroup.myAgeGroupLowAge);
                mySelectedAgeGroup.myAgeGroupTopAge = EditorGUILayout.IntField("Age Group end age", mySelectedAgeGroup.myAgeGroupTopAge);
                mySelectedAgeGroup.myChildChance = EditorGUILayout.FloatField("Age Group Child Chance per Year", mySelectedAgeGroup.myChildChance);
                mySelectedAgeGroup.myDeathChance = EditorGUILayout.FloatField("Age Group Death Chance per Year", mySelectedAgeGroup.myDeathChance);
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Delete " + myData.AgeGroups[i].myAgeGroupName))
                {
                    myData.AgeGroups.RemoveAt(i);
                }
                GUI.backgroundColor = oldColor;
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    public void CharacteristicsPage()
    {
        Color oldColor = GUI.backgroundColor;
        GUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Characteristics", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Add Characteristic"))
        {
            myData.characteristics.Add(new Characteristic());
        }
        GUI.backgroundColor = oldColor;
        characteristicScrollPos = EditorGUILayout.BeginScrollView(characteristicScrollPos, EditorStyles.helpBox);
        for (int i = 0; i < myData.characteristics.Count; i++)
        {
            if (myData.characteristics[i] == mySelectedCharacteristic)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
            }
            if (GUILayout.Button(myData.characteristics[i].myCharacteristic))
            {
                mySelectedCharacteristic = myData.characteristics[i];
            }
            if (myData.characteristics[i] == mySelectedCharacteristic)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Characteristic Name: ");
                mySelectedCharacteristic.myCharacteristic = EditorGUILayout.TextField(mySelectedCharacteristic.myCharacteristic);
                GUILayout.EndHorizontal();
                mySelectedCharacteristic.myInheritanceChance = EditorGUILayout.FloatField("Characteristic Inheritance Chance", mySelectedCharacteristic.myInheritanceChance);
                mySelectedCharacteristic.myDevelopmentChance = EditorGUILayout.FloatField("Characteristic Development Chance", mySelectedCharacteristic.myInheritanceChance);
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Delete " + myData.characteristics[i].myCharacteristic))
                {
                    myData.characteristics.RemoveAt(i);
                }
                GUI.backgroundColor = oldColor;
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
}
