using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName ="Tools/Event Data")]
public class EventScriptableObject : ScriptableObject
{
    public GameHub.Job selectedJobOption;
    public GameHub.Job DependableJobOption;
    public GameHub.RelationType selectedRelationDependable;
    public int relationAmount;
    public int Age;
    public int DependableCharacterAge;
    public bool JobDependant;
    public bool AgeDependant;
    public bool DependableCharacterFlag;
    public bool RelationJobDependant;
    public bool RelationAgeDependant;
    public bool CanBeGottenAgain;
    public float ChanceOfHappening;

    public string EventTitle;
    public string EventText;
    public List<string> buttonTexts;
    public List<List<ResultData>> buttonResults = new List<List<ResultData>>();
}

[CreateAssetMenu(fileName = "EventDataRegistry", menuName = "Tools/Event Data Registry")]
public class EventRegistryScriptableObject : ScriptableObject
{
    public List<EventScriptableObject> Events = new List<EventScriptableObject>();
}

[System.Serializable]
public class ResultData
{
    public GameHub.EventResult myResult;
    public int myMoney;
    public GameHub.RelationType myRelationType;
    public GameHub.Job myJob;
}
