using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EventData", menuName ="Tools/Event Data")]
public class EventScriptableObject : ScriptableObject
{
    public enum AgeRequierment
    {
        Above,
        Below,
        Exact
    };
    public GameHub.Job selectedJobOption;
    public GameHub.Job DependableJobOption;
    public GameHub.RelationType selectedRelationDependable;
    public int relationAmount;
    public int Age;
    public AgeRequierment myAgeRequierment;
    public AgeRequierment myDependableAgeRequierment;
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
    public List<bool> HasSecondEvent;
    public List<string> buttonTexts;
    public List<string> buttonResultEventText;
    public List<string> buttonResultEventTitle;
    public List<string> buttonResultButtonText;

    public bool RandomizeName = false;
    public bool RandomizeAge = false;
    public bool ShouldHaveJob = false;
    public bool RandomizeJob = false;
    public int CharacterAge = 0;
    public GameHub.Job CharacterJob = GameHub.Job.Peasant;
    public GameHub.RelationType CharacterRelation = GameHub.RelationType.Stranger;

    public List<ResultDataRegistry> buttonResults = new List<ResultDataRegistry>();
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

    public bool RandomizeName = true;
    public bool RandomizeAge = false;
    public bool ShouldHaveJob = false;
    public bool RandomizeJob = false;
    public bool RandomizeGender = false;
    public bool OppositeGender = false;
    public int CharacterAge = 0;
    public GameHub.Job CharacterJob = GameHub.Job.Nothing;
    public GameHub.RelationType CharacterRelation = GameHub.RelationType.Stranger;
    public GameHub.Gender CharacterGender = GameHub.Gender.Girl;
}

[System.Serializable]
public class ResultDataRegistry
{
    public List<ResultData> results = new List<ResultData>();
}
