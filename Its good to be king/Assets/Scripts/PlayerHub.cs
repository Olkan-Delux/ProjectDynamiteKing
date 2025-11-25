using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Money
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
    Character myCharacter;
    Character myInteraction;
    Character myHeir;
    List<AgeDeathIncrease> PlayerDeathRatePlan;
    public List<AgeDeathIncrease> AIDeathRatePlan;

    private void Start()
    {
        PlayerDeathRatePlan = new List<AgeDeathIncrease>();
        AIDeathRatePlan = new List<AgeDeathIncrease>();
        CreateDeathChances();
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        myCharacter = GameHub.Instance.CreateRandomCharacter(GameHub.AgeBracket.NewBorn);
        myCharacter.myJob = GameHub.Job.Nothing;
        //createSpawnFamily();
        myCharacter.myMoney.Copper = 0;
        myCharacter.myMoney.Silver = 0;
        myCharacter.myMoney.Gold = 0;
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
        GameHub.Instance.UpdateEvents();
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
        myCharacter.myChanceOfDyingEachYear += GameHub.Instance.myJobStatistics[(int)myCharacter.myJob].myDeathChance;
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
        int MoneyThisRound = character.mySalary;
        while(MoneyThisRound > 0)
        {
            if(MoneyThisRound > 10000)
            {
                character.myMoney.Gold += 1;
                MoneyThisRound -= 10000;
            }
            else if(MoneyThisRound >= 100)
            {
                character.myMoney.Silver += 1;
                MoneyThisRound -= 100;
            }
            else 
            {
                character.myMoney.Copper += MoneyThisRound;
                MoneyThisRound = 0;
            }
        }
        
        while(myCharacter.myMoney.Copper > 100)
        {
            character.myMoney.Silver += 1;
            character.myMoney.Copper -= 100;
        }
        while(myCharacter.myMoney.Silver > 100)
        {
            character.myMoney.Gold += 1;
            character.myMoney.Silver -= 100;
        }

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
            case GameHub.EventType.TryBecomeBaker:
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

    public void ActivateJobEvent(GameHub.Job aJob)
    {
        myCharacter.myJob = aJob;
        myCharacter.mySalary = GameHub.Instance.myJobStatistics[(int)aJob].myJobSalary;
        UIHub.Instance.UpdateUI();
    }

    private void FindWife()
    {
        GameHub.Gender PartnerGender = GameHub.Gender.Boy;
        if(myCharacter.myGender == GameHub.Gender.Boy)
        {
            PartnerGender = GameHub.Gender.Girl;
        }
        Character wife = myCharacter.AddRelation(GameHub.Instance.CreateRandomCharacter(myCharacter.GetAgeRange()), GameHub.RelationType.Wife);
        wife.myName = GameHub.Instance.GetRandomName(GameHub.Gender.Girl, GameHub.NameRegion.English);
        wife.myGender = PartnerGender;
        wife.AddRelation(myCharacter, GameHub.RelationType.Wife);
        gameObject.GetComponent<RelationsShips>().AddRelationToMenu(GameHub.RelationType.Wife, wife, myCharacter);
    }


    private void createSpawnFamily()
    {
        GameHub.AgeBracket dadAgeBracket = GameHub.AgeBracket.Senior;
        int ChanceYoungAdult = 25;
        int ChanceAdult = 60;
        myCharacter.hasParents = true;
        int ParentAgeChance = Random.Range(0, 100);
        if(ParentAgeChance <= ChanceYoungAdult)
        {
            dadAgeBracket = GameHub.AgeBracket.YoungAdult;
        }
        else if(ParentAgeChance <= ChanceYoungAdult + ChanceAdult)
        {
            dadAgeBracket = GameHub.AgeBracket.Adult;
        }
        int dadAge = 0;
        int momAge = 0;
        switch(dadAgeBracket)
        {
            case GameHub.AgeBracket.YoungAdult:
                {
                    dadAge = Random.Range(GameHub.Instance.ChildTopAge, GameHub.Instance.YoungAdultTopAge);
                    momAge = Random.Range(GameHub.Instance.ChildTopAge, dadAge + 2);
                    break;
                }
            case GameHub.AgeBracket.Adult:
                {
                    dadAge = Random.Range(GameHub.Instance.YoungAdultTopAge, GameHub.Instance.AdultTopAge);
                    momAge = Random.Range(GameHub.Instance.YoungAdultTopAge, dadAge + 2);
                    break;
                }
            case GameHub.AgeBracket.Senior:
                {
                    dadAge = Random.Range(GameHub.Instance.AdultTopAge, 65);
                    momAge = Random.Range(GameHub.Instance.AdultTopAge, 40);
                    break;
                }
        }


        int chanceFatherDies = 15;
        int ChanceMotherDies = 10;
        Character dad = new Character();
        if(Random.Range(0,100) > chanceFatherDies)
        {
            GameHub.Job job = GameHub.Job.Monk;
            while(job == GameHub.Job.Monk || job == GameHub.Job.Priest || job == GameHub.Job.Bishop)
            {
                job = GameHub.Instance.GetRandomJob();
            }
            int vibe = Random.Range(1,10);
            dad.CreateCharacter(dadAge, vibe, GameHub.Instance.GetRandomName(GameHub.Gender.Boy, GameHub.NameRegion.English), job, GameHub.Gender.Boy);
            myCharacter.AddRelation(dad, GameHub.RelationType.Father);
        }
        Character mom= new Character();
        if(Random.Range(0,100) > ChanceMotherDies)
        {
            int vibe = Random.Range(1, 10);
            mom.CreateCharacter(momAge, vibe, GameHub.Instance.GetRandomName(GameHub.Gender.Girl, GameHub.NameRegion.English), GameHub.Job.Nothing, GameHub.Gender.Girl);
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
                GameHub.Job job = GameHub.Job.Nothing;
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
                    sibling.CreateCharacter(momAge - (18 + i), vibe, GameHub.Instance.GetRandomName(GameHub.Gender.Boy, GameHub.NameRegion.English), job, GameHub.Gender.Boy);
                    myCharacter.AddRelation(sibling, GameHub.RelationType.Brother);
                }
                else
                {
                    sibling.CreateCharacter(momAge - (18 + i), vibe, GameHub.Instance.GetRandomName(GameHub.Gender.Girl, GameHub.NameRegion.English), job, GameHub.Gender.Girl);
                    myCharacter.AddRelation(sibling, GameHub.RelationType.Sister);
                }
            }
        }

    }


    private void CreateDeathChances()
    {
        AIDeathRatePlan.Add(CreateAgeDeathIncrease(0, 3));
        AIDeathRatePlan.Add(CreateAgeDeathIncrease(5, 1));
        AIDeathRatePlan.Add(CreateAgeDeathIncrease(36, 3));
        AIDeathRatePlan.Add(CreateAgeDeathIncrease(46, 5));
        AIDeathRatePlan.Add(CreateAgeDeathIncrease(56, 10));


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
