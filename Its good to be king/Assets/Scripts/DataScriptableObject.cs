using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "GeneralData", menuName = "Tools/General Data")]
public class DataScriptableObject : ScriptableObject
{
    public List<Characteristic> characteristics;
    public List<AgeGroup> AgeGroups;
    public List<Jobb> Jobbs;
}
[Serializable]
public class Characteristic
{
    public string myCharacteristic;
    public float myInheritanceChance;
    public float myDevelopmentChance;
}

[Serializable]
public class Jobb
{
    public string myJob = "";
    public int myBaseLowSalary = 0;
    public int myBaseHighSalary = 0;
    public float myAcceptanceRate = 0.0f;
    public bool HasSequenceJob = false;
    public Jobb myPromotionalJob = null;
    public bool canGetPromoted = false;
    public int MyYearsTilPromotion = 0;
    public bool IsVisibleInJobMenu = false;
    public bool CanCharactersRandomlyGet = false;
    public float deathChanceIncrease = 0.0f;

    public int GetRandomSalary()
    {
        return Random.Range(myBaseLowSalary, myBaseHighSalary);
    }
}

[Serializable]
public class AgeGroup
{
    public string myAgeGroupName = "";
    public int myAgeGroupLowAge = 0;
    public int myAgeGroupTopAge = 0;
    public float myChildChance = 0.0f;
    public float myDeathChance = 0.0f;
}
