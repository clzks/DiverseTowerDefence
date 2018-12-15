using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameData : MonoBehaviour  // 인게임 상에 출력될 데이터들 모아놓자.. 
{
    public UILabel MBLabel;
    public UILabel RLabel;
    public UILabel RPLabel;
    public UILabel SALabel;
    public UILabel Gold;

    public void InitSetting()
    {
        MBLabel.text = UpgradeManager.Instance.MultiBouncingUpgrade.ToString();
        RLabel.text = UpgradeManager.Instance.RangeUpgrade.ToString();
        RPLabel.text = UpgradeManager.Instance.RapidUpgrade.ToString();
        SALabel.text = UpgradeManager.Instance.SplashAoEUpgrade.ToString();
        Gold.text = UpgradeManager.Instance.Gold.ToString();
    }
}
