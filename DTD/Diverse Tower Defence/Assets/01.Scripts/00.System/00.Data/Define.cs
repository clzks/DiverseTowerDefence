using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public static readonly string Ruby = "Ruby";
    public static readonly string PlayNum = "PlayNum";
    public static readonly string HighestStage = "HighestStage";
    public static readonly string ClearCount = "ClearCount";
    public static readonly string TowerDeckList = "TowerDeckList";
    public static readonly string PossesTowerList = "PossesTowerList";

    public static readonly string CurrStageNum = "CurrStageNum";
    public static readonly string CurrLife = "CurrLife";
    public static readonly string CurrGold = "CurrGold";
    public static readonly string CurrGroundInfo = "CurrGroundInfo";
    public static readonly string CurrentDeckList = "CurrentDeckList";
    public static readonly string MultiBouncingUpgrade = "MultiBouncingUpgrade";
    public static readonly string RangeUpgrade = "RangeUpgrade";
    public static readonly string RapidUpgrade = "RapidUpgrade";
    public static readonly string SplashAoEUpgrade = "SplashAoEUpgrade";

    public static Quaternion GetRotFromVectors(Vector3 vDir)
    {
        return Quaternion.Euler(0, Mathf.Atan2(vDir.x , vDir.z) * Mathf.Rad2Deg, 0);
    }
}
