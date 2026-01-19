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
    public Jobb selectedJobOption;
    public Jobb DependableJobOption;
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
    public bool IsSocialClassDependant = true;
    public bool IsCharacteristicDependant;
    public Characteristic myChosenCharacteristic;
    public float ChanceOfHappening;
    public GameHub.SocialClass socialClass = GameHub.SocialClass.Commoner;

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
    //public GameHub.Job CharacterJob = GameHub.Job.Peasant;
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
    public Jobb myJob;
    public int selectedJob = 0;

    public bool RandomizeName = true;
    public bool RandomizeAge = false;
    public bool ShouldHaveJob = false;
    public bool RandomizeJob = false;
    public bool RandomizeGender = false;
    public bool OppositeGender = false;
    public int CharacterAge = 0;
    public GameHub.RelationType CharacterRelation = GameHub.RelationType.Stranger;
    public GameHub.Gender CharacterGender = GameHub.Gender.Girl;
    public Characteristic characteristic = null;
    public int selectedCharacteristic = 0;
}

[System.Serializable]
public class ResultDataRegistry
{
    public List<ResultData> results = new List<ResultData>();
}
