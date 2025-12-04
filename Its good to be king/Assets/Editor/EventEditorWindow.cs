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
    public GameHub.Job selectedJobOption = GameHub.Job.Peasant;
    public GameHub.Job DependableJobOption = GameHub.Job.Peasant;
    public GameHub.RelationType selectedRelationOption = GameHub.RelationType.Stranger;
    public GameHub.RelationType selectedRelationDependable = GameHub.RelationType.Stranger;
    public int relationAmount = 1;
    public int Age = 10;
    public EventScriptableObject.AgeRequierment myAgeRequierment;
    public bool JobDependant;
    public bool AgeDependant;
    public int DependableCharacterAge = 10;
    public EventScriptableObject.AgeRequierment myDependableAgeRequierment;
    public bool DependableCharacterFlag = false;
    public bool RelationJobDependant = false;
    public bool RelationAgeDependant = false;
    public bool CanBeGottenAgain = false;
    public float ChanceOfHappening = 0.0f;

    public string EventTitle = "";
    public string EventText = "";
    public List<bool> HasSecondEvent = new List<bool>();
    public List<string> buttonTexts = new List<string>();
    public List<string> buttonResultEventText = new List<string>();
    public List<string> buttonResultEventTitle = new List<string>();
    public List<string> buttonResultButtonText = new List<string>();
    public List<ResultDataRegistry> buttonResults = new List<ResultDataRegistry>();
    public int ButtonNumber = 0;



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
                buttonResultEventText.Add("");
                buttonResultEventTitle.Add("");
                buttonResultButtonText.Add("");
                HasSecondEvent.Add(false);
                buttonResults.Add(new ResultDataRegistry());
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
            myAgeRequierment = (EventScriptableObject.AgeRequierment)EditorGUILayout.EnumPopup("Age Requierment:", myAgeRequierment);
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
            myAsset.buttonResultEventTitle = buttonResultEventTitle;
            myAsset.buttonResultEventText = buttonResultEventText;
            myAsset.buttonResultButtonText = buttonResultButtonText;
            myAsset.myAgeRequierment = myAgeRequierment;
            myAsset.myDependableAgeRequierment = myDependableAgeRequierment;
            myAsset.ChanceOfHappening = ChanceOfHappening;
            myAsset.HasSecondEvent = HasSecondEvent;

            myAssetRegistry.Events.Add(myAsset);
            EditorUtility.SetDirty(myAssetRegistry);
            EditorUtility.SetDirty(myAsset);
            AssetDatabase.SaveAssets();

            selectedJobOption = GameHub.Job.Peasant;
            DependableJobOption = GameHub.Job.Peasant;
            selectedRelationOption = GameHub.RelationType.Stranger;
            selectedRelationDependable = GameHub.RelationType.Stranger;
            relationAmount = 1;
            JobDependant = false;
            AgeDependant = false;
            DependableCharacterFlag = false;
            RelationJobDependant = false;
            RelationAgeDependant = false;
            CanBeGottenAgain = false;
            DependableCharacterAge = 10;
            ChanceOfHappening = 0f;
            Age = 10;
            EventTitle = "";
            EventText = "";
            buttonTexts.Clear();
            buttonResults.Clear();
            buttonResultEventText.Clear();
            buttonResultEventTitle.Clear();
            buttonResultButtonText.Clear();
            HasSecondEvent.Clear();
            ButtonNumber = 0;

        }
        GUI.backgroundColor = oldColor;
    }

    void DrawButton(int buttonIndex)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        buttonTexts[buttonIndex] = EditorGUILayout.TextField("Button Text:", buttonTexts[buttonIndex]);
        HasSecondEvent[buttonIndex] = EditorGUILayout.Toggle("Has Result Event:", HasSecondEvent[buttonIndex]);
        if(HasSecondEvent[buttonIndex])
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            buttonResultEventText[buttonIndex] = EditorGUILayout.TextField("Result Event Title:", buttonResultEventText[buttonIndex]);
            buttonResultEventTitle[buttonIndex] = EditorGUILayout.TextField("Result Event Text:", buttonResultEventTitle[buttonIndex]);
            buttonResultButtonText[buttonIndex] = EditorGUILayout.TextField("Result Event Button Text:", buttonResultButtonText[buttonIndex]);
            EditorGUILayout.EndVertical();
        }
        Color oldColor = GUI.backgroundColor;
        if (GUILayout.Button("Add Result"))
        {
            buttonResults[buttonIndex].results.Add(new ResultData());
        }
        for(int i = 0; i < buttonResults[buttonIndex].results.Count; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Result nr: " + (i+1).ToString());
            buttonResults[buttonIndex].results[i].myResult = (GameHub.EventResult)EditorGUILayout.EnumPopup("Result Type:", buttonResults[buttonIndex].results[i].myResult);
            switch(buttonResults[buttonIndex].results[i].myResult)
            {
                case GameHub.EventResult.Death:
                    {
                        buttonResults[buttonIndex].results[i].myRelationType = (GameHub.RelationType)EditorGUILayout.EnumPopup("Relation To Die:", buttonResults[buttonIndex].results[i].myRelationType);
                        break;
                    }
                case GameHub.EventResult.Money:
                    {
                        buttonResults[buttonIndex].results[i].myMoney = EditorGUILayout.IntSlider("Money Gotten:", buttonResults[buttonIndex].results[i].myMoney, -1000000, 1000000);
                        break;
                    }
                case GameHub.EventResult.Income:
                    {
                        buttonResults[buttonIndex].results[i].myMoney = EditorGUILayout.IntSlider("Income Gotten:", buttonResults[buttonIndex].results[i].myMoney, -10000, 10000);
                        break;
                    }
                case GameHub.EventResult.Land:
                    {
                        break;
                    }
                case GameHub.EventResult.Job:
                    {
                        buttonResults[buttonIndex].results[i].myJob = (GameHub.Job)EditorGUILayout.EnumPopup("Job Gotten:", buttonResults[buttonIndex].results[i].myJob);
                        break;
                    }
                case GameHub.EventResult.Character:
                    {
                        GUILayout.Label("Character Creation");
                        EditorGUILayout.BeginHorizontal();
                        buttonResults[buttonIndex].results[i].ShouldHaveJob = EditorGUILayout.Toggle("Should Have job", buttonResults[buttonIndex].results[i].ShouldHaveJob);
                        if(buttonResults[buttonIndex].results[i].ShouldHaveJob)
                        {
                            buttonResults[buttonIndex].results[i].RandomizeJob = EditorGUILayout.Toggle("Randomize Job", buttonResults[buttonIndex].results[i].RandomizeJob);
                            if(buttonResults[buttonIndex].results[i].RandomizeJob == false)
                            {
                                buttonResults[buttonIndex].results[i].CharacterJob = (GameHub.Job)EditorGUILayout.EnumPopup("Job:", buttonResults[buttonIndex].results[i].CharacterJob);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        buttonResults[buttonIndex].results[i].RandomizeAge = EditorGUILayout.Toggle("Randomize Age", buttonResults[buttonIndex].results[i].RandomizeAge);
                        if(buttonResults[buttonIndex].results[i].RandomizeAge == false)
                        {
                            buttonResults[buttonIndex].results[i].CharacterAge = EditorGUILayout.IntSlider("Age:", buttonResults[buttonIndex].results[i].CharacterAge, 0, 100);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        buttonResults[buttonIndex].results[i].CharacterRelation = (GameHub.RelationType)EditorGUILayout.EnumPopup("Relation Type:", buttonResults[buttonIndex].results[i].CharacterRelation);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();

                        buttonResults[buttonIndex].results[i].RandomizeGender = EditorGUILayout.Toggle("Randomize Gender", buttonResults[buttonIndex].results[i].RandomizeGender);
                        buttonResults[buttonIndex].results[i].OppositeGender = EditorGUILayout.Toggle("Is Opposite Gender of Player", buttonResults[buttonIndex].results[i].OppositeGender);
                        if(buttonResults[buttonIndex].results[i].RandomizeGender == false && buttonResults[buttonIndex].results[i].OppositeGender == false)
                        {
                            buttonResults[buttonIndex].results[i].CharacterGender = (GameHub.Gender)EditorGUILayout.EnumPopup("Character Gender:", buttonResults[buttonIndex].results[i].CharacterGender);
                        }
                        EditorGUILayout.EndHorizontal();

                        break;
                    }
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Remove Result"))
            {
                buttonResults[buttonIndex].results.RemoveAt(i);
            }
            GUI.backgroundColor = oldColor;
            EditorGUILayout.EndVertical();
        }


        GUI.backgroundColor = Color.red;
        if(GUILayout.Button("Remove Button"))
        {
            buttonTexts.RemoveAt(buttonIndex);
            buttonResults.RemoveAt(buttonIndex);
            buttonResultButtonText.RemoveAt(buttonIndex);
            buttonResultEventTitle.RemoveAt(buttonIndex);
            buttonResultEventText.RemoveAt(buttonIndex);
            HasSecondEvent.RemoveAt(buttonIndex);
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
            myDependableAgeRequierment = (EventScriptableObject.AgeRequierment)EditorGUILayout.EnumPopup("Dependable Age Requierment:", myDependableAgeRequierment);
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
