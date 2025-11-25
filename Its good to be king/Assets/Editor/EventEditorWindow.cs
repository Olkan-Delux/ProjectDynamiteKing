using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;


public class EventEditorWindow : EditorWindow
{
    private const string OriginalAssetPath = "Assets/Scriptable Objects/EventScriptableObject.asset";
    private const string RegistryAssetPath = "Assets/Scriptable Objects/EventDataRegistry.asset";
    private string assetPath = "";
    EventScriptableObject myAsset;
    EventRegistryScriptableObject myAssetRegistry;
    GameHub.Job selectedJobOption = GameHub.Job.Peasant; 
    GameHub.Job DependableJobOption = GameHub.Job.Peasant; 
    GameHub.RelationType selectedRelationOption = GameHub.RelationType.Stranger; 
    GameHub.RelationType selectedRelationDependable = GameHub.RelationType.Stranger; 
    int relationAmount = 1;
    int Age = 10;
    bool JobDependant;
    bool AgeDependant;
    int DependableCharacterAge = 10;
    bool DependableCharacterFlag = false;
    bool RelationJobDependant = false;
    bool RelationAgeDependant = false;
    bool CanBeGottenAgain = false;
    float ChanceOfHappening = 0.0f;

    private string EventTitle = "";
    private string EventText = "";
    private List<string> buttonTexts = new List<string>();
    private List<List<ResultData>> buttonResults = new List<List<ResultData>>();
    int ButtonNumber = 0;


    [MenuItem("Window/Event Editor Window")]
    public static void ShowWindow()
    {
        GetWindow<EventEditorWindow>("Event Editor Window");
    }

    void OnGUI()
    {
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EventTitle = EditorGUILayout.TextField("Event Title:", EventTitle);
        EventText = EditorGUILayout.TextField("Event Text:", EventText);
        if(ButtonNumber < 3)
        {
            if(GUILayout.Button("Add Button"))
            {
                ButtonNumber++;
                buttonTexts.Add("");
                buttonResults.Add(new List<ResultData>());
            }
        }
        for(int i = 0; i < ButtonNumber; i++)
        {
            DrawButton(i);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Event Dependables");
        EditorGUILayout.BeginHorizontal();
        JobDependant = EditorGUILayout.Toggle("Job Dependant:", JobDependant);
        if (JobDependant)
        {
            selectedJobOption = (GameHub.Job)EditorGUILayout.EnumPopup("Job Dropdown:", selectedJobOption);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        AgeDependant = EditorGUILayout.Toggle("Age Dependant:", AgeDependant);
        if (AgeDependant)
        {
            Age = EditorGUILayout.IntSlider("Age:", Age, 0, 100);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Chance of Happening per Year %");
        ChanceOfHappening = EditorGUILayout.Slider(ChanceOfHappening, 0, 100);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Event can Happen More Than Once");
        CanBeGottenAgain = EditorGUILayout.Toggle(CanBeGottenAgain);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Event requires other relation");
        DependableCharacterFlag = EditorGUILayout.Toggle(DependableCharacterFlag);
        EditorGUILayout.EndHorizontal();
        if (DependableCharacterFlag)
        {
            IsCharacterDependant();
        }
        EditorGUILayout.EndVertical();

        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.blue;
        if(GUILayout.Button("Save"))
        {
            myAssetRegistry = AssetDatabase.LoadAssetAtPath<EventRegistryScriptableObject>(RegistryAssetPath);

            assetPath = AssetDatabase.GenerateUniqueAssetPath(OriginalAssetPath);
            myAsset = ScriptableObject.CreateInstance<EventScriptableObject>();
            AssetDatabase.CreateAsset(myAsset, assetPath);
            AssetDatabase.SaveAssets();
            Debug.Log("Created new MyEditorData asset at " + assetPath);

            myAsset.selectedJobOption = selectedJobOption;
            myAsset.DependableJobOption = DependableJobOption;
            myAsset.selectedRelationDependable = selectedRelationDependable;
            myAsset.relationAmount = relationAmount;
            myAsset.DependableCharacterAge = DependableCharacterAge;
            myAsset.JobDependant = JobDependant;
            myAsset.AgeDependant = AgeDependant;
            myAsset.DependableCharacterFlag = DependableCharacterFlag;
            myAsset.RelationJobDependant = RelationJobDependant;
            myAsset.RelationAgeDependant = RelationAgeDependant;
            myAsset.CanBeGottenAgain = CanBeGottenAgain;
            myAsset.EventTitle = EventTitle;
            myAsset.EventText = EventText;
            myAsset.buttonTexts = buttonTexts;
            myAsset.buttonResults = buttonResults;

            myAssetRegistry.Events.Add(myAsset);
            EditorUtility.SetDirty(myAssetRegistry);
            EditorUtility.SetDirty(myAsset);
            AssetDatabase.SaveAssets();

        }
        GUI.backgroundColor = oldColor;
    }

    void DrawButton(int buttonIndex)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        buttonTexts[buttonIndex] = EditorGUILayout.TextField("Button Text:", buttonTexts[buttonIndex]);
        Color oldColor = GUI.backgroundColor;
        if (GUILayout.Button("Add Result"))
        {
            buttonResults[buttonIndex].Add(new ResultData());
        }
        for(int i = 0; i < buttonResults[buttonIndex].Count; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Result nr: " + (i+1).ToString());
            buttonResults[buttonIndex][i].myResult = (GameHub.EventResult)EditorGUILayout.EnumPopup("Result Type:", buttonResults[buttonIndex][i].myResult);
            switch(buttonResults[buttonIndex][i].myResult)
            {
                case GameHub.EventResult.Death:
                    {
                        buttonResults[buttonIndex][i].myRelationType = (GameHub.RelationType)EditorGUILayout.EnumPopup("Relation To Die:", buttonResults[buttonIndex][i].myRelationType);
                        break;
                    }
                case GameHub.EventResult.Money:
                    {
                        buttonResults[buttonIndex][i].myMoney = EditorGUILayout.IntSlider("Money Gotten:", buttonResults[buttonIndex][i].myMoney, -1000000, 1000000);
                        break;
                    }
                case GameHub.EventResult.Income:
                    {
                        buttonResults[buttonIndex][i].myMoney = EditorGUILayout.IntSlider("Income Gotten:", buttonResults[buttonIndex][i].myMoney, -10000, 10000);
                        break;
                    }
                case GameHub.EventResult.Land:
                    {
                        break;
                    }
                case GameHub.EventResult.Job:
                    {
                        buttonResults[buttonIndex][i].myJob = (GameHub.Job)EditorGUILayout.EnumPopup("Job Gotten:", buttonResults[buttonIndex][i].myJob);
                        break;
                    }
                case GameHub.EventResult.Wife:
                    {
                        break;
                    }
                case GameHub.EventResult.Child:
                    {
                        break;
                    }
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Remove Result"))
            {
                buttonResults[buttonIndex].RemoveAt(i);
            }
            GUI.backgroundColor = oldColor;
            EditorGUILayout.EndVertical();
        }


        GUI.backgroundColor = Color.red;
        if(GUILayout.Button("Remove Button"))
        {
            buttonTexts.RemoveAt(buttonIndex);
            buttonResults.RemoveAt(buttonIndex);
            ButtonNumber--;
        }
        GUI.backgroundColor = oldColor;
        EditorGUILayout.EndVertical();
    }

    void IsCharacterDependant()
    {
        EditorGUILayout.BeginHorizontal();

        selectedRelationDependable = (GameHub.RelationType)EditorGUILayout.EnumPopup("Dependable Character:", selectedRelationDependable);
        relationAmount = EditorGUILayout.IntSlider("Amount:", relationAmount, 1, 10, GUILayout.Width(200));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        RelationAgeDependant = EditorGUILayout.Toggle("Age Dependant:", RelationAgeDependant);
        if (RelationAgeDependant)
        {
            DependableCharacterAge = EditorGUILayout.IntSlider("Dependable Age:", DependableCharacterAge, 0, 100);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        RelationJobDependant = EditorGUILayout.Toggle("Job Dependant:", RelationJobDependant);
        if (RelationJobDependant)
        {
            DependableJobOption = (GameHub.Job)EditorGUILayout.EnumPopup("Job Dropdown:", DependableJobOption);
        }
        EditorGUILayout.EndHorizontal();
    }
}
