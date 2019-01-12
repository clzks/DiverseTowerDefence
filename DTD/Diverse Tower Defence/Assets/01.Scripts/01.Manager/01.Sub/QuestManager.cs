using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class QuestManager : MonoBehaviour
{
    public List<Quest> GlobalQuestList = new List<Quest>();             // 게임 한판마다 적용될 퀘스트 리스트
    public List<Quest> CurrGlobalQuestList = new List<Quest>();         // 현재 해당되는 글로발리스트

    public List<Quest> NormalQuestList = new List<Quest>();             // 주기적으로 갱신될 퀘스트 리스트
    public List<Quest> OutBreakQuestList = new List<Quest>();           // 난입퀘스트 리스트
    public List<Quest> DailyQuestList = new List<Quest>();              // 매일접속 등등
    public List<int> AchieveQuestList = new List<int>();                // 이미 달성한 퀘스트..

    public List<Quest> CurrNormalQuestList = new List<Quest>();             // 현재 진행중인 노말 퀘
    public List<GameObject> CurrQuestPanelList = new List<GameObject>();    // 라벨과 스프라이트를 가지는 퀘스트 전용 게임 오브젝트 리스트
    public GameObject QuestPanel;                                           // 퀘스트용 패널
    public UIScrollView ScrollView;                                         // 퀘스트용 스크롤뷰
    public UIGrid grid;
    public List<Reward> RewardList = new List<Reward>();                // 보상리스트

    private QuestData qData = null;         // 퀘스트 전용 데이터

    private UILabel MostKillcount;
    private UILabel AllKillCount;
    private UILabel CurrHp;

    private UILabel MultiBouncingCount; 
    private UILabel RapidCount;
    private UILabel RangeCount;
    private UILabel SplashCount;

    private List<int[]> IndexOfTowerNumList = new List<int[]>();

    private void Start()
    {
        if (qData == null)
        {
            qData = new QuestData();
        }
        //for(int i = 0; i <5; ++i)
        //{
        //    int[] temp = { 0, 0, 0, 0, 0, 0, 0, 0 };
        //    IndexOfTowerNumList.Add(temp);
        //}
    }

    public void SetLabel()
    {
        MostKillcount = GameObject.Find("UI Root/Camera/TempQuestData/MostKillCount").GetComponent<UILabel>();
        AllKillCount = GameObject.Find("UI Root/Camera/TempQuestData/CurrAllKillCount").GetComponent<UILabel>();
        CurrHp = GameObject.Find("UI Root/Camera/TempQuestData/CurrHp").GetComponent<UILabel>();
        //MultiBouncingCount = GameObject.Find("UI Root/Camera/TempQuestData/MultiBouncingCount").GetComponent<UILabel>();
        //RapidCount = GameObject.Find("UI Root/Camera/TempQuestData/RapidCount").GetComponent<UILabel>();
        //RangeCount = GameObject.Find("UI Root/Camera/TempQuestData/RangeCount").GetComponent<UILabel>();
        //SplashCount = GameObject.Find("UI Root/Camera/TempQuestData/SplashCount").GetComponent<UILabel>();
    }

    public enum EMissionType
    {
        TotalKill,
        MostKill,
        Construct,
        Merge,
        Life,
        Boss,
        Stage,
        Etc
    }

    public enum ECheckType
    {

    }

    private static object _Lock = new object();
    public static QuestManager instance = null;
    public static QuestManager Instance
    {
        get
        {
            lock (_Lock)

            {
                if (instance == null)
                {
                    instance = GameObject.Find("Manager").AddComponent<QuestManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }

   
    public void GetInfomation(EMissionType e)
    {
        switch(e)
        {
            case EMissionType.TotalKill:
                //for(int i = 0; i < CurrGlobalQuestList.Count; ++i)
                //{
                //    if((int)CurrGlobalQuestList[i].eDetailQuestType == 0)
                //    {
                //        RenewQuestUI(i);
                //    }
                //}
                CheckRenewUIList(0);
                CheckCompleteQuest(EMissionType.TotalKill);
                break;

            case EMissionType.MostKill:
                //for (int i = 0; i < CurrGlobalQuestList.Count; ++i)
                //{
                //    if ((int)CurrGlobalQuestList[i].eDetailQuestType == 1)
                //    {
                //        RenewQuestUI(i);
                //    }
                //}
                CheckRenewUIList(1);
                CheckCompleteQuest(EMissionType.MostKill);
                break;

            case EMissionType.Construct:
                //for (int i = 0; i < CurrGlobalQuestList.Count; ++i)
                //{
                //    if ((int)CurrGlobalQuestList[i].eDetailQuestType == 2 || (int)CurrGlobalQuestList[i].eDetailQuestType == 1)
                //    {
                //        RenewQuestUI(i);
                //    }
                //}
                CheckRenewUIList(1, 2);
                CheckCompleteQuest(EMissionType.Construct);
                break;

            case EMissionType.Merge:

                break;

            case EMissionType.Stage:

                break;

            case EMissionType.Life:

                break;

            case EMissionType.Boss:

                break;
        }
    }

    public void CheckCondition()
    {

    }

    public void InitQuestData()
    {
        string LoadString = File.ReadAllText(Application.persistentDataPath + "/DTDQuestInfo.json");
        JObject LoadData = JObject.Parse(LoadString);
        int QuestCount = (int)LoadData["questcount"];

        for(int i = 0; i < QuestCount; ++i)
        {
            Quest q = ParseJsonQuest(LoadData, i, "global");

            GlobalQuestList.Add(q);
        }

        InitCurrGlobalQuest(LoadData);

    }
    
    public void InitCurrGlobalQuest(JObject LoadData)
    {
        Quest q = ParseJsonQuest(LoadData, 0, "global");
        CurrGlobalQuestList.Add(q);

        q = ParseJsonQuest(LoadData, 1, "global");
        CurrGlobalQuestList.Add(q);

        q = ParseJsonQuest(LoadData, 2, "global");
        CurrGlobalQuestList.Add(q);

        q = ParseJsonQuest(LoadData, 3, "global");
        CurrGlobalQuestList.Add(q);

        q = ParseJsonQuest(LoadData, 4, "global");
        CurrGlobalQuestList.Add(q);

        q = ParseJsonQuest(LoadData, 5, "global");
        CurrGlobalQuestList.Add(q);

        q = ParseJsonQuest(LoadData, 10, "global");
        CurrGlobalQuestList.Add(q);
    }


    public Quest ParseJsonQuest(JObject LoadData , int i, string key)
    {
        Quest quest = new Quest((int)LoadData[key][i]["id"], (int)LoadData[key][i]["trailingquestindex"], (int)LoadData[key][i]["type"],
                (int)LoadData[key][i]["mission"], (string)LoadData[key][i]["text"],
                 (int)LoadData[key][i]["goalcount"], (int)LoadData[key][i]["constructlevel"],
                (int)LoadData[key][i]["rewardtype"], (int)LoadData[key][i]["rewardvalue"],
                (float)LoadData[key][i]["cooltime"]);

        return quest;
    }

    public void AddKillCount()
    {
        qData.fCurrAllKillCount++;
    }

    public void DecreaseHp()
    {
        qData.DecreaseHp();
    }

    public void RenewMostKillCount()
    {
        int MostKillCount = qData.MostKillCountTower;
        foreach (KeyValuePair<int, GameObject> items in ConstructManager.Instance.UserTowerDic)
        {
            Tower t = items.Value.GetComponentInChildren<Tower>();
            if(MostKillCount < t.CurrKillCount)
            {
                MostKillCount = t.CurrKillCount;
                qData.MostKillCountTowerIndex = t.ID;
                qData.MostKillCountTower = MostKillCount;
                Debug.Log("최고 킬수 타워 인덱스 = " + qData.MostKillCountTowerIndex);
            }
        }
    }

    public void OccurToConstructMoment(int level, int index)
    {
        qData.ToTLevelNumList[level].TypeOfTowerLevel[index]++;


        for (int i = 0; i <5; ++i)
        {
            print("레벨 " + i + "타워 갯수 : " + qData.ToTLevelNumList[i].TypeOfTowerLevel[0] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[1] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[2] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[3] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[4] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[5] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[6] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[7]);
        }
    }

    public void OccurToDestructMoment(int level, int index)
    {
        if (qData.ToTLevelNumList[level].TypeOfTowerLevel[index] > 0)
        {
            qData.ToTLevelNumList[level].TypeOfTowerLevel[index]--;
        }

        for (int i = 0; i < 5; ++i)
        {
            print("레벨 " + i + "타워 갯수 : " + qData.ToTLevelNumList[i].TypeOfTowerLevel[0] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[1] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[2] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[3] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[4] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[5] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[6] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[7]);
        }
    }

    public void OccurToMergeMoment(int level, int index, int level2, int index2, int level3, int index3)
    {
        if (qData.ToTLevelNumList[level].TypeOfTowerLevel[index] > 0)
        {
            qData.ToTLevelNumList[level].TypeOfTowerLevel[index]--;
        }

        if (qData.ToTLevelNumList[level2].TypeOfTowerLevel[index2] > 0)
        {
            qData.ToTLevelNumList[level2].TypeOfTowerLevel[index2]--;
        }

        qData.ToTLevelNumList[level3].TypeOfTowerLevel[index3]++;

        for (int i = 0; i < 5; ++i)
        {
            print("레벨 " + i + "타워 갯수 : " + qData.ToTLevelNumList[i].TypeOfTowerLevel[0] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[1] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[2] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[3] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[4] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[5] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[6] + " ," + qData.ToTLevelNumList[i].TypeOfTowerLevel[7]);
        }
    }
    public void OccurToMonsterEndLine()
    {
        DecreaseHp();
        //switch (SystemInfo.batteryStatus)
        //{
        //    case BatteryStatus.Charging:
        //        break;
        //
        //    case BatteryStatus.Discharging:
        //        break;
        //
        //    case BatteryStatus.Full:
        //        break;
        //
        //    case BatteryStatus.NotCharging:
        //        break;
        //}
        CurrHp.text =  qData.CurrHp.ToString();
    }


    public void OccurToKillMoment()
    {
        RenewMostKillCount();
        AddKillCount();
        GetInfomation(EMissionType.TotalKill);
        GetInfomation(EMissionType.MostKill);

        MostKillcount.text = qData.MostKillCountTower.ToString();
        AllKillCount.text = qData.fCurrAllKillCount.ToString();
        //CurrHp.text = SystemInfo.batteryLevel.ToString();
        CurrHp.text = qData.CurrHp.ToString();
    }

    public void SetQuestUI()
    {
        QuestPanel = Resources.Load<GameObject>("QuestPanel/GlobalQuest");
        ScrollView = GameObject.Find("UI Root/Camera/LeftUI/ControlDetail/QuestDetail/GlobalQuestTab/Scroll View").GetComponent<UIScrollView>();
        grid = GameObject.Find("UI Root/Camera/LeftUI/ControlDetail/QuestDetail/GlobalQuestTab/Scroll View/Grid").GetComponent<UIGrid>();
        GameObject g;
        int index = 0;
        for (int i = CurrGlobalQuestList.Count; i > 0; --i)
        {
            g = Instantiate(QuestPanel, grid.transform, false);
            g.name = "Quest" + index.ToString();
            UILabel label = g.GetComponentInChildren<UILabel>();
            string progress = "진행 상황 (" + CurrGlobalQuestList[index].GetProgressCount() + " / " + CurrGlobalQuestList[index].nGoalCount + ")";
            label.text = CurrGlobalQuestList[index].Text + progress;
            index++;
            CurrQuestPanelList.Add(g);
        }
        grid.Reposition();
        ScrollView.ResetPosition();
    }

    public void ResetQuestUI()
    {
        GameObject g;
        int index = 0;
        for (int i = CurrGlobalQuestList.Count; i > 0; --i)
        {
            g = Instantiate(QuestPanel, grid.transform, false);
            UILabel label = g.GetComponentInChildren<UILabel>();
            string progress = "진행 상황 (" + CurrGlobalQuestList[index].GetProgressCount() + " / " + CurrGlobalQuestList[index].nGoalCount + ")";
            label.text = CurrGlobalQuestList[index].Text + progress;
            index++;
            CurrQuestPanelList.Add(g);
        }
        ScrollView.ResetPosition();
    }

    public void RenewQuestUI(int index)
    {
        UILabel label = CurrQuestPanelList[index].GetComponentInChildren<UILabel>();
        string progress = "진행 상황 (" + CurrGlobalQuestList[index].GetProgressCount() + " / " + CurrGlobalQuestList[index].nGoalCount + ")";
        label.text = CurrGlobalQuestList[index].Text + progress;
    }

    public void CheckRenewUIList(int DetailQuestType)
    {
        for (int i = 0; i < CurrGlobalQuestList.Count; ++i)
        {
            if ((int)CurrGlobalQuestList[i].eDetailQuestType == DetailQuestType)
            {
                RenewQuestUI(i);
            }
        }
    }
    public void CheckRenewUIList(int DetailQuestType1, int DetailQuestType2)
    {
        for (int i = 0; i < CurrGlobalQuestList.Count; ++i)
        {
            if ((int)CurrGlobalQuestList[i].eDetailQuestType == DetailQuestType1 || (int)CurrGlobalQuestList[i].eDetailQuestType == DetailQuestType2)
            {
                RenewQuestUI(i);
            }
        }
    }

    public void CheckCompleteQuest(EMissionType DetailQuestType)
    {
        for (int i = 0; i < CurrGlobalQuestList.Count; ++i)
        {
            if (CurrGlobalQuestList[i].eDetailQuestType == DetailQuestType)
            {
                if(CurrGlobalQuestList[i].GetProgressCount() >= CurrGlobalQuestList[i].nGoalCount)
                {
                    RewardQuest(CurrGlobalQuestList[i].eRewardtype, CurrGlobalQuestList[i].nRewardValue);
                    RemoveAtQuestList(i);
                }
            }
        }
    }

    public void RewardQuest(Reward.ERewardType rewardType, int value)
    {
        switch(rewardType)
        {
            case Reward.ERewardType.Gold:
                UpgradeManager.Instance.AddGold(value);
                break;

            case Reward.ERewardType.Gem:
                UserDataManager.Instance.AddGems(value);
                break;

            case Reward.ERewardType.Life:

                break;

            case Reward.ERewardType.Ap:

                break;

            case Reward.ERewardType.Etc:

                break;
        }
    }

    public void ActQuestPool(int index)
    {
        var del = CurrQuestPanelList[index];
        CurrGlobalQuestList.RemoveAt(index);
        CurrQuestPanelList.RemoveAt(index);
        del.SetActive(false);

        grid.enabled = true;
        grid.Reposition();
        ScrollView.ResetPosition();
    }


    public void RemoveAtQuestList(int index)
    {
        var del = CurrQuestPanelList[index];
        CurrGlobalQuestList.RemoveAt(index);
        CurrQuestPanelList.RemoveAt(index);
        //del.SetActive(false);
        Destroy(del);

        grid.enabled = true;
        grid.Reposition();
        ScrollView.ResetPosition();
    }


    public int GetMostKillCountTower()
    {
        return qData.MostKillCountTower;
    }

    public int GetCurrAllKillCount()
    {
        return qData.fCurrAllKillCount;
    }
    public int GetCurrHp()
    {
        return qData.CurrHp;
    }
    public int GetMergeCount()
    {
        return qData.MergeCount;
    }
    public int GetCompleteQuestCount()
    {
        return qData.CompleteQuestCount;
    }

    public int GetConstructArray(int level)
    {
        return qData.CheckNumberOfTower(level);
    }

    public int GetMostKillTowerIndex()
    {
        return qData.MostKillCountTowerIndex;
    }

    public void SetMostKillTowerInit()
    {
        qData.MostKillCountTower = 0;
        qData.MostKillCountTowerIndex = -1;
        RenewMostKillCount();
    }
}
 