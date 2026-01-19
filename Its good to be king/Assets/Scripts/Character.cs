using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Relation
{
    public Character myRelation;
    public GameHub.RelationType relationType;
}
[System.Serializable]
public class Character
{
    public bool myIsDead = false;
    public int myVibe = 0;
    public int myAge = 0;
    public int myHealth = 100;
    public int mySalary = 0;
    public string myName;
    public string myButtonName;
    public Jobb myJob;
    public GameHub.Gender myGender;
    public Money myMoney;
    public List<Relation> myRelations;
    public List<int> myOwnedLand;
    public int myChanceOfDyingEachYear = 0;
    public bool hasPartner = false;
    public bool hasParents = false;
    public bool hasBeenCheckedThisRound = false;
    public int AgeWhenLastPressed = 0;
    public GameHub.SocialClass mySocialClass = GameHub.SocialClass.Commoner;
    public List<Characteristic> myCharacteristics = new List<Characteristic>();

    public void CreateCharacter(int age, int aVibe, string aName, Jobb aJob, GameHub.Gender aGender)
    {
        myRelations = new List<Relation>();
        myOwnedLand = new List<int>();
        myMoney = new Money();
        myVibe = aVibe;
        myAge = age;
        myName = aName;
        myJob = aJob;
        myGender = aGender;
        GameHub.Instance.AddCharacter(this);
    }

    public void UpdateCharacter()
    {
        hasBeenCheckedThisRound = true;
        myAge++;
        if (GetIfPersonDies(myAge - 1, myAge))
        {
            Debug.Log(myName + " died at age " + myAge);
            myIsDead = true;
        }
    }

    public void AddMoney(int anAmount)
    {
        myMoney.Gold = anAmount / 10000;
        anAmount %= 10000;
        myMoney.Silver = anAmount / 100;
        anAmount %= 100;
        myMoney.Copper = anAmount;
    }

    public void SearchJob()
    {
        if(myJob == null && myGender == GameHub.Gender.Boy && myAge > 13)
        {
            int RandomRange = Random.Range(1, 100);
            Jobb aJob = GameHub.Instance.GetRandomJob();
            if(RandomRange > aJob.myAcceptanceRate)
            {
                myJob = aJob;
            }
        }
    }

    public void SearchPartner()
    {
        if(!hasPartner && myAge > 18)
        {
            int RandomRange = Random.Range(1, 100);
            if(RandomRange > 20)
            {
                hasPartner = true;
                Character partner = new Character();
                int index = 0;
                if(myGender == GameHub.Gender.Boy)
                {
                    int RandomAge = 0;
                    while(RandomAge < 18)
                    {
                        if(index > 20)
                        {
                            break;
                        }
                        RandomAge = Random.Range(myAge - 10,myAge + 2);
                    }
                    partner.CreateCharacter(RandomAge, 5, GameHub.Instance.GetRandomName(GameHub.Gender.Boy), GameHub.Instance.GetRandomJob(), GameHub.Gender.Boy);
                }
                else 
                {
                    int RandomAge = 0;
                    while (RandomAge < 18)
                    {
                        if (index > 20)
                        {
                            break;
                        }
                        RandomAge = Random.Range(myAge - 2, myAge + 10);
                    }
                    partner.CreateCharacter(RandomAge, 5, GameHub.Instance.GetRandomName(GameHub.Gender.Girl), null, GameHub.Gender.Girl);
                    partner.myJob = null;
                }
                AddRelation(partner, GameHub.RelationType.Wife);
                partner.AddRelation(this, GameHub.RelationType.Wife);
            }
        }
    }

    public void TryForKids(int childAge)
    {

        if (hasPartner && myGender == GameHub.Gender.Girl && myAge < 36) 
        {
            float chanceOfChild = 0.0f;
            for (int i = 0; i < GameHub.Instance.myDataScriptableObject.AgeGroups.Count; i++)
            {
                if (myAge > GameHub.Instance.myDataScriptableObject.AgeGroups[i].myAgeGroupLowAge && myAge < GameHub.Instance.myDataScriptableObject.AgeGroups[i].myAgeGroupTopAge)
                {
                    chanceOfChild = GameHub.Instance.myDataScriptableObject.AgeGroups[i].myChildChance;
                }
            }
            if (chanceOfChild > Random.Range(0.0f, 100.0f))
            {
                Character child = AddChild();
                child.myAge = childAge;
            }
        }
    }

    public void UpdateOnClick()
    {
        Debug.Log("Updated Character");
        for (int i = AgeWhenLastPressed; i < myAge; i++)
        {
            SearchPartner();
            TryForKids(myAge - i);
            if(!hasBeenCheckedThisRound)
            {
                SearchJob();
                hasBeenCheckedThisRound = true;
            }
        }
        foreach (Relation relation in myRelations)
        {
            if(!relation.myRelation.hasBeenCheckedThisRound)
            {
                relation.myRelation.myAge += myAge - AgeWhenLastPressed;
                if(GetIfPersonDies(relation.myRelation.myAge - (myAge - AgeWhenLastPressed), relation.myRelation.myAge))
                {
                    myIsDead = true;
                }
                relation.myRelation.AgeWhenLastPressed = relation.myRelation.myAge;
                hasBeenCheckedThisRound = true;
            }
        }
        GenerateParent();
        AgeWhenLastPressed = myAge;
    }

    public void GenerateParent()
    {
        if(hasParents == false)
        {
            int dadAge = Random.Range(myAge + 18, myAge + 38);
            int momAge = Random.Range(myAge + 18, dadAge + 2);


            Character dad = new Character();
            Character mom = new Character();
            bool dadIsDead = !GetIfPersonDies(18, dadAge);
            bool momIsDead = !GetIfPersonDies(18, momAge);
            if (dadIsDead)
            {
                dad.CreateCharacter(dadAge, 5, GameHub.Instance.GetRandomName(GameHub.Gender.Boy), GameHub.Instance.GetRandomJob(), GameHub.Gender.Boy);
                AddRelation(dad, GameHub.RelationType.Father);
                dad.AddRelation(this, GameHub.RelationType.Child);
                dad.hasPartner = true;
            }
            if(momIsDead)
            {
                mom.CreateCharacter(momAge, 5, GameHub.Instance.GetRandomName(GameHub.Gender.Girl), null, GameHub.Gender.Girl);
                AddRelation(mom, GameHub.RelationType.Mother);
                mom.AddRelation(this, GameHub.RelationType.Child);
                mom.hasPartner = true;
            }
            if (momIsDead && dadIsDead)
            {
                dad.AddRelation(mom, GameHub.RelationType.Wife);
                mom.AddRelation(dad, GameHub.RelationType.Wife);
            }
            hasParents = true;
            for(int i = 0; i < momAge - 18; i++)
            {
                if(i != myAge)
                {
                    mom.TryForKids(i);
                }
            }
        }
    }

    public Relation GetRelationFromName(string aName)
    {
        foreach (Relation relation in myRelations)
        {
            if(relation.myRelation.myName == aName)
            {
                return relation;
            }
        }
        Relation relle = new Relation();
        return relle;
    }

    public Relation GetRelationFromButton(string aButtonName)
    {
        foreach (Relation relation in myRelations)
        {
            if (relation.myRelation.myButtonName == aButtonName)
            {
                return relation;
            }
        }
        Relation relle = new Relation();
        return relle;
    }

    public void IncreaseVibe(int vibe)
    {
        if((myVibe + vibe) > 10)
        {
            myVibe = 10;
        }
        else if ((myVibe + vibe) < 0)
        {
            myVibe = 0;
        }
        else 
        {
            myVibe += vibe;
        }
    }

    public Character AddRelation(Character aCharacter, GameHub.RelationType aRelationType)
    {
        Relation relation = new Relation();
        relation.myRelation = aCharacter;
        relation.relationType = aRelationType;
        myRelations.Add(relation);
        return aCharacter;
    }

    public List<Character> GetCharacterFromRelationType(GameHub.RelationType aRelationType)
    {
        List<Character> characters = new List<Character>();
        foreach(Relation relation in myRelations)
        {
            if(relation.relationType == aRelationType)
            {
                characters.Add(relation.myRelation);
            }
        }
        return characters;
    }

    public Character AddChild()
    {
        Character child = GameHub.Instance.CreateRandomCharacter(0);
        child.myJob = null;
        AddRelation(child, GameHub.RelationType.Child);
        child.AddRelation(this, GetParentFromGender(myGender));
        child.hasParents = true;
        List<Character> Partner = GetCharacterFromRelationType(GameHub.RelationType.Wife);
        if (Partner.Count > 0)
        {
            child.AddRelation(Partner[0], GetParentFromGender(Partner[0].myGender));
            Partner[0].AddRelation(child, GameHub.RelationType.Child);
        }

        foreach (Character character in GetCharacterFromRelationType(GameHub.RelationType.Child))
        {
            if(character != child)
            {
            child.AddRelation(character, GetSiblingFromGender(character.myGender));
            character.AddRelation(child, GetSiblingFromGender(child.myGender));
            }
        }
        return child;
    }

    private GameHub.RelationType GetParentFromGender(GameHub.Gender aGender)
    {
        switch(aGender)
        {
            case GameHub.Gender.Boy:
                {
                    return GameHub.RelationType.Father;
                }
            case GameHub.Gender.Girl:
                {
                    return GameHub.RelationType.Mother;
                }
        }
        return GameHub.RelationType.Mother;
    }

    private GameHub.RelationType GetSiblingFromGender(GameHub.Gender aGender)
    {
        if (aGender == GameHub.Gender.Boy)
        {
            return GameHub.RelationType.Brother;
        }
        else
        {
            return GameHub.RelationType.Sister;
        }
    }

    public bool GetIfPersonDies(int startAge, int endAge)
    {
        float chanceOfDeath = 0.1f;
        for (int i = 0; i < GameHub.Instance.myDataScriptableObject.AgeGroups.Count; i++)
        {
            if (endAge > GameHub.Instance.myDataScriptableObject.AgeGroups[i].myAgeGroupLowAge && endAge < GameHub.Instance.myDataScriptableObject.AgeGroups[i].myAgeGroupTopAge)
            {
                chanceOfDeath = GameHub.Instance.myDataScriptableObject.AgeGroups[i].myDeathChance;
            }
        }
        float chanceDead = 1f - Mathf.Pow(1f - chanceOfDeath, endAge - startAge);
        float random = Random.Range(0.0f, 100.0f);
        if(chanceDead > random)
        {
            return true;
        }
        return false;
    }
}
