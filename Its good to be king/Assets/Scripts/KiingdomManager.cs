using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using csDelaunay;

public class KiingdomManager
{
    struct MoreCellData
    {
        public Map.CellData myCell;
        public bool shouldBeRemoved;
    }

    struct Empire
    {
        public List<Kingdom> duchies;
        public Character supremeLeader;
    }


    public int chanceToBeWoman = 15;
    public int smallestCountry = 3;
    List<Kingdom> ExistingCountries;

    public void UpdateKingdoms()
    {
        foreach(Kingdom kingdom in ExistingCountries)
        {
            kingdom.UpdateKingdom();
        }
    }

    public void CreateCountries(int chanceToExpand, int chanceToExpandDecrease)
    {
        ExistingCountries = new List<Kingdom>();
        Map map = GameHub.Instance.myMap;
        Voronoi vornoi = map.GetVoronoi();
        List<MoreCellData> cells = new List<MoreCellData>();
        int index = 0;
        Map.CellData[] cellDatas = map.GetCells();
        for (int i = 0; i < cellDatas.Length; i++)
        {
            cellDatas[i].Index = i;
            if(cellDatas[i].aLandType != Map.LandTypes.Water)
            {
                cellDatas[i].isOwned = false;
                cellDatas[i].distributionIndex = index;
                MoreCellData expandedCell = new MoreCellData();
                expandedCell.myCell = cellDatas[i];
                expandedCell.shouldBeRemoved = false;
                cells.Add(expandedCell);
                index++;
            }
        }
        int bruh = 0;
        float distanceTooFar = (Mathf.Sqrt(map.PolygonNumber) * (map.PixelWidth / map.PixelHeight) * 2.0f);
        index = 0;
        while (cells.Count > 0)
        {
            bruh++;
            int startCellIndex = cellDatas[Random.Range(0, cells.Count - 1)].distributionIndex;
            Kingdom kd = new Kingdom();
            kd.CreateKingdom();
            kd.myColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            kd.kingdomIndex = index;


            CreateKing(kd);
            SearchForExpansion(chanceToExpand, chanceToExpandDecrease, 0, kd, startCellIndex, vornoi, cells, cellDatas, distanceTooFar);
            ExistingCountries.Add(kd);
            int removed = 0;
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].shouldBeRemoved)
                {
                    removed++;
                    cells.Remove(cells[i]);
                    i--;
                }
                else
                {
                    cellDatas[cells[i].myCell.Index].distributionIndex -= removed;
                }
            }
            index++;
        }

        DestroySmalledCountries(vornoi, cellDatas);
    }

    private void DestroySmalledCountries(Voronoi voronoi, Map.CellData[] cells)
    {
        foreach(Kingdom kingdom in ExistingCountries)
        {
            if(kingdom.myLands.Count < smallestCountry)
            {
                int InheritedkingdomIndex = 0;
                foreach(Map.CellData celldata in kingdom.myLands)
                {
                    foreach(Site site in voronoi.SitesIndexedByLocation[voronoi.SiteCoords()[celldata.CordIndex]].NeighborSites())
                    {
                        if(cells[site.SiteIndex].Owner != kingdom.myOwner)
                        {
                            if(ExistingCountries[cells[site.SiteIndex].kingdomIndex].myLands.Count > smallestCountry)
                            {
                                InheritedkingdomIndex = cells[site.SiteIndex].kingdomIndex;
                                break;
                            }
                        }
                    }
                }
                for (int i = 0; i < kingdom.myLands.Count; i++)
                {
                    cells[kingdom.myLands[i].Index].Owner = ExistingCountries[InheritedkingdomIndex].myOwner;
                    ExistingCountries[InheritedkingdomIndex].myLands.Add(cells[kingdom.myLands[i].Index]);
                }
                kingdom.hasBeenDefeated = true;
            }
        }

        for (int i = ExistingCountries.Count - 1; i > 0; i--)
        {
            if (ExistingCountries[i].hasBeenDefeated)
            {
                ExistingCountries[i] = null;
                ExistingCountries.Remove(ExistingCountries[i]);
            }
        }
    }

    public void DrawCountries()
    {
        Map map = GameHub.Instance.myMap;
        foreach (Kingdom kingdom in ExistingCountries)
        {
           map.DrawBorders(kingdom);
        }
    }

    private void CreateKing(Kingdom kd)
    {
        Character king = new Character();
        king.myAge = Random.Range(13, 84);
        int WomanOrManChance = Random.Range(0, 100);
        if (WomanOrManChance < chanceToBeWoman)
        {
            king.myGender = GameHub.Gender.Girl;
        }
        else
        {
            king.myGender = GameHub.Gender.Boy;
        }
        kd.myNameRegion = GetRandomRegion();
        king.myName = GameHub.Instance.GetRandomName(king.myGender, kd.myNameRegion);
        king.myJob = GameHub.Job.King;
        kd.myOwner = king;
    }

    private GameHub.NameRegion GetRandomRegion()
    {
        int regionChance = Random.Range(0, 100);
        GameHub.NameRegion nameRegion = GameHub.NameRegion.SheeshMama;
        if(regionChance < 45)
        {
            nameRegion = GameHub.NameRegion.Nordic;
        }
        else if(regionChance < 90)
        {
            nameRegion = GameHub.NameRegion.English;
        }
        return nameRegion;
    }

    private void SearchForExpansion(int chanceToExpand, int chanceToExpandDecrease, int depth, Kingdom kingdom, int siteIndex, Voronoi voronoi, List<MoreCellData> cells, Map.CellData[] celldatas, float distanceTooFar)
    {
        List<Map.CellData> cellsToCheck = new List<Map.CellData>();
        int index = 0;
        while(true)
        {
            MoreCellData data = cells[siteIndex];
            data.shouldBeRemoved = true;
            cells[siteIndex] = data;
            celldatas[cells[siteIndex].myCell.Index].isOwned = true;
            celldatas[cells[siteIndex].myCell.Index].Owner = kingdom.myOwner;
            celldatas[cells[siteIndex].myCell.Index].kingdomIndex = kingdom.kingdomIndex;
            kingdom.myLands.Add(celldatas[cells[siteIndex].myCell.Index]);

            foreach(Site site in voronoi.SitesIndexedByLocation[voronoi.SiteCoords()[data.myCell.CordIndex]].NeighborSites())
            {
                if(celldatas[site.SiteIndex].distributionIndex < cells.Count)
                {
                    float randomChance = 0;
                    if(index > 0)
                    {
                        randomChance = Random.Range(0, 100);
                    }

                    if(randomChance < chanceToExpand && !celldatas[site.SiteIndex].isOwned && celldatas[site.SiteIndex].aLandType != Map.LandTypes.Water && !cells[celldatas[site.SiteIndex].distributionIndex].shouldBeRemoved)
                    {
                        Vector2f cellPos = voronoi.SiteCoords()[data.myCell.CordIndex];
                        Vector2f sitePos = voronoi.SiteCoords()[celldatas[site.SiteIndex].CordIndex];
                        if (Vector2.Distance(new Vector2(cellPos.x, cellPos.y), new Vector2(sitePos.x, sitePos.y)) < distanceTooFar)
                        {
                            celldatas[site.SiteIndex].isOwned = true;
                            celldatas[site.SiteIndex].Owner = kingdom.myOwner;
                            cellsToCheck.Add(celldatas[site.SiteIndex]);
                        }
                    }
                }
            }
            if (index >= cellsToCheck.Count)
            {
                break;
            }
            siteIndex = cellsToCheck[index].distributionIndex;
            index++;
        }
    }

}

