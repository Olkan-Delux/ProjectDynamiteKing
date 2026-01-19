using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Money
{
    public int Copper;
    public int Silver;
    public int Gold;
}

public struct AgeDeathIncrease
{
    public int Age;
    public int ChanceIncrease;
}

public class PlayerHub : MonoBehaviour
{
    public Character myCharacter;
    public Character myInteraction;
    public Character myHeir;
    List<AgeDeathIncrease> PlayerDeathRatePlan;

    private void Start()
    {
        PlayerDeathRatePlan = new List<AgeDeathIncrease>();
        CreateDeathChances();
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        myCharacter = GameHub.Instance.CreateRandomCharacter(0);
        myCharacter.myName = MenuData.ourCharacterName;
        myCharacter.myJob = null;
        //createSpawnFamily();
        myCharacter.myMoney.Copper = 0;
        myCharacter.myMoney.Silver = 0;
        myCharacter.myMoney.Gold = 0;
        GameHub.Instance.LoadPreviousGame();
        //For loop is to stop people from aging by your newly loaded age
        foreach (Relation character in myCharacter.myRelations)
        {
            character.myRelation.hasBeenCheckedThisRound = true;
        }
        gameObject.AddComponent<RelationsShips>().DrawRelations(myCharacter);

        UIHub.Instance.UpdateUI();
    }

    public void SetHeir(Character aHeir)
    {
        myHeir = aHeir;
    }

    public void SetInteraction(Character anInteraction)
    {
        myInteraction = anInteraction;
    }

    public void Die()
    {
        gameObject.GetComponent<RelationsShips>().ClearRelations();
        if (myHeir != null)
        {
            myCharacter = myHeir;
            gameObject.GetComponent<RelationsShips>().DrawRelations(myCharacter);
            myHeir = null;
        }
        else 
        {
            CreatePlayer();
        }
    }

    public Character GetCurrentInteraction()
    {
        return myInteraction;
    }

    public void SetCurrentInteraction(Character anInteraction)
    {
        myInteraction = anInteraction;
    }

    public void SwitchCharacter(Character newPlayer)
    {
        myCharacter = newPlayer;
    }

    public Character GetPlayerCharacter()
    {
        return myCharacter;
    }

    public RelationsShips GetRelationShipMenu()
    {
        return GetComponent<RelationsShips>();
    }

    public void NextRound()
    {
        GameHub.Instance.ResetUpdateStatus();
        //GameHub.Instance.UpdateEvents();
        CalculatePlayerDeathChance();
        GetSalary(myCharacter);
        myCharacter.UpdateCharacter();
        if(myCharacter.myIsDead)
        {
            Die();
        }
        for(int i = myCharacter.myRelations.Count - 1; i >= 0; i--)
        {
            Relation relation = myCharacter.myRelations[i];
            relation.myRelation.UpdateCharacter();
            switch(relation.relationType)
            {
                case GameHub.RelationType.Child:
                {
                    GetSalary(relation.myRelation);
                    break;
                }
                case GameHub.RelationType.Father:
                {
                    GetSalary(relation.myRelation);
                    break;
                }
            }
            if(relation.myRelation.myIsDead)
            {
                myCharacter.myRelations.Remove(relation);
                gameObject.GetComponent<RelationsShips>().RemoveRelationFromMenu(relation.myRelation.myButtonName);
            }
        }
        GetComponent<RelationsShips>().UpdateRelationMenu();
        GameHub.Instance.UpdateEvents();
        UIHub.Instance.UpdateUI();
    }

    private void CalculatePlayerDeathChance()
    {
        myCharacter.myChanceOfDyingEachYear = 0;
        for(int i = PlayerDeathRatePlan.Count - 1; i >= 0; i--)
        {
            if(myCharacter.myAge > PlayerDeathRatePlan[i].Age)
            {
                myCharacter.myChanceOfDyingEachYear = PlayerDeathRatePlan[i].ChanceIncrease;
                break;
            }
        }
        if(myCharacter.myJob != null)
        {
            myCharacter.myChanceOfDyingEachYear += (int)myCharacter.myJob.deathChanceIncrease;
        }
    }

    private void ChangeHealth(int aHealth)
    {
        myCharacter.myHealth += aHealth;
        if (myCharacter.myHealth <= 0)
        {
            Die();
        }
    }


    private void MeetPerson(Character aPerson, GameHub.RelationType aRelationType)
    {
        myCharacter.AddRelation(aPerson, aRelationType);
    }

    private void GetSalary(Character character)
    {
        character.AddMoney(character.mySalary);
    }

    public void ActivateButtonEvent(int index)
    {
        ActivateEvent((GameHub.EventType)index);
    }

    public void ActivateEvent(GameHub.EventType anEventType)
    {
        Debug.Log(anEventType);
        switch(anEventType)
        {
            case GameHub.EventType.Child:
                {
                    GetRelationShipMenu().AddRelationToMenu(GameHub.RelationType.Child, myCharacter.AddChild(), myCharacter);
                    break;
                }
            case GameHub.EventType.Wife:
                {
                    FindWife();
                    break;
                }
            case GameHub.EventType.Crusade:
                {
                    break;
                }
        }
    }

    public void ActivateOtherEvent(ResultData aResult)
    {
        switch (aResult.myResult)
        {
            case GameHub.EventResult.Death:
                {
                    Die();
                    break;
                }
            case GameHub.EventResult.Money:
                {
                    myCharacter.AddMoney(aResult.myMoney);
                    break;
                }
            case GameHub.EventResult.Income:
                {
                    myCharacter.mySalary += aResult.myMoney;
                    break;
                }
            case GameHub.EventResult.Land:
                {

                    break;
                }
            case GameHub.EventResult.Job:
                {
                    myCharacter.myJob = aResult.myJob;
                    break;
                }
            case GameHub.EventResult.Character:
                {

                    break;
                }
            case GameHub.EventResult.Characteristic:
                {
                    myCharacter.myCharacteristics.Add(aResult.characteristic);
                    break;
                }
        }
    }

    public void ActivateJobEvent(Jobb aJob)
    {
        myCharacter.myJob = aJob;
        myCharacter.mySalary = aJob.GetRandomSalary();
        UIHub.Instance.UpdateUI();
    }

    private void FindWife()
    {
        GameHub.Gender PartnerGender = GameHub.Gender.Boy;
        if(myCharacter.myGender == GameHub.Gender.Boy)
        {
            PartnerGender = GameHub.Gender.Girl;
        }
        Character wife = myCharacter.AddRelation(GameHub.Instance.CreateRandomCharacter(GetRandomAgeInAgeGroupFromAge(myCharacter.myAge)), GameHub.RelationType.Wife);
        wife.myName = GameHub.Instance.GetRandomName(GameHub.Gender.Girl);
        wife.myGender = PartnerGender;
        wife.AddRelation(myCharacter, GameHub.RelationType.Wife);
        gameObject.GetComponent<RelationsShips>().AddRelationToMenu(GameHub.RelationType.Wife, wife, myCharacter);
    }

    private int GetRandomAgeInAgeGroupFromAge(int age)
    {
        for(int i = 0; i < GameHub.Instance.myDataScriptableObject.AgeGroups.Count; i++)
        {
            if(age > GameHub.Instance.myDataScriptableObject.AgeGroups[i].myAgeGroupLowAge && age < GameHub.Instance.myDataScriptableObject.AgeGroups[i].myAgeGroupTopAge)
            {
                return (int)Random.Range(GameHub.Instance.myDataScriptableObject.AgeGroups[i].myAgeGroupLowAge, GameHub.Instance.myDataScriptableObject.AgeGroups[i].myAgeGroupTopAge);
            }
        }
        return age;
    }


    private void createSpawnFamily()
    {
        myCharacter.hasParents = true;
        int momAge = 0;
        int dadAge = 0;

        int momLowerAge = -10;
        int momUpperAge = 2;

        int momActualAgeInRelation = Random.Range(momLowerAge, momUpperAge);
        dadAge = Random.Range(18, 70);
        momAge = dadAge + momActualAgeInRelation;
        if(momAge < 18)
        {
            momAge = 18;
        }
        else if(momAge > 48)
        {
            momAge = 48;
        }

        int chanceFatherDies = 15;
        int ChanceMotherDies = 10;
        Character dad = new Character();
        if(Random.Range(0,100) > chanceFatherDies)
        {
            Jobb job = GameHub.Instance.GetRandomJob();
            int vibe = Random.Range(1,10);
            dad.CreateCharacter(dadAge, vibe, GameHub.Instance.GetRandomName(GameHub.Gender.Boy), job, GameHub.Gender.Boy);
            myCharacter.AddRelation(dad, GameHub.RelationType.Father);
        }
        Character mom= new Character();
        if(Random.Range(0,100) > ChanceMotherDies)
        {
            int vibe = Random.Range(1, 10);
            mom.CreateCharacter(momAge, vibe, GameHub.Instance.GetRandomName(GameHub.Gender.Girl), null, GameHub.Gender.Girl);
            myCharacter.AddRelation(mom, GameHub.RelationType.Mother);
        }

        int YearlyChanceOfHavingSiblings = 25;
        int SiblingAmount = 0;
        for(int i = 0; i < momAge - 18; i++)
        {
            if(Random.Range(0, 100) < YearlyChanceOfHavingSiblings)
            {
                SiblingAmount++;
                int gender = Random.Range(0, 2);
                Character sibling = new Character();
                Jobb job = null;
                if(i > 12 && (GameHub.Gender)gender == GameHub.Gender.Boy) 
                {
                    if(SiblingAmount < 2)
                    {
                        job = dad.myJob;
                    }
                    else 
                    {
                        job = GameHub.Instance.GetRandomJob();
                    }
                }
                int vibe = Random.Range(1,10);
                if(gender == 0)
                {
                    sibling.CreateCharacter(momAge - (18 + i), vibe, GameHub.Instance.GetRandomName(GameHub.Gender.Boy), job, GameHub.Gender.Boy);
                    myCharacter.AddRelation(sibling, GameHub.RelationType.Brother);
                }
                else
                {
                    sibling.CreateCharacter(momAge - (18 + i), vibe, GameHub.Instance.GetRandomName(GameHub.Gender.Girl), job, GameHub.Gender.Girl);
                    myCharacter.AddRelation(sibling, GameHub.RelationType.Sister);
                }
            }
        }

    }


    private void CreateDeathChances()
    {
        PlayerDeathRatePlan.Add(CreateAgeDeathIncrease(36, 3));
        PlayerDeathRatePlan.Add(CreateAgeDeathIncrease(46, 5));
        PlayerDeathRatePlan.Add(CreateAgeDeathIncrease(56, 10));
        PlayerDeathRatePlan.Add(CreateAgeDeathIncrease(66, 66));
    }

    private AgeDeathIncrease CreateAgeDeathIncrease(int age, int chanceIncrease)
    {
        AgeDeathIncrease deathChance = new AgeDeathIncrease();
        deathChance.Age = age;
        deathChance.ChanceIncrease = chanceIncrease;
        return deathChance;
    }
}
