using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum borderType
{
    Normal,
    Mountain,
    River,
    Sea,
    narrow
}

[CreateAssetMenu(fileName = "ProvinceData", menuName = "Game/Province Data")]
public class MapTilesScriptableObject : ScriptableObject
{
    public List<Province> Provinces = new List<Province>();
}

[System.Serializable]
public class Province
{
    public int id;
    public Color color;
    public string provinceName;
    public int cityCount = 0;
    public int villageCount = 0;
    public int MaxFarmCount = 0;
    public List<ProvinceConnection> neighbors = new List<ProvinceConnection>();
    public int StartIronMax;
    public int StartIronMin;
    public int StartFoodMax;
    public int StartFoodMin;
    public int StartWoodMax;
    public int StartWoodMin;
    public int StartStoneMax;
    public int StartStoneMín;
    public int StartCattleMax;
    public int StartCattleMin;
    public int StartPopulationMax;
    public int StartPopulationMin;
}

[System.Serializable]
public class ProvinceConnection
{
    public int connectionId = 0;
    public borderType borderType = borderType.Normal;
}

