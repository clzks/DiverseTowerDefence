using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public List<Quest> GlobalQuestList = new List<Quest>();             // 게임 한판마다 적용될 퀘스트 리스트
    public List<Quest> NormalQuestList = new List<Quest>();             // 주기적으로 갱신될 퀘스트 리스트
    public List<Quest> OutBreakQuestList = new List<Quest>();           // 난입퀘스트 리스트
    public List<Quest> DailyQuestList = new List<Quest>();              // 매일접속 등등
    public List<Quest> AchieveQuestList = new List<Quest>();            // 이미 달성한 퀘스트..

    public List<Quest> CurrNormalQuestList = new List<Quest>();         // 현재 진행중인 노말 퀘

    public List<Reward> RewardList = new List<Reward>();                // 보상리스트

    [SerializeField] private QuestData qData = null;         // 퀘스트 전용 데이터

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
        MultiBouncingCount = GameObject.Find("UI Root/Camera/TempQuestData/MultiBouncingCount").GetComponent<UILabel>();
        RapidCount = GameObject.Find("UI Root/Camera/TempQuestData/RapidCount").GetComponent<UILabel>();
        RangeCount = GameObject.Find("UI Root/Camera/TempQuestData/RangeCount").GetComponent<UILabel>();
        SplashCount = GameObject.Find("UI Root/Camera/TempQuestData/SplashCount").GetComponent<UILabel>();
    }

    public enum EGlobalQuestInfoType
    {
        Kill,
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

   
    public void GetInfomation(EGlobalQuestInfoType e)
    {
        switch(e)
        {
            case EGlobalQuestInfoType.Kill:
                
                break;

            case EGlobalQuestInfoType.Construct:

                break;

            case EGlobalQuestInfoType.Merge:
                break;

            case EGlobalQuestInfoType.Stage:

                break;

            case EGlobalQuestInfoType.Life:

                break;

            case EGlobalQuestInfoType.Boss:

                break;
        }
    }

    public void CheckCondition()
    {

    }

    public void InitQuestData()
    {
        //qData = new QuestData();
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
                qData.MostKillCountTower = MostKillCount;
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
        CurrHp.text = qData.CurrHp.ToString();
    }


    public void OccurToKillMoment()
    {
        RenewMostKillCount();
        AddKillCount();
        GetInfomation(EGlobalQuestInfoType.Kill);

        MostKillcount.text = qData.MostKillCountTower.ToString();
        AllKillCount.text = qData.fCurrAllKillCount.ToString();
        CurrHp.text = qData.CurrHp.ToString();
    }

    public void LoadQuestData()
    {

    }
}
 