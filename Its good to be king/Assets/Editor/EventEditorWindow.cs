using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;


public class EventEditorWindow : EditorWindow
{
    private const string OriginalAssetPath = "Assets/Scriptable Objects/EventScriptableObject.asset";
    private const string RegistryAssetPath = "Assets/Scriptable Objects/EventDataRegistry.asset";
    private const string DataAssetPath = "Assets/Scriptable Objects/DataScriptableObject.asset";
    private string assetPath = "";
    EventScriptableObject myAsset;
    EventRegistryScriptableObject myAssetRegistry;
    DataScriptableObject myDataScriptableObject;
    public GameHub.RelationType selectedRelationOption = GameHub.RelationType.Stranger;
    public int relationAmount = 1;
    public int Age = 10;
    public EventScriptableObject.AgeRequierment myAgeRequierment;
    public bool JobDependant;
    public bool AgeDependant;
    public bool DependableCharacterFlag = false;
    //public GameHub.Job selectedJobOption = GameHub.Job.Peasant;
    //public GameHub.Job DependableJobOption = GameHub.Job.Peasant;
    //public GameHub.RelationType selectedRelationDependable = GameHub.RelationType.Stranger;
    //public int DependableCharacterAge = 10;
    //public EventScriptableObject.AgeRequierment myDependableAgeRequierment;
    //public bool RelationJobDependant = false;
    //public bool RelationAgeDependant = false;
    //public int myDependableSelectedJob = 0;
    //public Jobb myDependableChosenJobb = null;
    public bool CanBeGottenAgain = false;
    public bool IsSocialClassDependant = true;
    public bool IsCharacteristicDependant = false;
    public Characteristic myChosenCharacteristic = null;
    public int mySelectedCharacteristic = 0;
    public Jobb myChosenJobb = null;
    public int mySelectedJobb = 0;

    public float ChanceOfHappening = 0.0f;
    public GameHub.SocialClass socialClass = GameHub.SocialClass.Commoner;

    public string EventTitle = "";
    public string EventText = "";
    //public List<bool> HasSecondEvent = new List<bool>();
    public List<string> buttonTexts = new List<string>();
    //public List<string> buttonResultEventText = new List<string>();
    //public List<string> buttonResultEventTitle = new List<string>();
    //public List<string> buttonResultButtonText = new List<string>();
    //public List<ResultDataRegistry> buttonResults = new List<ResultDataRegistry>();
    public List<ButtonAlternativeResultData> buttonAlternativeResults = new List<ButtonAlternativeResultData>();
    private List<Vector2> resultScrollPositions = new List<Vector2>();
    public int ButtonNumber = 0;

    public List<DependableData> myDependables = new List<DependableData>();

    public bool doesExistAlready = false;
    public string dataPath = "";
    public int selectedIndex = 0;

    private bool alreadyExists = false;

    //private Vector2 buttonScrollPos;
    private Vector2 dependableScrollPos;




    [MenuItem("Window/Event Editor Window")]
    public static void ShowWindow()
    {
        GetWindow<EventEditorWindow>("Event Editor Window");
    }

    private void OnEnable()
    {
        myDataScriptableObject = AssetDatabase.LoadAssetAtPath<DataScriptableObject>(DataAssetPath);
        if (myDataScriptableObject != null)
        {
            Debug.Log("Loaded: " + myDataScriptableObject.name);
        }
        else
        {
            Debug.LogError("Could not load ScriptableObject at path!");
        }
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
                buttonAlternativeResults.Add(new ButtonAlternativeResultData());
                //buttonAlternativeResults.Add(new ButtonAlternativeResults());
                //buttonResultEventText.Add("");
                //buttonResultEventTitle.Add("");
                //buttonResultButtonText.Add("");
                //HasSecondEvent.Add(false);
                //buttonResults.Add(new ResultDataRegistry());
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
        IsSocialClassDependant = EditorGUILayout.Toggle("Social Class Dependant:", IsSocialClassDependant);
        if (IsSocialClassDependant)
        {
            socialClass = (GameHub.SocialClass)EditorGUILayout.EnumPopup("Social Class:", socialClass);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        JobDependant = EditorGUILayout.Toggle("Job Dependant:", JobDependant);
        if (JobDependant)
        {
            if (myDataScriptableObject.Jobbs.Count > 0)
            {
                string[] jobs = myDataScriptableObject.Jobbs.ConvertAll(i => i.myJob).ToArray();
                mySelectedJobb = EditorGUILayout.Popup(mySelectedJobb, jobs);
                myChosenJobb = myDataScriptableObject.Jobbs[mySelectedJobb];
            }
            else
            {
                EditorGUILayout.LabelField("There are no Jobs currently. Please check the variable editor");
            }
            //selectedJobOption = (GameHub.Job)EditorGUILayout.EnumPopup("Job Dropdown:", selectedJobOption);
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
        IsCharacteristicDependant = EditorGUILayout.Toggle("Characteristic Dependant:", IsCharacteristicDependant);
        if (IsCharacteristicDependant)
        {
            if(myDataScriptableObject.characteristics.Count > 0)
            {
                string[] characteristics = myDataScriptableObject.characteristics.ConvertAll(i => i.myCharacteristic).ToArray();
                mySelectedCharacteristic = EditorGUILayout.Popup(mySelectedCharacteristic, characteristics);
                myChosenCharacteristic = myDataScriptableObject.characteristics[mySelectedCharacteristic];
            }
            else
            {
                EditorGUILayout.LabelField("There are no characteristics currently. Please check the variable editor");
            }
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
        CharacterDependables();
        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField("Event requires other relation");
        //DependableCharacterFlag = EditorGUILayout.Toggle(DependableCharacterFlag);
        //EditorGUILayout.EndHorizontal();
        //if (DependableCharacterFlag)
        //{
        //    IsCharacterDependant();
        //}
        EditorGUILayout.EndVertical();

        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.blue;
        if(GUILayout.Button("Save"))
        {
            myAssetRegistry = AssetDatabase.LoadAssetAtPath<EventRegistryScriptableObject>(RegistryAssetPath);

            if (doesExistAlready)
            {
                AssetDatabase.DeleteAsset(dataPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Deleted: " + dataPath);
                myAssetRegistry.Events.RemoveAt(selectedIndex);
            }
            else
            {
                alreadyExists = false;
                for (int i = 0; i < myAssetRegistry.Events.Count; i++)
                {
                    if (myAssetRegistry.Events[i].EventTitle == EventTitle)
                    {
                        alreadyExists = true;
                        break;
                    }
                }
            }

            if(alreadyExists || ChanceOfHappening == 0 || buttonTexts.Count == 0)
            {
                EditorUtility.DisplayDialog("Error!","Either this event already exists, or the chance of it happening is zero, or there isnt a result button \n " +
                                            "Im not gonna find that out for you though :-/", "I will change this");
                return;
            }


            assetPath = AssetDatabase.GenerateUniqueAssetPath(OriginalAssetPath);
            myAsset = ScriptableObject.CreateInstance<EventScriptableObject>();
            AssetDatabase.CreateAsset(myAsset, assetPath);
            AssetDatabase.SaveAssets();
            Debug.Log("Created new MyEditorData asset at " + assetPath);

            myAsset.selectedJobOption = myChosenJobb;
            myAsset.relationAmount = relationAmount;
            //myAsset.DependableJobOption = myDependableChosenJobb;
            //myAsset.selectedRelationDependable = selectedRelationDependable;
            //myAsset.DependableCharacterAge = DependableCharacterAge;
            //myAsset.DependableCharacterFlag = DependableCharacterFlag;
            //myAsset.RelationJobDependant = RelationJobDependant;
            //myAsset.RelationAgeDependant = RelationAgeDependant;
            myAsset.JobDependant = JobDependant;
            myAsset.AgeDependant = AgeDependant;
            myAsset.CanBeGottenAgain = CanBeGottenAgain;
            myAsset.EventTitle = EventTitle;
            myAsset.EventText = EventText;
            myAsset.buttonTexts = buttonTexts;
            //myAsset.buttonResults = buttonResults;
            //myAsset.buttonResultEventTitle = buttonResultEventTitle;
            //myAsset.buttonResultEventText = buttonResultEventText;
            //myAsset.buttonResultButtonText = buttonResultButtonText;
            //myAsset.myDependableAgeRequierment = myDependableAgeRequierment;
            //myAsset.HasSecondEvent = HasSecondEvent;
            myAsset.myButtonAlternatives = buttonAlternativeResults;
            myAsset.myAgeRequierment = myAgeRequierment;
            myAsset.ChanceOfHappening = ChanceOfHappening;
            myAsset.socialClass = socialClass;  
            myAsset.IsSocialClassDependant = IsSocialClassDependant;  
            myAsset.IsCharacteristicDependant = IsCharacteristicDependant;
            myAsset.myChosenCharacteristic = myChosenCharacteristic;
            myAsset.myDependables = myDependables;

            myAssetRegistry.Events.Add(myAsset);
            EditorUtility.SetDirty(myAssetRegistry);
            EditorUtility.SetDirty(myAsset);
            AssetDatabase.SaveAssets();

            myChosenJobb = null;
            //myDependableChosenJobb = null;
            mySelectedJobb = 0;
            //myDependableSelectedJob = 0;
            myChosenCharacteristic = null;
            IsCharacteristicDependant = false;
            socialClass = GameHub.SocialClass.Commoner;
            selectedRelationOption = GameHub.RelationType.Stranger;
            //selectedRelationDependable = GameHub.RelationType.Stranger;
            relationAmount = 1;
            JobDependant = false;
            AgeDependant = false;
            DependableCharacterFlag = false;
            //RelationJobDependant = false;
            //RelationAgeDependant = false;
            CanBeGottenAgain = false;
            doesExistAlready = false;
            //DependableCharacterAge = 10;
            ChanceOfHappening = 0f;
            Age = 10;
            EventTitle = "";
            EventText = "";
            buttonTexts.Clear();
            buttonAlternativeResults.Clear();
            //buttonResults.Clear();
            //buttonResultEventText.Clear();
            //buttonResultEventTitle.Clear();
            //buttonResultButtonText.Clear();
            //HasSecondEvent.Clear();
            myDependables.Clear();
            ButtonNumber = 0;
            alreadyExists = false;
        }
        GUI.backgroundColor = oldColor;
    }

    void DrawButton(int buttonIndex)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        buttonTexts[buttonIndex] = EditorGUILayout.TextField("Button Text:", buttonTexts[buttonIndex]);
        Color oldColor = GUI.backgroundColor;
        if (GUILayout.Button("Add Button Result"))
        {
            buttonAlternativeResults[buttonIndex].myButtonResults.Add(new ButtonAlternativeResults());
            //resultScrollPositions.Add(new Vector2());
        }

        EditorGUILayout.LabelField("Button Results");
        for (int y = 0; y < buttonAlternativeResults[buttonIndex].myButtonResults.Count; y++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Result: " + buttonAlternativeResults[buttonIndex].myButtonResults[y].resultEventTitle);
            buttonAlternativeResults[buttonIndex].myButtonResults[y].shouldBeShown = EditorGUILayout.Toggle("Show", buttonAlternativeResults[buttonIndex].myButtonResults[y].shouldBeShown);
            EditorGUILayout.EndHorizontal();
            if (buttonAlternativeResults[buttonIndex].myButtonResults[y].shouldBeShown)
            {
                buttonAlternativeResults[buttonIndex].myButtonResults[y].scrollPosition = EditorGUILayout.BeginScrollView(buttonAlternativeResults[buttonIndex].myButtonResults[y].scrollPosition);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                buttonAlternativeResults[buttonIndex].myButtonResults[y].ResultChanceOfHappening = EditorGUILayout.Slider("Chance of happening:", buttonAlternativeResults[buttonIndex].myButtonResults[y].ResultChanceOfHappening, 0, 100);
                buttonAlternativeResults[buttonIndex].myButtonResults[y].resultEventTitle = EditorGUILayout.TextField("Result Event Title:", buttonAlternativeResults[buttonIndex].myButtonResults[y].resultEventTitle);
                buttonAlternativeResults[buttonIndex].myButtonResults[y].resultEventText = EditorGUILayout.TextField("Result Event Text:", buttonAlternativeResults[buttonIndex].myButtonResults[y].resultEventText);
                buttonAlternativeResults[buttonIndex].myButtonResults[y].resultButtonText = EditorGUILayout.TextField("Result Event Button Text:", buttonAlternativeResults[buttonIndex].myButtonResults[y].resultButtonText);
                EditorGUILayout.EndVertical();
                if (GUILayout.Button("Add Result"))
                {
                    buttonAlternativeResults[buttonIndex].myButtonResults[y].results.Add(new ResultData());
                }
                for (int i = 0; i < buttonAlternativeResults[buttonIndex].myButtonResults[y].results.Count; i++)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Label("Result nr: " + (i + 1).ToString());
                    buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].myResult = (GameHub.EventResult)EditorGUILayout.EnumPopup("Result Type:", buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].myResult);
                    switch (buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].myResult)
                    {
                        case GameHub.EventResult.Death:
                            {
                                buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].myRelationType = (GameHub.RelationType)EditorGUILayout.EnumPopup("Relation To Die:", buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].myRelationType);
                                break;
                            }
                        case GameHub.EventResult.Money:
                            {
                                buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].myMoney = EditorGUILayout.IntSlider("Money Gotten:", buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].myMoney, -1000000, 1000000);
                                break;
                            }
                        case GameHub.EventResult.Income:
                            {
                                buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].myMoney = EditorGUILayout.IntSlider("Income Gotten:", buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].myMoney, -10000, 10000);
                                break;
                            }
                        case GameHub.EventResult.Land:
                            {
                                break;
                            }
                        case GameHub.EventResult.Job:
                            {
                                if (myDataScriptableObject.Jobbs.Count > 0)
                                {
                                    string[] jobs = myDataScriptableObject.Jobbs.ConvertAll(p => p.myJob).ToArray();
                                    buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].selectedJob = EditorGUILayout.Popup(buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].selectedJob, jobs);
                                    buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].myJob = myDataScriptableObject.Jobbs[buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].selectedJob];
                                }
                                else
                                {
                                    EditorGUILayout.LabelField("There are no Jobs currently. Please check the variable editor");
                                }
                                break;
                            }
                        case GameHub.EventResult.Character:
                            {
                                GUILayout.Label("Character Creation");
                                EditorGUILayout.BeginHorizontal();
                                buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].ShouldHaveJob = EditorGUILayout.Toggle("Should Have job", buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].ShouldHaveJob);
                                if (buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].ShouldHaveJob)
                                {
                                    buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].RandomizeJob = EditorGUILayout.Toggle("Randomize Job", buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].RandomizeJob);
                                    if (buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].RandomizeJob == false)
                                    {
                                        if (myDataScriptableObject.Jobbs.Count > 0)
                                        {
                                            string[] jobs = myDataScriptableObject.Jobbs.ConvertAll(p => p.myJob).ToArray();
                                            buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].selectedJob = EditorGUILayout.Popup(buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].selectedJob, jobs);
                                            buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].myJob = myDataScriptableObject.Jobbs[buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].selectedJob];
                                        }
                                        else
                                        {
                                            EditorGUILayout.LabelField("There are no Jobs currently. Please check the variable editor");
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].RandomizeAge = EditorGUILayout.Toggle("Randomize Age", buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].RandomizeAge);
                                if (buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].RandomizeAge == false)
                                {
                                    buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].CharacterAge = EditorGUILayout.IntSlider("Age:", buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].CharacterAge, 0, 100);
                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].CharacterRelation = (GameHub.RelationType)EditorGUILayout.EnumPopup("Relation Type:", buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].CharacterRelation);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();

                                buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].RandomizeGender = EditorGUILayout.Toggle("Randomize Gender", buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].RandomizeGender);
                                buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].OppositeGender = EditorGUILayout.Toggle("Is Opposite Gender of Player", buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].OppositeGender);
                                if (buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].RandomizeGender == false && buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].OppositeGender == false)
                                {
                                    buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].CharacterGender = (GameHub.Gender)EditorGUILayout.EnumPopup("Character Gender:", buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].CharacterGender);
                                }
                                EditorGUILayout.EndHorizontal();

                                break;
                            }
                        case GameHub.EventResult.Characteristic:
                            {
                                if (myDataScriptableObject.characteristics.Count > 0)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Add characteristic to Player");
                                    string[] characteristics = myDataScriptableObject.characteristics.ConvertAll(p => p.myCharacteristic).ToArray();
                                    buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].selectedCharacteristic = EditorGUILayout.Popup(buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].selectedCharacteristic, characteristics);
                                    buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].characteristic = myDataScriptableObject.characteristics[buttonAlternativeResults[buttonIndex].myButtonResults[y].results[i].selectedCharacteristic];
                                    EditorGUILayout.EndHorizontal();
                                }
                                else
                                {
                                    EditorGUILayout.LabelField("There are no characteristics currently. Please check the variable editor");
                                }
                                break;
                            }
                    }
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Remove Result"))
                    {
                        buttonAlternativeResults[buttonIndex].myButtonResults[y].results.RemoveAt(i);
                    }
                    GUI.backgroundColor = oldColor;
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndScrollView();
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Remove Button Result"))
                {
                    buttonAlternativeResults.RemoveAt(buttonIndex);
                    //resultScrollPositions.RemoveAt(y);
                }
                GUI.backgroundColor = oldColor;
            }
        }
       

        GUI.backgroundColor = Color.red;
        if(GUILayout.Button("Remove Button"))
        {
            buttonTexts.RemoveAt(buttonIndex);
            buttonAlternativeResults.RemoveAt(buttonIndex);
            ButtonNumber--;
        }
        GUI.backgroundColor = oldColor;
        EditorGUILayout.EndVertical();
    }

    //void IsCharacterDependant()
    //{
    //    EditorGUILayout.BeginHorizontal();

    //    selectedRelationDependable = (GameHub.RelationType)EditorGUILayout.EnumPopup("Dependable Character:", selectedRelationDependable);
    //    relationAmount = EditorGUILayout.IntSlider("Amount:", relationAmount, 1, 10, GUILayout.Width(200));

    //    EditorGUILayout.EndHorizontal();
    //    EditorGUILayout.BeginHorizontal();

    //    RelationAgeDependant = EditorGUILayout.Toggle("Age Dependant:", RelationAgeDependant);
    //    if (RelationAgeDependant)
    //    {
    //        DependableCharacterAge = EditorGUILayout.IntSlider("Dependable Age:", DependableCharacterAge, 0, 100);
    //        myDependableAgeRequierment = (EventScriptableObject.AgeRequierment)EditorGUILayout.EnumPopup("Dependable Age Requierment:", myDependableAgeRequierment);
    //    }
    //    EditorGUILayout.EndHorizontal();
    //    EditorGUILayout.BeginHorizontal();
    //    RelationJobDependant = EditorGUILayout.Toggle("Job Dependant:", RelationJobDependant);
    //    if (RelationJobDependant)
    //    {
    //        if (myDataScriptableObject.Jobbs.Count > 0)
    //        {
    //            string[] jobs = myDataScriptableObject.Jobbs.ConvertAll(i => i.myJob).ToArray();
    //            myDependableSelectedJob = EditorGUILayout.Popup(myDependableSelectedJob, jobs);
    //            myDependableChosenJobb = myDataScriptableObject.Jobbs[myDependableSelectedJob];
    //        }
    //        else
    //        {
    //            EditorGUILayout.LabelField("There are no Jobs currently. Please check the variable editor");
    //        }
    //    }
    //    EditorGUILayout.EndHorizontal();
    //}

    void CharacterDependables()
    {
        if(GUILayout.Button("Add Dependable Character"))
        {
            myDependables.Add(new DependableData());
        }
        dependableScrollPos = EditorGUILayout.BeginScrollView(dependableScrollPos);
        for (int i = 0; i < myDependables.Count; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.BeginHorizontal();

            myDependables[i].myRelationType = (GameHub.RelationType)EditorGUILayout.EnumPopup("Dependable Relation:", myDependables[i].myRelationType);
            myDependables[i].amount = EditorGUILayout.IntSlider("Amount:", myDependables[i].amount, 1, 10, GUILayout.Width(200));
            

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Toggle On if Character needs to exist, Off if it needs to not exist:");
            myDependables[i].haveOrNotHaveFlag = EditorGUILayout.Toggle("", myDependables[i].haveOrNotHaveFlag);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            myDependables[i].AgeDependant = EditorGUILayout.Toggle("Age Dependant:", myDependables[i].AgeDependant);
            if (myDependables[i].AgeDependant)
            {
                myDependables[i].Age = EditorGUILayout.IntSlider("Dependable Age:", myDependables[i].Age, 0, 100);
                myDependables[i].AgeRequierment = (EventScriptableObject.AgeRequierment)EditorGUILayout.EnumPopup("Dependable Age Requierment:", myDependables[i].AgeRequierment);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            myDependables[i].JobDependant = EditorGUILayout.Toggle("Job Dependant:", myDependables[i].JobDependant);
            if (myDependables[i].JobDependant)
            {
                if (myDataScriptableObject.Jobbs.Count > 0)
                {
                    string[] jobs = myDataScriptableObject.Jobbs.ConvertAll(y => y.myJob).ToArray();
                    myDependables[i].SelectedJob = EditorGUILayout.Popup(myDependables[i].SelectedJob, jobs);
                    myDependables[i].ChosenJobb = myDataScriptableObject.Jobbs[myDependables[i].SelectedJob];
                }
                else
                {
                    EditorGUILayout.LabelField("There are no Jobs currently. Please check the variable editor");
                }
            }
            EditorGUILayout.EndHorizontal();
            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Remove Dependable"))
            {
                myDependables.RemoveAt(i);
            }
            GUI.backgroundColor = oldColor;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();
    }
}
