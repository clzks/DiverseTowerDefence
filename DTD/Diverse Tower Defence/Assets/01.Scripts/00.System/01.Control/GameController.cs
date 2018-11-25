using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    public UIPanel ConstructDetail;
    public UIPanel UpgradeDetail;
    public UIPanel QuestDetail;
    // 큰 업그레이드 틀 관리 ===========================================================

    public void Start()
    {
        UpgradeDetail.gameObject.SetActive(false);
        QuestDetail.gameObject.SetActive(false);
        ConstructDetail.gameObject.SetActive(false);
    }
    public void SetConstructPanel()
    {
        if(StageManager.Instance.ControlType == StageManager.EControl.EConstruct)
        {
            StageManager.Instance.ControlType = StageManager.EControl.ENone;
            ConstructDetail.gameObject.SetActive(false);
        }
        else
        {
            StageManager.Instance.ControlType = StageManager.EControl.EConstruct;
            ConstructDetail.gameObject.SetActive(true);
            UpgradeDetail.gameObject.SetActive(false);
            QuestDetail.gameObject.SetActive(false);
        }
    }

    public void SetUpgradePanel()
    {
        if (StageManager.Instance.ControlType == StageManager.EControl.EUpgrade)
        {
            StageManager.Instance.ControlType = StageManager.EControl.ENone;
            UpgradeDetail.gameObject.SetActive(false);
        }
        else
        {
            StageManager.Instance.ControlType = StageManager.EControl.EUpgrade;
            ConstructDetail.gameObject.SetActive(false);
            UpgradeDetail.gameObject.SetActive(true);
            QuestDetail.gameObject.SetActive(false);
        }
    }

    public void SetQuestPanel()
    {
        if (StageManager.Instance.ControlType == StageManager.EControl.EQuest)
        {
            StageManager.Instance.ControlType = StageManager.EControl.ENone;
            QuestDetail.gameObject.SetActive(false);
        }
        else
        {
            StageManager.Instance.ControlType = StageManager.EControl.EQuest;
            ConstructDetail.gameObject.SetActive(false);
            UpgradeDetail.gameObject.SetActive(false);
            QuestDetail.gameObject.SetActive(true);
        }
    }

    // ===============================================================================================

    // 건설 컨트롤러 ==================================================================================
    public void SetConstructMode()
    {
        if (StageManager.Instance.ConstructType == StageManager.EConstruct.Construct)
        {
            StageManager.Instance.ConstructType = StageManager.EConstruct.None;
        }
        else
        {
            StageManager.Instance.ConstructType = StageManager.EConstruct.Construct;
        }
    }

    public void SetMergeMode()
    {
        if (StageManager.Instance.ConstructType == StageManager.EConstruct.Merge)
        {
            StageManager.Instance.ConstructType = StageManager.EConstruct.None;
        }
        else
        {
            StageManager.Instance.ConstructType = StageManager.EConstruct.Merge;
        }
    }

    public void SetSellMode()
    {
        if (StageManager.Instance.ConstructType == StageManager.EConstruct.Sell)
        {
            StageManager.Instance.ConstructType = StageManager.EConstruct.None;
        }
        else
        {
            StageManager.Instance.ConstructType = StageManager.EConstruct.Sell;
        }
    }
    // ===============================================================================================

    // =========== 타워 업그레이드 ==========================================
    public void Upgrade()
    {
        string name = UIButton.current.transform.parent.name;
        //int i = 0;
        
        if(name == Define.MultiBouncingUpgrade)
        {
            int g = CalculationGold(UpgradeManager.Instance.MultiBouncingUpgrade);
            if (NeedGold(g))
            {
                UpgradeManager.Instance.Gold -= g;
                UpgradeManager.Instance.Upgrade(ref UpgradeManager.Instance.MultiBouncingUpgrade);
            }
            else
            {
                print("골드가 부조캅니다");
            }
        }
        else if(name == Define.RangeUpgrade)
        {
        //    i = 2;
            int g = CalculationGold(UpgradeManager.Instance.RangeUpgrade);
            if (NeedGold(g))
            {
                UpgradeManager.Instance.Gold -= g;
                UpgradeManager.Instance.Upgrade(ref UpgradeManager.Instance.RangeUpgrade);
            }
            else
            {
                print("골드가 부조캅니다");
            }
        }
        else if (name == Define.RapidUpgrade)
        {
        //    i = 1;
            int g = CalculationGold(UpgradeManager.Instance.RapidUpgrade);
            if (NeedGold(g))
            {
                UpgradeManager.Instance.Gold -= g;
                UpgradeManager.Instance.Upgrade(ref UpgradeManager.Instance.RapidUpgrade);
            }
            else
            {
                print("골드가 부조캅니다");
            }
        }
        else
        {
            //    i = 3;
            int g = CalculationGold(UpgradeManager.Instance.SplashAoEUpgrade);
            if (NeedGold(g))
            {
                UpgradeManager.Instance.Gold -= g;
                UpgradeManager.Instance.Upgrade(ref UpgradeManager.Instance.SplashAoEUpgrade);
            }
            else
            {
                print("골드가 부조캅니다");
            }
        }

        //ConstructManager.Instance.CheckUpgradeTower(i);
    }
    // ======================================================================

    // ================== 각종 함수 ==========================================
    private bool NeedGold(int n)
    {
        bool b = true;

        if (UpgradeManager.Instance.Gold < n)
        {
            b = false;
        }

        return b;
    }

    private int CalculationGold(int n)
    {
        int g = 10;
        g += 2 * n;
        return g;
    }
}
