using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land
{
    GameHub.LandMark myLandMark;
    bool IsOwned = false;
    bool isDefended = false;
    float productionIncrease = 1.0f;
    private int baseProductionvalue = 100;

    public void SetProductionIncrease(float aProductionIncrease)
    {
        productionIncrease += aProductionIncrease;
    }

    public void SetLandMark(GameHub.LandMark aLandMark)
    {
        myLandMark = aLandMark;
        switch (myLandMark)
        {
            case GameHub.LandMark.WindMill:
                {
                    productionIncrease += 0.2f;
                    break;
                }
            case GameHub.LandMark.Road:
                {
                    productionIncrease += 0.07f;
                    break;
                }
            case GameHub.LandMark.Castle:
                {
                    isDefended = true;
                    break;
                }
        }
    }

    public int GetLandValue()
    {
        float returnValue = baseProductionvalue * productionIncrease;
        return (int)returnValue;
    }
}
