using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security;
using UnityEditor;
using UnityEngine;

public class ButtonEvent
{
    public List<Event> myPossibleEvents;
    public List<int> ChanseOfHappeningOutof100;
}

public class JobButtonEvent
{
    public Event AcceptanceEvent;
    public Event DeniedEvent;
    public Jobb jobb;
}

public class ChanceEvent
{
    public Event myEvent;
    public List<Event> myResultEvents = new List<Event>();
    public List<int> myResultEventsIndex = new List<int>();
    public Jobb selectedJobOption;
    public int relationAmount;
    public int Age;
    public EventScriptableObject.AgeRequierment myAgeRequierment;
    //public Jobb DependableJobOption;
    //public GameHub.RelationType selectedRelationDependable;
    //public EventScriptableObject.AgeRequierment myDependableAgeRequierment;
    //public int DependableCharacterAge;
    //public bool DependableCharacterFlag;
    //public bool RelationJobDependant;
    //public bool RelationAgeDependant;
    public bool JobDependant;
    public bool AgeDependant;
    public bool CanBeGottenAgain;
    public bool hasBeenGotten;
    public bool IsSocialClassDependant;
    public bool IsCharacteristicDependant;
    public Characteristic characteristic;
    public float ChanceOfHappening;
    public GameHub.SocialClass socialClass;

    public string EventTitle;
    public string EventText;
    public List<string> buttonTexts;
    public List<ButtonAlternativeResultData> myButtonAlternatives = new List<ButtonAlternativeResultData>();
    //public List<ResultDataRegistry> buttonResults = new List<ResultDataRegistry>();
    public List<DependableData> dependables = new List<DependableData>();
}


public class GameHub : MonoBehaviour
{
    public enum SocialClass
    {
        Criminal,
        Commoner,
        Nobel,
        Royal
    };
    public enum Gender
    {
        Boy,
        Girl
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

    public enum EventResult
    {
        Death,
        Money,
        Income,
        Land,
        Job,
        Character,
        Characteristic
    };


    private static GameHub instance;

    public Dictionary<string, string> namePlaceholders = new Dictionary<string, string>()
    {
        {"Brother", ""},
        {"sister", ""},
        {"Mother", ""},
        {"Father", ""},
        {"Wife", ""},
        {"Child", ""},
    };
    public PlayerHub myPlayer;
    public Map myMap;
    public GameObject EventButton;
    public GameObject EventCanvas;
    public GameObject EventPanel;
    public GameObject PauseMenu;
    public bool isPaused = false;
    List<ButtonEvent> myButtonEvents = new List<ButtonEvent>();
    private List<JobButtonEvent> myJobEvents = new List<JobButtonEvent>();
    private List<ChanceEvent> myChanceEvents = new List<ChanceEvent>();
    private List<int> myChanceIndexes = new List<int>();
    Event[] myChosenEvent;
    public int ChildTopAge;
    public int YoungAdultTopAge;
    public int AdultTopAge;
    public List<Character> allCharacters;
    private KiingdomManager myKingdomManager;
    public NameScriptableObject myNames;
    public DataScriptableObject myDataScriptableObject;
    public List<string> myBoyNames = new List<string>();
    public List<string> myGirlNames = new List<string>();
    public EventRegistryScriptableObject myEvents;
    private NameRegion myHeadRegion;

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
        CreateEvents();
        ReadNames();
        ChildTopAge = 12;
        YoungAdultTopAge = 24;
        AdultTopAge = 36;
    }

    public void LoadPreviousGame()
    {
        if (MenuData.ShouldLoad)
        {
            SaveData loaded = SaveSystem.Load();

            myPlayer.myCharacter = loaded.playerCharacter;
            myPlayer.myHeir = loaded.playerHeir;
            myPlayer.myInteraction = loaded.playerInteraction;

            allCharacters = loaded.allCharacters;

            myHeadRegion = loaded.nameRegion;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            PauseMenu.SetActive(isPaused);
        }
    }

    public void ResumePaused()
    {
        PauseMenu.SetActive(false);
        isPaused = false;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SaveGame()
    {
        SaveSystem.Save(GetPlayer(), this);
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

    public NameRegion GetRegion()
    {
        return myHeadRegion;
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
        var playerCharacter = myPlayer.GetPlayerCharacter();

        myChanceIndexes = myChanceIndexes.OrderBy(x => Random.value).ToList();

        foreach (var index in myChanceIndexes)
        {
            var chanceEvent = myChanceEvents[index];

            if (!CanActivateEvent(chanceEvent, playerCharacter))
                continue;

            chanceEvent.myEvent.Activate();
            return;
        }
    }
    private bool CanActivateEvent(ChanceEvent chanceEvent, Character player)
    {
        if (!CheckSocialClass(chanceEvent, player)) return false;
        if (!CheckJob(chanceEvent, player)) return false;
        if (!CheckAge(chanceEvent, player)) return false;
        if (!CheckCharacteristics(chanceEvent, player)) return false;
        if (!CheckChance(chanceEvent)) return false;
        if (!CheckDependableCharacters(chanceEvent, player)) return false;

        return true;
    }

    private bool CheckSocialClass(ChanceEvent e, Character player)
    {
        return !e.IsSocialClassDependant || e.socialClass == player.mySocialClass;
    }

    private bool CheckJob(ChanceEvent e, Character player)
    {
        return !e.JobDependant || player.myJob.myJob == e.selectedJobOption.myJob;
    }
    private bool CheckAge(ChanceEvent e, Character player)
    { 
        if (!e.AgeDependant) return true;

        switch (e.myAgeRequierment)
        {
            case EventScriptableObject.AgeRequierment.Exact:
                return player.myAge == e.Age;

            case EventScriptableObject.AgeRequierment.Below:
                return player.myAge <= e.Age;

            case EventScriptableObject.AgeRequierment.Above:
                return player.myAge >= e.Age;

            default:
                return true;
        }
    }

    private bool CheckCharacteristics(ChanceEvent e, Character player)
    {
        if (!e.IsCharacteristicDependant) return true;

        return player.myCharacteristics.Any(c =>
            c.myCharacteristic == e.characteristic.myCharacteristic);
    }

    private bool CheckChance(ChanceEvent e)
    {
        return e.ChanceOfHappening >= Random.Range(0, 100);
    }

    private bool CheckDependableCharacters(ChanceEvent e, Character player)
    {
        bool areCharactersCorrect = true;
        for(int i = 0; i < e.dependables.Count; i++)
        {
            var characters = player.GetCharacterFromRelationType(e.dependables[i].myRelationType);
            var selected = characters.FirstOrDefault();
            if(e.dependables[i].haveOrNotHaveFlag == false && characters.Count == 0)
            {
                continue;
            }
            if (characters.Count != e.dependables[i].amount) areCharactersCorrect = false;

            if (selected == null) areCharactersCorrect = false;
            if (e.relationAmount == 0) areCharactersCorrect = false;

            if (e.dependables[i].AgeDependant && !CheckDependableAge(e.dependables[i].AgeRequierment, e.dependables[i].Age, selected))
                areCharactersCorrect = false;

            if (e.dependables[i].JobDependant && selected.myJob.myJob != e.dependables[i].ChosenJobb.myJob)
                areCharactersCorrect = false;
        }
        return areCharactersCorrect;
    }

    private bool CheckDependableAge(EventScriptableObject.AgeRequierment ageRequierment, int age, Character character)
    {
        switch (ageRequierment)
        {
            case EventScriptableObject.AgeRequierment.Exact:
                return character.myAge == age;

            case EventScriptableObject.AgeRequierment.Below:
                return character.myAge <= age;

            case EventScriptableObject.AgeRequierment.Above:
                return character.myAge >= age;

            default:
                return true;
        }
    }


    //public void UpdateEvents()
    //{
    //    myChanceIndexes = myChanceIndexes.OrderBy(x => Random.value).ToList();
    //    for (int i = 0; i < myChanceIndexes.Count; i++)
    //    {
    //        bool shouldActivateEvent = true;
    //        if(myChanceEvents[myChanceIndexes[i]].IsSocialClassDependant && myChanceEvents[myChanceIndexes[i]].socialClass != myPlayer.GetPlayerCharacter().mySocialClass)
    //        {
    //            shouldActivateEvent = false;
    //        }
    //        if (myChanceEvents[myChanceIndexes[i]].CanBeGottenAgain)
    //        {

    //        }
    //        if (myChanceEvents[myChanceIndexes[i]].JobDependant && myPlayer.GetPlayerCharacter().myJob.myJob != myChanceEvents[myChanceIndexes[i]].selectedJobOption.myJob)
    //        {
    //            shouldActivateEvent = false;
    //        }
    //        if (myChanceEvents[myChanceIndexes[i]].AgeDependant)
    //        {
    //            if (myChanceEvents[myChanceIndexes[i]].myAgeRequierment == EventScriptableObject.AgeRequierment.Exact && myPlayer.GetPlayerCharacter().myAge != myChanceEvents[myChanceIndexes[i]].Age)
    //            {
    //                shouldActivateEvent = false;
    //            }
    //            else if(myChanceEvents[myChanceIndexes[i]].myAgeRequierment == EventScriptableObject.AgeRequierment.Below && myPlayer.GetPlayerCharacter().myAge > myChanceEvents[myChanceIndexes[i]].Age)
    //            {
    //                shouldActivateEvent = false;
    //            }
    //            else if(myChanceEvents[myChanceIndexes[i]].myAgeRequierment == EventScriptableObject.AgeRequierment.Above && myPlayer.GetPlayerCharacter().myAge < myChanceEvents[myChanceIndexes[i]].Age)
    //            {
    //                shouldActivateEvent = false;
    //            }
    //        }
    //        if (myChanceEvents[myChanceIndexes[i]].IsCharacteristicDependant)
    //        {
    //            bool hasCharacteristic = false;
    //            for(int p = 0; p < myPlayer.GetPlayerCharacter().myCharacteristics.Count; p++)
    //            {
    //                if(myChanceEvents[myChanceIndexes[i]].characteristic.myCharacteristic == myPlayer.GetPlayerCharacter().myCharacteristics[p].myCharacteristic)
    //                {
    //                    hasCharacteristic = true;
    //                }
    //            }
    //            if(hasCharacteristic == false)
    //            {
    //                shouldActivateEvent = false;
    //            }
    //        }
    //        if (myChanceEvents[myChanceIndexes[i]].ChanceOfHappening < Random.Range(0, 100))
    //        {
    //            shouldActivateEvent = false;
    //        }
    //        if(myChanceEvents[myChanceIndexes[i]].DependableCharacterFlag)
    //        {
    //            Character selectedCharacter = myPlayer.GetPlayerCharacter().GetCharacterFromRelationType(myChanceEvents[myChanceIndexes[i]].selectedRelationDependable)[0];
    //            if(selectedCharacter != null)
    //            {
    //                if(myChanceEvents[myChanceIndexes[i]].relationAmount == 0)
    //                {
    //                    shouldActivateEvent = false;
    //                }
    //                if(myChanceEvents[myChanceIndexes[i]].RelationAgeDependant)
    //                {
    //                    if (myChanceEvents[myChanceIndexes[i]].myDependableAgeRequierment == EventScriptableObject.AgeRequierment.Exact && selectedCharacter.myAge != myChanceEvents[myChanceIndexes[i]].DependableCharacterAge)
    //                    {
    //                        shouldActivateEvent = false;
    //                    }
    //                    else if (myChanceEvents[myChanceIndexes[i]].myDependableAgeRequierment == EventScriptableObject.AgeRequierment.Below && selectedCharacter.myAge > myChanceEvents[myChanceIndexes[i]].DependableCharacterAge)
    //                    {
    //                        shouldActivateEvent = false;
    //                    }
    //                    else if (myChanceEvents[myChanceIndexes[i]].myDependableAgeRequierment == EventScriptableObject.AgeRequierment.Above && selectedCharacter.myAge < myChanceEvents[myChanceIndexes[i]].DependableCharacterAge)
    //                    {
    //                        shouldActivateEvent = false;
    //                    }
    //                }
    //                if(selectedCharacter.myJob.myJob != myChanceEvents[myChanceIndexes[i]].DependableJobOption.myJob)
    //                {
    //                    shouldActivateEvent = false;
    //                }
    //            }
    //            else
    //            {
    //                shouldActivateEvent = false;
    //            }
    //        }
    //        if (shouldActivateEvent)
    //        {
    //            myChanceEvents[myChanceIndexes[i]].myEvent.Activate();
    //            //myChanceEvents = myChanceEvents.OrderBy(x => Random.value).ToList();
    //            return;
    //        }
    //    }
    //}

    void ReadNames()
    {
        if(myNames.myRegions.Count <= 0)
        {
            Debug.Log("There are no names??");
        }
        int selectedRegion = Random.Range(0, myNames.myRegions.Count - 1);
        myHeadRegion = myNames.myRegions[selectedRegion];

        myGirlNames = myHeadRegion.FemaleNames;
        myBoyNames = myHeadRegion.MaleNames;
    }

    public string GetRandomName(Gender aGender)
    {
        string name;
        if(aGender == Gender.Boy)
        {
            name = myBoyNames[Random.Range(0, myBoyNames.Count - 1)];
        }
        else 
        {
            name = myGirlNames[Random.Range(0, myGirlNames.Count - 1)];
        }
        return name;
    }

    public void CreateEvents()
    {

        myChosenEvent = new Event[(int)EventType.Count];
        for(int i = 0; i < (int)EventType.Count; i++)
        {
            myButtonEvents.Add(new ButtonEvent());
            myButtonEvents[i].ChanseOfHappeningOutof100 = new List<int>();
            myButtonEvents[i].myPossibleEvents = new List<Event>();
        }

        EventFactory factory = gameObject.AddComponent<EventFactory>();

        for(int i = 0; i < myEvents.Events.Count; i++)
        {
            ChanceEvent aChanceEvent = new ChanceEvent();
            Event chanceEvent = factory.CreateEvent(myEvents.Events[i].EventTitle, myEvents.Events[i].EventText);
            myChanceEvents.Add(aChanceEvent);
            for(int j = 0; j < myEvents.Events[i].buttonTexts.Count; j++)
            {
                float totalWeight = 0.0f;
                ButtonAlternativeResults result = null;
                if(myEvents.Events[i].myButtonAlternatives.Count != 0)
                {
                    for(int k = 0; k < myEvents.Events[i].myButtonAlternatives.Count; k++)
                    {
                        totalWeight += myEvents.Events[i].myButtonAlternatives[j].myButtonResults[k].ResultChanceOfHappening;
                    }
                    float randomPoint = Random.Range(0.0f, totalWeight);
                    float current = 0.0f;
                    for (int k = 0; k < myEvents.Events[i].myButtonAlternatives.Count; k++)
                    {
                        current += myEvents.Events[i].myButtonAlternatives[j].myButtonResults[k].ResultChanceOfHappening;
                        if(randomPoint <= current)
                        {
                            result = myEvents.Events[i].myButtonAlternatives[j].myButtonResults[k];
                            break;
                        }
                    }

                }

                if (result != null)
                {
                    Event chanceResultEvent = factory.CreateEvent(result.resultEventTitle, result.resultEventText);
                    chanceResultEvent.SetCanvasAndButton(EventCanvas, EventButton, EventPanel);
                    myChanceEvents[i].myResultEvents.Add(chanceResultEvent);
                    chanceResultEvent.AddEventDecision(result.resultButtonText, () =>
                    {
                        chanceResultEvent.DeActivate();
                    });
                }
                int currentI = i;
                int currentJ = j;
                chanceEvent.AddEventDecision(myEvents.Events[i].buttonTexts[j], () => {
                    myPlayer.ActivateEvent(EventType.Crusade);
                    if(myEvents.Events[currentI].myButtonAlternatives.Count != 0)
                    {
                        myChanceEvents[currentI].myResultEvents[currentJ].Activate();
                    }
                    chanceEvent.DeActivate();
                });
            }
            chanceEvent.SetCanvasAndButton(EventCanvas, EventButton, EventPanel);
            aChanceEvent.myEvent = chanceEvent;
            aChanceEvent.selectedJobOption = myEvents.Events[i].selectedJobOption;
            aChanceEvent.relationAmount = myEvents.Events[i].relationAmount;
            aChanceEvent.Age = myEvents.Events[i].Age;
            //aChanceEvent.DependableJobOption = myEvents.Events[i].DependableJobOption;
            //aChanceEvent.selectedRelationDependable = myEvents.Events[i].selectedRelationDependable;
            //aChanceEvent.DependableCharacterAge = myEvents.Events[i].DependableCharacterAge;
            //aChanceEvent.DependableCharacterFlag = myEvents.Events[i].DependableCharacterFlag;
            //aChanceEvent.RelationJobDependant = myEvents.Events[i].RelationJobDependant;
            //aChanceEvent.RelationAgeDependant = myEvents.Events[i].RelationAgeDependant;
            //aChanceEvent.myDependableAgeRequierment = myEvents.Events[i].myDependableAgeRequierment;
            aChanceEvent.JobDependant = myEvents.Events[i].JobDependant;
            aChanceEvent.AgeDependant = myEvents.Events[i].AgeDependant;
            aChanceEvent.CanBeGottenAgain = myEvents.Events[i].CanBeGottenAgain;
            aChanceEvent.ChanceOfHappening = myEvents.Events[i].ChanceOfHappening;
            aChanceEvent.EventTitle = myEvents.Events[i].EventTitle;
            aChanceEvent.EventText = myEvents.Events[i].EventText;
            aChanceEvent.buttonTexts = myEvents.Events[i].buttonTexts;
            //aChanceEvent.buttonResults = myEvents.Events[i].buttonResults;
            aChanceEvent.myButtonAlternatives = myEvents.Events[i].myButtonAlternatives;
            aChanceEvent.myAgeRequierment = myEvents.Events[i].myAgeRequierment;
            aChanceEvent.socialClass = myEvents.Events[i].socialClass;
            aChanceEvent.IsSocialClassDependant = myEvents.Events[i].IsSocialClassDependant;
            aChanceEvent.IsCharacteristicDependant = myEvents.Events[i].IsCharacteristicDependant;
            aChanceEvent.characteristic = myEvents.Events[i].myChosenCharacteristic;
            aChanceEvent.dependables = myEvents.Events[i].myDependables;
        }
        for (int i = 0; i < myEvents.Events.Count; i++)
        {
            myChanceIndexes.Add(i);
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

        for(int i = 0; i < myDataScriptableObject.Jobbs.Count; i++)
        {
            myJobEvents.Add(new JobButtonEvent());
            if (myDataScriptableObject.Jobbs[i].IsVisibleInJobMenu)
            {
                int index = i;
                Event jobEvent = factory.CreateEvent("They wanted you, Congrats!", "There was a spot open for you as a " + myDataScriptableObject.Jobbs[index]);
                jobEvent.AddEventDecision("Ill take it!", () =>
                {
                    myPlayer.ActivateJobEvent(myDataScriptableObject.Jobbs[index]);
                    jobEvent.DeActivate();
                });
                jobEvent.AddEventDecision("I might find something better", () => { jobEvent.DeActivate(); });
                jobEvent.SetCanvasAndButton(EventCanvas, EventButton, EventPanel);
                //myJobEvents.Add(new JobButtonEvent());
                myJobEvents[i].AcceptanceEvent = jobEvent;
                myJobEvents[i].DeniedEvent = NotFoundJobEvent;
                myJobEvents[i].jobb = myDataScriptableObject.Jobbs[i];

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

    public void ActivateJobEvent(int index)
    {
        int Number = Random.Range(0, 100);
        if(Number < myJobEvents[index].jobb.myAcceptanceRate)
        {
            myJobEvents[index].AcceptanceEvent.Activate();
        }
        else
        {
            myJobEvents[index].DeniedEvent.Activate();
        }
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

    public Character CreateRandomCharacter(int age)
    {
        int randomGender = Random.Range(0, 1);
        Jobb job = GetRandomJob();
        Character randomCharacter = new Character();
        randomCharacter.CreateCharacter(age, 4, GetRandomName((Gender)randomGender), job, (Gender)randomGender);
        return randomCharacter;
    }

    public Jobb GetRandomJob()
    {
        float jobChance = Random.Range(1, 100);
        for(int i = 0; i < myDataScriptableObject.Jobbs.Count; i++)
        {
            if(jobChance < myDataScriptableObject.Jobbs[i].myAcceptanceRate && myDataScriptableObject.Jobbs[i].CanCharactersRandomlyGet)
            {
                return myDataScriptableObject.Jobbs[i];
            }
        }
        return null;
    }

    //public EventType GetEventFromJob(Job aJob)
    //{
    //    switch(aJob)
    //    {
    //        case GameHub.Job.Baker:
    //            {
    //                return EventType.TryBecomeBaker;
    //            }
    //        case GameHub.Job.Bishop:
    //            {
    //                return EventType.TryBecomeBishop;
    //            }
    //        case GameHub.Job.Carpenter:
    //            {
    //                return EventType.TryBecomeCarpenter;
    //            }
    //        case GameHub.Job.Fisher:
    //            {
    //                return EventType.TryBecomeFisher;
    //            }
    //        case GameHub.Job.Guard:
    //            {
    //                return EventType.TryBecomeGuard;
    //            }
    //        case GameHub.Job.Hunter:
    //            {
    //                return EventType.TryBecomeHunter;
    //            }
    //        case GameHub.Job.King:
    //            {
    //                return EventType.TryBecomeKing;
    //            }
    //        case GameHub.Job.Knight:
    //            {
    //                return EventType.TryBecomeKnight;
    //            }
    //        case GameHub.Job.Mercenary:
    //            {
    //                return EventType.TryBecomeMercenary;
    //            }
    //        case GameHub.Job.Merchant:
    //            {
    //                return EventType.TryBecomeMerchant;
    //            }
    //        case GameHub.Job.Miner:
    //            {
    //                return EventType.TryBecomeBaker;
    //            }
    //        case GameHub.Job.Monk:
    //            {
    //                return EventType.TryBecomeMonk;
    //            }
    //        case GameHub.Job.Noblemen:
    //            {
    //                return EventType.TryBecomeNoblemen;
    //            }
    //        case GameHub.Job.Peasant:
    //            {
    //                return EventType.TryBecomePeasant;
    //            }
    //        case GameHub.Job.Priest:
    //            {
    //                return EventType.TryBecomePriest;
    //            }
    //        case GameHub.Job.Sailor:
    //            {
    //                return EventType.TryBecomeSailor;
    //            }
    //        case GameHub.Job.ShoeMaker:
    //            {
    //                return EventType.TryBecomeShoeMaker;
    //            }
    //        case GameHub.Job.Smith:
    //            {
    //                return EventType.TryBecomeSmith;
    //            }
    //        case GameHub.Job.Soldier:
    //            {
    //                return EventType.TryBecomeSoldier;
    //            }
    //    }
    //    return EventType.TryBecomeBaker;
    //}

}
