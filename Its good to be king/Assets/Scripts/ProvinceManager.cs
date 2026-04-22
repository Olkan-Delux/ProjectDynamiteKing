using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;

public class ProvinceManager : MonoBehaviour
{
    public MapTilesScriptableObject myProvicnes;
    public List<ProvinceState> provinceStates = new List<ProvinceState>();
    public List<KingdomClass> kingdoms = new List<KingdomClass>();
    private Dictionary<Color, ProvinceState> myProvincesByColor = new Dictionary<Color, ProvinceState>();
    public int maxKingdomSize = 1;
    public int minKingdomSize = 1;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < myProvicnes.Provinces.Count; i++)
        {
            provinceStates.Add(new ProvinceState {provinceDefintition = myProvicnes.Provinces[i]});
            myProvincesByColor.Add(ProvinceExtractor.Normalize(myProvicnes.Provinces[i].color), provinceStates[i]);
        }

        for (int i = 0; i < myProvicnes.Provinces.Count; i++)
        {
            for(int j = 0; j < myProvicnes.Provinces[i].neighbors.Count; j++)
            {
                provinceStates[i].myNeightbors.Add(new neighborData {neighbour = provinceStates[myProvicnes.Provinces[i].neighbors[j].connectionId], borderType = myProvicnes.Provinces[i].neighbors[j].borderType});

            }
        }

        GenerateKingdoms();
        SetLookupTexture();
        transform.gameObject.GetComponent<ProvinceExtractor>().SetKingdomSize(kingdoms.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetLookupTexture()
    {
        Texture2D lookup = new Texture2D(provinceStates.Count, 1, TextureFormat.RFloat, false);

        for (int i = 0; i < provinceStates.Count; i++)
        {
            lookup.SetPixel(provinceStates[i].provinceDefintition.id, 0, new Color(provinceStates[i].myKingdom.kingdomId / 255f, 0, 0, 1));
        }

        lookup.Apply();

        transform.gameObject.GetComponent<ProvinceExtractor>().SetLookUpTexture(lookup);
        transform.gameObject.GetComponent<ProvinceExtractor>().SetProvinceCount(provinceStates.Count);
    }

    private void GenerateKingdoms()
    {
        List<ProvinceState> unassigned = new List<ProvinceState>(provinceStates);
        //List<Kingdom> kingdoms = new List<Kingdom>();

        // Shuffle provinces to randomize seed selection
        unassigned = unassigned.OrderBy(x => Random.value).ToList();

        while (unassigned.Count > 0)
        {
            ProvinceState seed = unassigned[0];
            unassigned.RemoveAt(0);

            KingdomClass kingdom = new KingdomClass();
            kingdom.OccupiedProvicnes = new List<ProvinceState>();

            int targetSize = Random.Range(1, 6);

            Queue<ProvinceState> frontier = new Queue<ProvinceState>();
            seed.myKingdom = kingdom;
            kingdom.OccupiedProvicnes.Add(seed);
            seed.isOccupied = true;
            frontier.Enqueue(seed);

            while (kingdom.OccupiedProvicnes.Count < targetSize && frontier.Count > 0)
            {
                ProvinceState current = frontier.Dequeue();

                // Shuffle neighbors for randomness
                var neighbors = current.myNeightbors.OrderBy(x => Random.value);

                foreach (var neighbor in neighbors)
                {
                    if (neighbor.neighbour.isOccupied) continue;

                    float chance = GetExpansionChance(neighbor.borderType);

                    if(Random.value > chance)
                    {
                        continue;
                    }

                    neighbor.neighbour.isOccupied = true;
                    neighbor.neighbour.myKingdom = kingdom;
                    kingdom.OccupiedProvicnes.Add(neighbor.neighbour);
                    frontier.Enqueue(neighbor.neighbour);
                    unassigned.Remove(neighbor.neighbour);

                    if (kingdom.OccupiedProvicnes.Count >= targetSize)
                        break;
                }
            }

            kingdom.name = kingdoms.Count.ToString();
            kingdom.kingdomId = kingdoms.Count;
            kingdoms.Add(kingdom);

        }
    }

    public void SetUpKingdoms()
    {
        for(int i = 0; i < kingdoms.Count; i++)
        {
            kingdoms[i].culture = (KingdomClass.Culture)Random.Range(0, 3);
            kingdoms[i].rulerTrait = (KingdomClass.RulerTrait)Random.Range(0, 5);
            for (int j = 0; j < kingdoms[i].OccupiedProvicnes.Count; j++)
            {
                Province province = kingdoms[i].OccupiedProvicnes[j].provinceDefintition;
                kingdoms[i].Population += Random.Range(province.StartPopulationMin, province.StartPopulationMax);
                kingdoms[i].Iron += Random.Range(province.StartIronMin, province.StartIronMin);
                kingdoms[i].Cattle += Random.Range(province.StartCattleMin, province.StartCattleMax);
                kingdoms[i].Food += Random.Range(province.StartFoodMin, province.StartFoodMax);
                kingdoms[i].Stone += Random.Range(province.StartStoneMín, province.StartStoneMax);
                kingdoms[i].Wood += Random.Range(province.StartWoodMin, province.StartWoodMax);
            }
        }
    }

    public KingdomClass GetKingdomFromColor(Color color)
    {
        if (myProvincesByColor.TryGetValue(ProvinceExtractor.Normalize(color), out ProvinceState province))
        {
            return province.myKingdom;

        }
        else
        {
            return null;
        }
    }

   

    private float GetExpansionChance(borderType border)
    {
        switch(border)
        {
            case borderType.Normal: 
                return 1.0f;
            case borderType.narrow: 
                return 0.5f;
            case borderType.River: 
                return 0.5f;
            case borderType.Mountain: 
                return 0.7f;
            case borderType.Sea: 
                return 0.1f;
            default:
                return 1.0f;
        }
    }
}

public class ProvinceState
{
    public Province provinceDefintition;
    public bool isOccupied = false;
    public KingdomClass myKingdom;
    public Character myLord;
    public List<neighborData> myNeightbors = new List<neighborData>();
}

public class neighborData
{
    public ProvinceState neighbour;
    public borderType borderType;
}

public class KingdomClass
{
    public enum Culture
    {
        English = 0,
        Barbarian = 1,
        MiddleEastern = 2,
        Eastern = 3,
    }
    public enum RulerTrait
    {
        Nothing = 0,
        Aggressive = 1,
        Diplomatic = 2,
        Greedy = 3,
        Paranoid = 4,
        Honorable = 5
    }
    public int kingdomId;
    public string name;
    public Character King;
    public List<ProvinceState> OccupiedProvicnes = new List<ProvinceState>();
    public HashSet<KingdomClass> NeighborKingdoms = new HashSet<KingdomClass>();
    public Dictionary<KingdomClass, KingdomRelation> Relations = new Dictionary<KingdomClass, KingdomRelation>();
    public int Iron;
    public int Food;
    public int Wood;
    public int Stone;
    public int Cattle;
    public int Population;

    public int Knights;
    public int cavalry;
    public int peasants;
    public int mercenaries;
    public int Archers;
    public Culture culture = Culture.English;
    public RulerTrait rulerTrait = RulerTrait.Aggressive;

    public void RecalculateNeightbors()
    {
        NeighborKingdoms.Clear();
        for (int j = 0; j < OccupiedProvicnes.Count; j++)
        {
            for (int k = 0; k < OccupiedProvicnes[j].myNeightbors.Count; k++)
            {
                NeighborKingdoms.Add(OccupiedProvicnes[j].myNeightbors[k].neighbour.myKingdom);
                Relations.Add(OccupiedProvicnes[j].myNeightbors[k].neighbour.myKingdom, new KingdomRelation());
            }
        }
    }

}

public class KingdomRelation
{
    public float Opinion = 0;
}



