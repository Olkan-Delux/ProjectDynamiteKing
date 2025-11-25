using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public struct ButtonEvent
{
    public List<Event> myPossibleEvents;
    public List<int> ChanseOfHappeningOutof100;
}

public class ChanceEvent
{
    public Event myEvent;
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
    public bool hasBeenGotten;
    public float ChanceOfHappening;

    public string EventTitle;
    public string EventText;
    public List<string> buttonTexts;
    public List<List<ResultData>> buttonResults = new List<List<ResultData>>();
}

public struct JobStatistics
{
    public int myJobChance;
    public int myDeathChance;
    public int myJobChancePlayer;
    public int myJobSalary;
}

public struct RegionNames
{
    public string[] girlNames;
    public string[] boyNames;
}
public class GameHub : MonoBehaviour
{
    public enum Gender
    {
        Boy,
        Girl
    };
    public enum Job
    {
        Peasant,
        Smith,
        Fisher,
        Baker,
        ShoeMaker,
        Carpenter,
        Merchant,
        Hunter,
        Miner,
        Mercenary,
        Guard,
        Sailor,
        Soldier,
        Monk,
        Priest,
        Bishop,
        Knight,
        Noblemen,
        Nothing,
        King,
    };

    public enum LandMark
    {
        WindMill,
        Castle,
        Road,
        Nothing
    };

    public enum EventType
    {
        Mingle,
        Child,
        Wife,
        Crusade,
        VikingRaid,
        TryBecomePeasant,
        TryBecomeSmith,
        TryBecomeFisher,
        TryBecomeBaker,
        TryBecomeShoeMaker,
        TryBecomeCarpenter,
        TryBecomeMerchant,
        TryBecomeHunter,
        TryBecomeMiner,
        TryBecomeMercenary,
        TryBecomeGuard,
        TryBecomeSailor,
        TryBecomeSoldier,
        TryBecomeMonk,
        TryBecomePriest,
        TryBecomeBishop,
        TryBecomeKnight,
        TryBecomeNoblemen,
        TryBecomeKing,
        Count
    };

    public enum RelationType
    {
        Self,
        Wife,
        Child,
        Mother,
        Father,
        Brother,
        Sister,
        Stranger,
        Count
    }

    public enum AgeBracket
    {
        NewBorn,
        Child, 
        YoungAdult,
        Adult,
        Senior
    }

    public enum NameRegion
    {
        Nordic,
        English,
        SheeshMama,
        Count
    }

    public enum EventResult
    {
        Death,
        Money,
        Income,
        Land,
        Job,
        Child,
        Wife
    };


    private static GameHub instance;


    public PlayerHub myPlayer;
    public Map myMap;
    public GameObject EventButton;
    public GameObject EventCanvas;
    public GameObject EventPanel;
    ButtonEvent[] myButtonEvents;
    private List<ChanceEvent> myChanceEvents = new List<ChanceEvent>();
    Event[] myChosenEvent;
    public int ChildTopAge;
    public int YoungAdultTopAge;
    public int AdultTopAge;
    public List<Character> allCharacters;
    public JobStatistics[] myJobStatistics;
    private RegionNames[] myRegionNames;
    private KiingdomManager myKingdomManager;

    public EventRegistryScriptableObject myEvents;

    public static GameHub Instance{ get { return instance; } }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else 
        {
            instance = this;    
        }
    }

    private void Start()
    {
        allCharacters = new List<Character>();
        SetStatistics();
        CreateEvents();
        ReadNames();
        ChildTopAge = 12;
        YoungAdultTopAge = 24;
        AdultTopAge = 36;
    }

    public void CreateKingdoms()
    {
        myKingdomManager = new KiingdomManager();
        myKingdomManager.CreateCountries(15, 20);
        myKingdomManager.DrawCountries();
    }

    public PlayerHub GetPlayer()
    {
        return myPlayer;
    }

    public void AddCharacter(Character aCharacter)
    {
        allCharacters.Add(aCharacter);
    }

    public void ResetUpdateStatus()
    {
        foreach(Character character in allCharacters)
        {
            character.hasBeenCheckedThisRound = false;
        }
    }

    public void UpdateEvents()
    {
        for (int i = 0; i < myChanceEvents.Count; i++)
        {
            bool shouldActivateEvent = true;
            if (myChanceEvents[i].CanBeGottenAgain)
            {

            }
            if (myChanceEvents[i].JobDependant && myPlayer.GetPlayerCharacter().myJob != myChanceEvents[i].selectedJobOption)
            {
                shouldActivateEvent = false;
            }
            if (myChanceEvents[i].AgeDependant && myPlayer.GetPlayerCharacter().myAge != myChanceEvents[i].Age)
            {
                shouldActivateEvent = false;
            }
            if(myChanceEvents[i].ChanceOfHappening < Random.Range(0, 100))
            {
                shouldActivateEvent = false;
            }
            if(myChanceEvents[i].DependableCharacterFlag)
            {
                Character selectedCharacter = myPlayer.GetPlayerCharacter().GetCharacterFromRelationType(myChanceEvents[i].selectedRelationDependable)[0];
                if(selectedCharacter != null)
                {
                    if(selectedCharacter.myAge < myChanceEvents[i].DependableCharacterAge)
                    {
                        shouldActivateEvent = false;
                    }
                    if(selectedCharacter.myJob != myChanceEvents[i].DependableJobOption)
                    {
                        shouldActivateEvent = false;
                    }
                }
                else
                {
                    shouldActivateEvent = false;
                }
            }
            if (shouldActivateEvent)
            {
                myChanceEvents[i].myEvent.Activate();
                break;
            }
        }
    }

    void ReadNames()
    {
        myRegionNames = new RegionNames[(int)NameRegion.Count];
        string path = "Assets/TextFiles/Names.txt";
        StreamReader reader = new StreamReader(path);
        string entiretext = reader.ReadToEnd();
        string[] regions = entiretext.Split(',');
        for(int i = 0; i < regions.Length; i++)
        { 
            string[] genders = regions[i].Split('/');
            string[] boyNames = genders[0].Split('\n');
            string[] girlNames = genders[1].Split('\n');
            myRegionNames[i].boyNames = boyNames;
            myRegionNames[i].girlNames = girlNames;
        }
        reader.Close();
    }

    public string GetRandomName(Gender aGender, NameRegion aRegion)
    {
        string name;
        if(aGender == Gender.Boy)
        {
            name = myRegionNames[(int)aRegion].boyNames[Random.Range(0, myRegionNames[(int)aRegion].boyNames.Length)];
        }
        else 
        {
            name = myRegionNames[(int)aRegion].boyNames[Random.Range(0, myRegionNames[(int)aRegion].girlNames.Length)];
        }
        return name;
    }

    public void CreateEvents()
    {



        myButtonEvents = new ButtonEvent[(int)EventType.Count];
        myChosenEvent = new Event[(int)EventType.Count];
        for(int i = 0; i < myButtonEvents.Length; i++)
        {
            myButtonEvents[i].ChanseOfHappeningOutof100 = new List<int>();
            myButtonEvents[i].myPossibleEvents = new List<Event>();
        }

        EventFactory factory = gameObject.AddComponent<EventFactory>();

        for(int i = 0; i < myEvents.Events.Count; i++)
        {
            ChanceEvent aChanceEvent = new ChanceEvent();
            Event chanceEvent = factory.CreateEvent(myEvents.Events[i].EventTitle, myEvents.Events[i].EventText);
            for(int j = 0; j < myEvents.Events[i].buttonResults.Count; j++)
            {
                chanceEvent.AddEventDecision(myEvents.Events[i].buttonTexts[j], () => {
                    myPlayer.ActivateEvent(EventType.Crusade);
                    chanceEvent.DeActivate();
                });
            }
            chanceEvent.SetCanvasAndButton(EventCanvas, EventButton, EventPanel);
            aChanceEvent.myEvent = chanceEvent;
            aChanceEvent.selectedJobOption = myEvents.Events[i].selectedJobOption;
            aChanceEvent.DependableJobOption = myEvents.Events[i].DependableJobOption;
            aChanceEvent.selectedRelationDependable = myEvents.Events[i].selectedRelationDependable;
            aChanceEvent.relationAmount = myEvents.Events[i].relationAmount;
            aChanceEvent.Age = myEvents.Events[i].Age;
            aChanceEvent.DependableCharacterAge = myEvents.Events[i].DependableCharacterAge;
            aChanceEvent.JobDependant = myEvents.Events[i].JobDependant;
            aChanceEvent.AgeDependant = myEvents.Events[i].AgeDependant;
            aChanceEvent.DependableCharacterFlag = myEvents.Events[i].DependableCharacterFlag;
            aChanceEvent.RelationJobDependant = myEvents.Events[i].RelationJobDependant;
            aChanceEvent.RelationAgeDependant = myEvents.Events[i].RelationAgeDependant;
            aChanceEvent.CanBeGottenAgain = myEvents.Events[i].CanBeGottenAgain;
            aChanceEvent.ChanceOfHappening = myEvents.Events[i].ChanceOfHappening;
            aChanceEvent.EventTitle = myEvents.Events[i].EventTitle;
            aChanceEvent.EventText = myEvents.Events[i].EventText;
            aChanceEvent.buttonTexts = myEvents.Events[i].buttonTexts;
            aChanceEvent.buttonResults = myEvents.Events[i].buttonResults;

            myChanceEvents.Add(aChanceEvent);
            //myButtonEvents[(int)EventType.Crusade].myPossibleEvents.Add(chanceEvent);
            //myButtonEvents[(int)EventType.Wife].ChanseOfHappeningOutof100.Add(50);
        }


        Event FindWifevent = factory.CreateEvent("Yo Wife up this bitch if you finna risk it", "You can wife a nice lady on the street with some bomb as coochie baby, LESS GOOOOOOO");
        FindWifevent.AddEventDecision("Wife that bitch", () => { myPlayer.ActivateEvent(EventType.Wife);
            FindWifevent.DeActivate();
        });
        FindWifevent.AddEventDecision("CoochiBaby", ()=> { FindWifevent.DeActivate(); });
        FindWifevent.SetCanvasAndButton(EventCanvas, EventButton, EventPanel);
        myButtonEvents[(int)EventType.Wife].myPossibleEvents.Add(FindWifevent);
        myButtonEvents[(int)EventType.Wife].ChanseOfHappeningOutof100.Add(50);



        Event NotFoundWifeEvent = factory.CreateEvent("Alone as always", "youre a noob bruhdude");
        NotFoundWifeEvent.AddEventDecision("Sheeesh", ()=> { NotFoundWifeEvent.DeActivate(); });
        NotFoundWifeEvent.SetCanvasAndButton(EventCanvas, EventButton, EventPanel);
        myButtonEvents[(int)EventType.Wife].myPossibleEvents.Add(NotFoundWifeEvent);
        myButtonEvents[(int)EventType.Wife].ChanseOfHappeningOutof100.Add(50);



        Event GetChild = factory.CreateEvent("SUCCESS", "Your wife successfully childed a child");
        GetChild.AddEventDecision("Cool", ()=> { myPlayer.ActivateEvent(EventType.Child);
        GetChild.DeActivate();
        });
        GetChild.SetCanvasAndButton(EventCanvas, EventButton, EventPanel);
        myButtonEvents[(int)EventType.Child].myPossibleEvents.Add(GetChild);
        myButtonEvents[(int)EventType.Child].ChanseOfHappeningOutof100.Add(100);

        Event NotFoundJobEvent = factory.CreateEvent("Bad Job Market", "You couldnt find a job");
        NotFoundJobEvent.AddEventDecision("Damn", ()=> { NotFoundJobEvent.DeActivate(); });
        NotFoundJobEvent.SetCanvasAndButton(EventCanvas, EventButton, EventPanel);

        for(int i = 0; i < (int)Job.King; i++)
        {
            if (i != (int)Job.Knight || i != (int)Job.Noblemen || i != (int)Job.King || i != (int)Job.Nothing || i != (int)Job.Bishop)
            {
                int index = i;
                Event jobEvent = factory.CreateEvent("They wanted you, Congrats!", "There was a spot open for you as a " + System.Enum.GetName(typeof(Job), (Job)i));
                jobEvent.AddEventDecision("Ill take it!", () =>
                {
                    myPlayer.ActivateJobEvent((Job)index);
                    jobEvent.DeActivate();
                });
                jobEvent.AddEventDecision("I might find something better", () => { jobEvent.DeActivate(); });
                jobEvent.SetCanvasAndButton(EventCanvas, EventButton, EventPanel);
                myButtonEvents[(int)GetEventFromJob((Job)i)].myPossibleEvents.Add(jobEvent);
                myButtonEvents[(int)GetEventFromJob((Job)i)].ChanseOfHappeningOutof100.Add(myJobStatistics[i].myJobChancePlayer);
                myButtonEvents[(int)GetEventFromJob((Job)i)].myPossibleEvents.Add(NotFoundJobEvent);
                myButtonEvents[(int)GetEventFromJob((Job)i)].ChanseOfHappeningOutof100.Add(100 - myJobStatistics[i].myJobChancePlayer);
            }
        }



        //Event DontGetChild = factory.CreateEvent("Incompetent Fuck", "You Missed your wifes pussy with your nut every day for a year lmao, sad tbh", EmptyImpact);
        //DontGetChild.AddEventDecision("DamnSauce", EmptyImpact);
        //DontGetChild.SetCanvasAndButton(EventCanvas, EventButton, EventPanel);
        //myButtonEvents[(int)EventType.Child].myPossibleEvents.Add(DontGetChild);
        //myButtonEvents[(int)EventType.Child].ChanseOfHappeningOutof100.Add(50);
    }

    public void ActivateEvent(int index)
    {
        CalculateEventChanse(index).Activate();
        UIHub.Instance.CloseMenus();
    }

    public Event CalculateEventChanse(int index)
    {
        {
            int Number = Random.Range(0, 100);
            int totaalChance = 0;
            int newIndex = 0;
            foreach (int chance in myButtonEvents[index].ChanseOfHappeningOutof100)
            {
                totaalChance += chance;
                if (Number < totaalChance)
                {
                    myChosenEvent[index] = myButtonEvents[index].myPossibleEvents[newIndex];
                    break;
                }
                newIndex++;
            }
        }
        return myChosenEvent[index];
    }

    public Character CreateRandomCharacter(AgeBracket anAge)
    {
        int randomGender = Random.Range(0, 1);
        int age = GetRandomAgeFromAgeBracket(anAge);
        Job job = GetRandomJob();
        Character randomCharacter = new Character();
        randomCharacter.CreateCharacter(age, 4, GetRandomName((Gender)randomGender, NameRegion.Nordic), job, (Gender)randomGender);
        return randomCharacter;
    }

    public int GetRandomAgeFromAgeBracket(AgeBracket anAge)
    {
        int low = 0;
        int high = 0;
        switch(anAge)
        {
            case AgeBracket.Child:
                {
                    low = 1;
                    high = ChildTopAge;
                    break;
                }
            case AgeBracket.YoungAdult:
                {
                    low = ChildTopAge + 1;
                    high = YoungAdultTopAge;
                    break;
                }
            case AgeBracket.Adult:
                {
                    low = YoungAdultTopAge + 1;
                    high = AdultTopAge;
                    break;
                }
            case AgeBracket.Senior:
                {
                    low = AdultTopAge + 1;
                    high = 72;
                    break;
                }
        }
        return Random.Range(low, high);
    }

    private void SetStatistics()
    {
        myJobStatistics = new JobStatistics[(int)Job.Nothing + 1];

        myJobStatistics[(int)Job.Peasant].myJobChance = 15;
        myJobStatistics[(int)Job.Smith].myJobChance = 7;
        myJobStatistics[(int)Job.Fisher].myJobChance = 10;
        myJobStatistics[(int)Job.Baker].myJobChance = 2;
        myJobStatistics[(int)Job.ShoeMaker].myJobChance = 7;
        myJobStatistics[(int)Job.Carpenter].myJobChance = 7;
        myJobStatistics[(int)Job.Merchant].myJobChance = 7;
        myJobStatistics[(int)Job.Hunter].myJobChance = 7;
        myJobStatistics[(int)Job.Miner].myJobChance = 7;
        myJobStatistics[(int)Job.Mercenary].myJobChance = 7;
        myJobStatistics[(int)Job.Guard].myJobChance = 2;
        myJobStatistics[(int)Job.Sailor].myJobChance = 2;
        myJobStatistics[(int)Job.Soldier].myJobChance = 5;
        myJobStatistics[(int)Job.Monk].myJobChance = 3;
        myJobStatistics[(int)Job.Priest].myJobChance = 2;
        myJobStatistics[(int)Job.Bishop].myJobChance = 1;
        myJobStatistics[(int)Job.Knight].myJobChance = 1;
        myJobStatistics[(int)Job.Noblemen].myJobChance = 1;
        myJobStatistics[(int)Job.Nothing].myJobChance = 7;



        myJobStatistics[(int)Job.Peasant].myDeathChance = 0;
        myJobStatistics[(int)Job.Smith].myDeathChance = 1;
        myJobStatistics[(int)Job.Fisher].myDeathChance = 2;
        myJobStatistics[(int)Job.Baker].myDeathChance = 0;
        myJobStatistics[(int)Job.ShoeMaker].myDeathChance = 0;
        myJobStatistics[(int)Job.Carpenter].myDeathChance = 0;
        myJobStatistics[(int)Job.Merchant].myDeathChance = 2;
        myJobStatistics[(int)Job.Hunter].myDeathChance = 2;
        myJobStatistics[(int)Job.Miner].myDeathChance = 3;
        myJobStatistics[(int)Job.Mercenary].myDeathChance = 0;
        myJobStatistics[(int)Job.Guard].myDeathChance = 3;
        myJobStatistics[(int)Job.Sailor].myDeathChance = 2;
        myJobStatistics[(int)Job.Soldier].myDeathChance = 0;
        myJobStatistics[(int)Job.Monk].myDeathChance = 0;
        myJobStatistics[(int)Job.Priest].myDeathChance = 0;
        myJobStatistics[(int)Job.Bishop].myDeathChance = 0;
        myJobStatistics[(int)Job.Knight].myDeathChance = 0;
        myJobStatistics[(int)Job.Noblemen].myDeathChance = 0;
        myJobStatistics[(int)Job.Nothing].myDeathChance = 0;



        myJobStatistics[(int)Job.Peasant].myJobChancePlayer = 50;
        myJobStatistics[(int)Job.Smith].myJobChancePlayer = 20;
        myJobStatistics[(int)Job.Fisher].myJobChancePlayer = 20;
        myJobStatistics[(int)Job.Baker].myJobChancePlayer = 10;
        myJobStatistics[(int)Job.ShoeMaker].myJobChancePlayer = 30;
        myJobStatistics[(int)Job.Carpenter].myJobChancePlayer = 20;
        myJobStatistics[(int)Job.Merchant].myJobChancePlayer = 10;
        myJobStatistics[(int)Job.Hunter].myJobChancePlayer = 30;
        myJobStatistics[(int)Job.Miner].myJobChancePlayer = 50;
        myJobStatistics[(int)Job.Mercenary].myJobChancePlayer = 20;
        myJobStatistics[(int)Job.Guard].myJobChancePlayer = 10;
        myJobStatistics[(int)Job.Sailor].myJobChancePlayer = 10;
        myJobStatistics[(int)Job.Soldier].myJobChancePlayer = 50;
        myJobStatistics[(int)Job.Monk].myJobChancePlayer = 100;
        myJobStatistics[(int)Job.Priest].myJobChancePlayer = 10;
        myJobStatistics[(int)Job.Bishop].myJobChancePlayer = 3;
        myJobStatistics[(int)Job.Knight].myJobChancePlayer = 0;
        myJobStatistics[(int)Job.Noblemen].myJobChancePlayer = 0;
        myJobStatistics[(int)Job.Nothing].myJobChancePlayer = 0;



        myJobStatistics[(int)Job.Peasant].myJobSalary = 20;
        myJobStatistics[(int)Job.Smith].myJobSalary = 30;
        myJobStatistics[(int)Job.Fisher].myJobSalary = 30;
        myJobStatistics[(int)Job.Baker].myJobSalary = 50;
        myJobStatistics[(int)Job.ShoeMaker].myJobSalary = 20;
        myJobStatistics[(int)Job.Carpenter].myJobSalary = 30;
        myJobStatistics[(int)Job.Merchant].myJobSalary = 50;
        myJobStatistics[(int)Job.Hunter].myJobSalary = 20;
        myJobStatistics[(int)Job.Miner].myJobSalary = 20;
        myJobStatistics[(int)Job.Mercenary].myJobSalary = 50;
        myJobStatistics[(int)Job.Guard].myJobSalary = 100;
        myJobStatistics[(int)Job.Sailor].myJobSalary = 40;
        myJobStatistics[(int)Job.Soldier].myJobSalary = 20;
        myJobStatistics[(int)Job.Monk].myJobSalary = 0;
        myJobStatistics[(int)Job.Priest].myJobSalary = 0;
        myJobStatistics[(int)Job.Bishop].myJobSalary = 0;
        myJobStatistics[(int)Job.Knight].myJobSalary = 0;
        myJobStatistics[(int)Job.Noblemen].myJobSalary = 0;
        myJobStatistics[(int)Job.Nothing].myJobSalary = 0;
    }

    public Job GetRandomJob()
    {
        int jobChance = Random.Range(1, 100);
        int thing = 0;
        for(int i = 0; i < myJobStatistics.Length; i++)
        {
            thing += myJobStatistics[i].myJobChance;
            if(jobChance < thing)
            {
                return (Job)myJobStatistics[i].myJobChance;
            }
        }
        return Job.Nothing;
    }

    public EventType GetEventFromJob(Job aJob)
    {
        switch(aJob)
        {
            case GameHub.Job.Baker:
                {
                    return EventType.TryBecomeBaker;
                }
            case GameHub.Job.Bishop:
                {
                    return EventType.TryBecomeBishop;
                }
            case GameHub.Job.Carpenter:
                {
                    return EventType.TryBecomeCarpenter;
                }
            case GameHub.Job.Fisher:
                {
                    return EventType.TryBecomeFisher;
                }
            case GameHub.Job.Guard:
                {
                    return EventType.TryBecomeGuard;
                }
            case GameHub.Job.Hunter:
                {
                    return EventType.TryBecomeHunter;
                }
            case GameHub.Job.King:
                {
                    return EventType.TryBecomeKing;
                }
            case GameHub.Job.Knight:
                {
                    return EventType.TryBecomeKnight;
                }
            case GameHub.Job.Mercenary:
                {
                    return EventType.TryBecomeMercenary;
                }
            case GameHub.Job.Merchant:
                {
                    return EventType.TryBecomeMerchant;
                }
            case GameHub.Job.Miner:
                {
                    return EventType.TryBecomeBaker;
                }
            case GameHub.Job.Monk:
                {
                    return EventType.TryBecomeMonk;
                }
            case GameHub.Job.Noblemen:
                {
                    return EventType.TryBecomeNoblemen;
                }
            case GameHub.Job.Peasant:
                {
                    return EventType.TryBecomePeasant;
                }
            case GameHub.Job.Priest:
                {
                    return EventType.TryBecomePriest;
                }
            case GameHub.Job.Sailor:
                {
                    return EventType.TryBecomeSailor;
                }
            case GameHub.Job.ShoeMaker:
                {
                    return EventType.TryBecomeShoeMaker;
                }
            case GameHub.Job.Smith:
                {
                    return EventType.TryBecomeSmith;
                }
            case GameHub.Job.Soldier:
                {
                    return EventType.TryBecomeSoldier;
                }
        }
        return EventType.TryBecomeBaker;
    }

}
