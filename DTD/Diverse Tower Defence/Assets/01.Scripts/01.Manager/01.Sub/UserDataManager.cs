using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class UserDataManager : MonoBehaviour
{
    // 인게임 데이터 아닌거 =======================================
    public int Ruby;
    public List<int> TowerDeckList = new List<int>();                   // 현재 저장된 타워 덱 리스트
    public List<int> PossesTowerList = new List<int>();                 // 유저가 보유한 타워 덱 리스트

    public int PlayNum;                                                 // 플레이 횟수
    public int HighestStage;                                            // 최고 스테이지 기록
    public int ClearCount;                                              // ??
    // ===========================================================


    // 인게임 데이터 ==============================================
    public int CurrStageNum;
    public int CurrLife;
    public int CurrGold;
    public List<GroundInfo> CurrGroundInfo = new List<GroundInfo>();
    public List<int> CurrentDeckList = new List<int>();
    public int MultiBouncingUpgrade;
    public int RangeUpgrade;
    public int RapidUpgrade;
    public int SplashAoEUpgrade;

    // ===========================================================
    private static object _Lock = new object();
    public static UserDataManager instance = null;
    public static UserDataManager Instance
    {
        get
        {
            lock (_Lock)

            {
                if (instance == null)
                {
                    instance = GameObject.Find("Manager").AddComponent<UserDataManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }

    private void Start()
    {
        InitUserData();
    }

    public void InitUserData()
    {
        Ruby = 0;                                                   // 루비 초기화
        TowerDeckList.Clear();                                      // 기본 제공 타워 덱(나중에 수정가능)
        PossesTowerList.Clear();                                    // 보유한 타워 덱;

        InitDeckList();                                             // 덱 리스트 세팅
        InitPossesTowerList();                                      // 보유한 타워 리스트 초기화
        CurrentDeckList = TowerDeckList;                            // 현재 덱 또한 초기화 된 타워덱리스트

        InitVariousFigure();                                        // 그 외의 수치들 초기화

        SaveAllData();
    }

    public void LoadDeckList()
    {
        string LoadString = File.ReadAllText(Application.persistentDataPath + "/SaveData.json");
        JObject LoadData = JObject.Parse(LoadString);

        JArray j1 = (JArray)LoadData[Define.PossesTowerList];
        PossesTowerList = j1.ToObject<List<int>>();

        JArray j2 = (JArray)LoadData[Define.CurrentDeckList];
        CurrentDeckList = j1.ToObject<List<int>>();

        JArray j3 = (JArray)LoadData[Define.TowerDeckList];
        TowerDeckList = j3.ToObject<List<int>>();
    }

    public void LoadAllData()
    {
        string LoadString = File.ReadAllText(Application.persistentDataPath + "/SaveData.json");
        JObject LoadData = JObject.Parse(LoadString);

        Ruby = (int)LoadData[Define.Ruby];
        PlayNum = (int)LoadData[Define.PlayNum];
        HighestStage = (int)LoadData[Define.HighestStage];
        ClearCount = (int)LoadData[Define.ClearCount];
        CurrStageNum = (int)LoadData[Define.CurrStageNum];
        CurrLife = (int)LoadData[Define.CurrLife];
        CurrGold = (int)LoadData[Define.CurrGold];
        MultiBouncingUpgrade = (int)LoadData[Define.MultiBouncingUpgrade];
        RangeUpgrade = (int)LoadData[Define.RangeUpgrade];
        RapidUpgrade = (int)LoadData[Define.RapidUpgrade];
        SplashAoEUpgrade = (int)LoadData[Define.SplashAoEUpgrade];
        
        //PossesTowerList = LoadData[Define.PossesTowerList];
        JArray j1 = (JArray)LoadData[Define.PossesTowerList];
        PossesTowerList = j1.ToObject<List<int>>();

        JArray j2 = (JArray)LoadData[Define.CurrentDeckList];
        CurrentDeckList = j1.ToObject<List<int>>();

        JArray j3 = (JArray)LoadData[Define.TowerDeckList];
        TowerDeckList = j3.ToObject<List<int>>();
    }

    public void SaveAllData()
    {
        JObject jSaveData = new JObject();
        jSaveData[Define.Ruby] = Ruby;
        jSaveData[Define.PlayNum] = PlayNum;
        jSaveData[Define.HighestStage] = HighestStage;
        jSaveData[Define.ClearCount] = ClearCount;
        jSaveData[Define.CurrStageNum] = CurrStageNum;
        jSaveData[Define.CurrLife] = CurrLife;
        jSaveData[Define.CurrGold] = CurrGold;
        jSaveData[Define.MultiBouncingUpgrade] = MultiBouncingUpgrade;
        jSaveData[Define.RangeUpgrade] = RangeUpgrade;
        jSaveData[Define.RapidUpgrade] = RapidUpgrade;
        jSaveData[Define.SplashAoEUpgrade] = SplashAoEUpgrade;

        JArray jTowerDeckListData = new JArray();
        for (int i = 0; i < TowerDeckList.Count; ++i)
        {
            jTowerDeckListData.Add(TowerDeckList[i]);
        }

        JArray jCurrentDeckListData = new JArray();
        for (int i = 0; i < CurrentDeckList.Count; ++i)
        {
            jCurrentDeckListData.Add(CurrentDeckList[i]);
        }

        JArray jPossesDeckListData = new JArray();
        for (int i = 0; i < PossesTowerList.Count; ++i)
        {
            jPossesDeckListData.Add(PossesTowerList[i]);
        }
        jSaveData[Define.TowerDeckList] = jTowerDeckListData;
        jSaveData[Define.CurrentDeckList] = jCurrentDeckListData;
        jSaveData[Define.PossesTowerList] = jPossesDeckListData;
        string SaveString = JsonConvert.SerializeObject(jSaveData, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/SaveData.json", SaveString);
    }


    public void InitDeckList()
    {
        TowerDeckList.Add(0);
        TowerDeckList.Add(5);
        TowerDeckList.Add(10);
        TowerDeckList.Add(14);
        TowerDeckList.Add(17);
        TowerDeckList.Add(20);
        TowerDeckList.Add(23);
        TowerDeckList.Add(25);
    }
    public void InitPossesTowerList()
    {
        for (int i = 0; i < 31; ++i)
        {
            PossesTowerList.Add(i);
        }
    }

    public void InitVariousFigure()
    {
        PlayNum = 0;
        HighestStage = 0;
        ClearCount = 0;
        CurrStageNum = 0;
        CurrLife = 0;
        CurrGold = 0;
        MultiBouncingUpgrade = 0;
        RangeUpgrade = 0;
        RapidUpgrade = 0;
        SplashAoEUpgrade = 0;
    }
}
