using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UpgradeManager : MonoBehaviour
{
    public int MultiBouncingUpgrade = 0;
    public int RangeUpgrade = 0;
    public int RapidUpgrade = 0;
    public int SplashAoEUpgrade = 0;
    public int Gold = 500;

    private static object _Lock = new object();
    public static UpgradeManager instance = null;
    public static UpgradeManager Instance
    {
        get
        {
            lock (_Lock)

            {
                if (instance == null)
                {
                    instance = GameObject.Find("Manager").AddComponent<UpgradeManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }
    

    public void Upgrade(ref int i)
    {
        if(i < 100)
        {
            i++;
            StageManager.Instance.inGameData.InitSetting();
            print("업그레이드 완료");
        }
    }
}
