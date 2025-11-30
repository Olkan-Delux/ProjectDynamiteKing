using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EventManagerWindow : EditorWindow
{
    private const string RegistryAssetPath = "Assets/Scriptable Objects/EventDataRegistry.asset";
    private EventRegistryScriptableObject myEventRegistrySO;
    private EventScriptableObject mySelectedObject;
    private int mySelectedIndex;
    private Vector2 scrollPos;

    [MenuItem("Window/Event Manager Window")]
    public static void ShowWindow()
    {
        GetWindow<EventManagerWindow>("Event Manager Window");
    }

    private void OnEnable()
    {
        myEventRegistrySO = AssetDatabase.LoadAssetAtPath<EventRegistryScriptableObject>(RegistryAssetPath);
        if (myEventRegistrySO != null)
        {
            Debug.Log("Loaded: " + myEventRegistrySO.name);
        }
        else
        {
            Debug.LogError("Could not load ScriptableObject at path!");
        }
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Dynamic Scrollable Button List", EditorStyles.boldLabel);

        // Start scroll view
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // Draw all buttons dynamically
        for (int i = 0; i < myEventRegistrySO.Events.Count; i++)
        {
            if (GUILayout.Button(myEventRegistrySO.Events[i].EventTitle, GUILayout.Height(25)))
            {
                Debug.Log($"Clicked: {myEventRegistrySO.Events[i].EventTitle}");
                mySelectedObject = myEventRegistrySO.Events[i];
                mySelectedIndex = i;
            }
        }

        if (mySelectedObject != null)
        {
            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button("Load Event in Manager", GUILayout.Height(25)))
            {
                EventEditorWindow window = GetWindow<EventEditorWindow>("Event Editor Window");
                window.selectedJobOption = mySelectedObject.selectedJobOption;
                window.DependableJobOption = mySelectedObject.DependableJobOption;
                //window.selectedRelationOption = mySelectedObject.selectedRelationOption;
                window.selectedRelationDependable = mySelectedObject.selectedRelationDependable;
                window.relationAmount = mySelectedObject.relationAmount;
                window.Age = mySelectedObject.Age;
                window.DependableCharacterAge = mySelectedObject.DependableCharacterAge;
                window.JobDependant = mySelectedObject.JobDependant;
                window.AgeDependant = mySelectedObject.AgeDependant;
                window.DependableCharacterFlag = mySelectedObject.DependableCharacterFlag;
                window.RelationJobDependant = mySelectedObject.RelationJobDependant;
                window.RelationAgeDependant = mySelectedObject.RelationAgeDependant;
                window.CanBeGottenAgain = mySelectedObject.CanBeGottenAgain;
                window.ChanceOfHappening = mySelectedObject.ChanceOfHappening;
                window.EventTitle = mySelectedObject.EventTitle;
                window.EventText = mySelectedObject.EventText;
                window.buttonTexts = mySelectedObject.buttonTexts;
                window.buttonResultEventText = mySelectedObject.buttonResultEventText;
                window.buttonResultEventTitle = mySelectedObject.buttonResultEventTitle;
                window.buttonResultButtonText = mySelectedObject.buttonResultButtonText;
                window.buttonResults = mySelectedObject.buttonResults;
                window.ButtonNumber = mySelectedObject.buttonTexts.Count;

                this.Close();
            }

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Remove Event"))
            {
                string path = AssetDatabase.GetAssetPath(mySelectedObject);
                if(!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Debug.Log("Deleted: " + path);
                    myEventRegistrySO.Events.RemoveAt(mySelectedIndex);

                }
            }
            GUI.backgroundColor = oldColor;
        }

        // End scroll view
        EditorGUILayout.EndScrollView();
    }
}
