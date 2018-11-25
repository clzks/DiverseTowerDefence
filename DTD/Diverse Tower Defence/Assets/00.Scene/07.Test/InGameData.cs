using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameData : MonoBehaviour
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
