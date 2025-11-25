using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kingdom
{
    int Soliders;
    public List<Map.CellData> myLands;
    public Color myColor;
    public Character myOwner;
    public GameHub.NameRegion myNameRegion;
    public int kingdomIndex;
    public bool hasBeenDefeated = false;

    public void CreateKingdom()
    {
        myLands = new List<Map.CellData>();
    }

    public void UpdateKingdom()
    {

    }

}
