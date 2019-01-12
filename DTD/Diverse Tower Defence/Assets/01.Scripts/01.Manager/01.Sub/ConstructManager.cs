using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mono.Data.SqliteClient;
using System.IO;
using System.Data;

public class TowerStatus
{
    public string Name;
    public int ID;
    public int Level;
    public int TowerType;
    public float Attack;
    public float AtkSpd;
    public int SellCost;
    public int TowerIndex;
    public float UpgradePerDamage;
    public int BulletType;                  // 공격타입 (즉발, 투사체, 광선, 근접, 스플)
    public int BulletModelIndex;            // 불릿 모델 인덱스
    public float Range;
    public float BounceDDR;              // 튕길때마다 데미지 감소 비율
    public float BulletSpd;
    public int BounceNum;                // 튕기는 횟수
    public float MinDamage;              // 최소데미지
    public float MaxDamage;              // 최대데미지
    public float UpgradePerMinD;
    public float UpgradePerMaxD;
    public float SplashRange;
    public float CriticalProb;           // 크리확률
    public float CriticalMagni;          // 크리 배율
    public float RayLength;              // 광선길이
    public float RayDirX;
    public float RayDirZ;
    public float DotSustainTime;         // 독뎀 지속시간
    public float DotDamage;
    public float UPDD;                   // 업그레이드 시 독뎀 증가량
    public float MaxDotST;               // 독뎀 최고 중첩 누적시간
    public float AOSustainTime;          // 장판지속시간
    public float AODamage;               // 장판데미지
    public float UPDAO;                  // 장판데미지 증가량
    public int MultiNum;                 // 멀티샷 추가 개수
    public float MultiRate;              // 멀티샷의 데미지 비율
    public int CurrUpgradeNum;           // 현재 업그레이드 숫자

    public TowerStatus(string n, int id, int lev, int type, float a, float s, int c, int t, float upd, int bt, int bmid, float range, float bddr, float bs, int bn, float minD, float maxD, float upmd, float upxd, 
        float sr, float cp, float cm, float rl, float rdx, float rdz, float dst, float dd, float updd, float mdst, float aost, float aod, float updao, int multinum, float multirate ,int cun)
    {
        Name = n;
        ID = id;
        Level = lev;
        TowerType = type;
        Attack = a;
        AtkSpd = s;
        SellCost = c;
        TowerIndex = t;
        UpgradePerDamage = upd;
        BulletType = bt;
        BulletModelIndex = bmid;
        Range = range;
        BounceDDR = bddr;
        BulletSpd = bs;
        BounceNum = bn;
        MinDamage = minD;
        MaxDamage = maxD;
        UpgradePerMinD = upmd;
        UpgradePerMaxD = upxd;
        SplashRange = sr;
        CriticalProb = cp;     // 크리확률
        CriticalMagni = cm;    // 크리 배율
        RayLength = rl;        // 광선길이
        RayDirX = rdx;
        RayDirZ = rdz;
        DotSustainTime = dst;   // 독뎀 지속시간
        DotDamage = dd;
        UPDD = updd;             // 업그레이드 시 독뎀 증가량
        MaxDotST = mdst;         // 독뎀 최고 중첩 누적시간
        AOSustainTime = aost;
        AODamage = aod;
        UPDAO = updao;
        MultiNum = multinum;
        MultiRate = multirate;
        CurrUpgradeNum = cun;
    }
}

public class TowerType
{
    public string TowerName;
    public int TypeIndex;
    public int TowerIndex;
    public TowerType(string n, int typeindex, int towerindex)
    {
        TowerName = n;
        TypeIndex = typeindex;
        TowerIndex = towerindex;
    }
}

public class GroundInfo
{
    public int index;
    public int TowerIndex;
    public int TowerLevel;
    public int TowerIndexOfList;
    public Vector3 position;
}


public class ConstructManager : MonoBehaviour
{
    public Camera MainCamera;
    // ========================= 타워 리스트 ==================================================== //
    public List<GameObject> towerModelList = new List<GameObject>();                    // 모든 타워 모델 리스트
    public Dictionary<string, List<TowerStatus>> towerStatusDic = new Dictionary<string, List<TowerStatus>>();      // 타워별 리스트를 이름을 통한 딕션으로 저장
    public List<TowerType> towerTypeList = new List<TowerType>();                       // 타워 타입 데이터
    private int nTypeAmount = 34;
    private int nModelCount = 32;
    private int nMaxLevelofTower = 4;
    // ============================================================================================== //

    // ========================= 건설관련  =================================================== //
    //public List<int> UserDataManager.Instance.TowerDeckList = new List<int>();
    public List<GroundInfo> groundInfoList = new List<GroundInfo>();
    public int nGroundNum = 48;
    private GameObject target;
    public Dictionary<int, GameObject> UserTowerDic = new Dictionary<int, GameObject>();        // 현재 건설된 타워들 (GroundIndex, Tower)
    public GameObject TowerLabel;                                                                      // 타워에 사용할 라벨
    public List<GameObject> LabelList = new List<GameObject>();
    // =============================================================================================== //

    // ==========  타워 관리 =============================
    public List<Tower> MultiBouncingList = new List<Tower>();
    public List<Tower> RapidList = new List<Tower>();
    public List<Tower> RangeList = new List<Tower>();
    public List<Tower> SplashAoEList = new List<Tower>();
    // ==================================================
    private static object _Lock = new object();
    private static ConstructManager instance = null;
    public static ConstructManager Instance
    {
        get
        {
            lock (_Lock)

            {
                if (instance == null)
                {
                    instance = GameObject.Find("Manager").AddComponent<ConstructManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }

    public enum ETowerType
    {
        Range,
        Splash,
        Bouncing,
        Lucky,
        Critical,
        Rapid,
        Normal,
        Laser,
        Dot,
        AoE,
        Multi,
        Random,
        Metamon
    }
    
    public enum EAttackType
    {
        Normal,
        Projectile,
        Laser,
        Multi,
        Splash,
        Bouncing,
        Instant
    }

    private void Start()
    {
        LoadTowerInfoLabel();

        LoadTowerModel();

        //LoadBulletModel();
        //SetTowerStatus();

        SetGroundInfo();

    }
    private void Update()
    {
        if (StageManager.Instance.isGameStart)
        {
            if (GameManager.Instance.nCurrentScene == 1)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    target = GetClickedObject();
                    if (target != null)
                    {
                        if (target.tag == "TowerPlace")
                        {
                            int GroundIndex = System.Convert.ToInt32(target.name);

                            if(StageManager.Instance.ControlType == StageManager.EControl.EConstruct)
                            {
                                if (StageManager.Instance.ConstructType == StageManager.EConstruct.Construct)
                                {
                                    if (NeedGold(100))
                                    {
                                        ConstructTower(GroundIndex);
                                    }
                                    else
                                    {
                                        print("골드가 부족캅니다");
                                    }
                                }
                                else if(StageManager.Instance.ConstructType == StageManager.EConstruct.Merge)
                                {
                                    MergeTower(GroundIndex);
                                }
                                else if(StageManager.Instance.ConstructType == StageManager.EConstruct.Sell)
                                {
                                    DestructTower(GroundIndex);
                                }
                            }
                            StageManager.Instance.inGameData.InitSetting();
                        }
                    }
                }
            }
        }
    }

    private GameObject GetClickedObject()
    {
        RaycastHit rayHit;
        GameObject target = null;
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
       
        if(Physics.Raycast(ray.origin, ray.direction, out rayHit, float.MaxValue, 1 << 10))
        {
            target = rayHit.collider.gameObject;
        }
        return target;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnDrawGizmos()
    {
        
    }

    public void ConstructTower(int i)
    {
        if (groundInfoList[i].TowerIndex == -1)  // 타워가 건설되어 있지 않을 때
        {
            int r = Random.Range(0, 8);

            groundInfoList[i].TowerIndex = UserDataManager.Instance.TowerDeckList[r];
            groundInfoList[i].TowerLevel = 0;
            groundInfoList[i].TowerIndexOfList = r;
            int ThisTowerIndex = groundInfoList[i].TowerIndex;

            GameObject goTower = Instantiate(towerModelList[ThisTowerIndex]);
            goTower.name = "tower" + i.ToString();
            goTower.AddComponent<Tower>();
            Tower t = goTower.GetComponent<Tower>();
            t.SetTowerStatus(towerStatusDic[towerTypeList[groundInfoList[i].TowerIndex].TowerName][groundInfoList[i].TowerLevel], groundInfoList[i].TowerIndex, i);
            //SetTowerManageMent(t);
            goTower.transform.position = groundInfoList[i].position;
            UserTowerDic.Add(i, goTower);

            UpgradeManager.Instance.Gold -= 100;
            QuestManager.Instance.GetInfomation(QuestManager.EMissionType.Construct);
            QuestManager.Instance.OccurToConstructMoment(0, r);
        }
        else
        {
            print("이미 타워가 건설되어 있습니다");
        }
    }

    public void MergeTower(int i)
    {
        if (groundInfoList[i].TowerIndex != -1)  //  타워가 건설되어 있을 때
        {
            // 해당지역의 타워인덱스와 레벨을 받아온다.
            int TowerIndex = groundInfoList[i].TowerIndex;
            int Level = groundInfoList[i].TowerLevel;
            bool isSearch = false;
            for (int n = 0; n < nGroundNum; ++n)
            {
                bool TIsame = false;
                bool LVsame = false;
                if (n == i) // 같은자리는 검색할 필요 없음
                    continue;

                if (groundInfoList[n].TowerIndex == TowerIndex)     // 
                    TIsame = true;
                if (groundInfoList[n].TowerLevel == Level)          // 
                    LVsame = true;

                if (TIsame && LVsame)
                {
                    int index1 = groundInfoList[i].TowerIndexOfList;
                    int index2 = groundInfoList[n].TowerIndexOfList;

                    isSearch = true;
                    groundInfoList[i].TowerLevel += 1;  // 클릭한 지역의 타워 레벨은 1업~

                    bool destructMostKillTower = CheckMostKilledTower(i);

                    if (!destructMostKillTower)
                    {
                        destructMostKillTower = CheckMostKilledTower(n);
                    }

                    int r = Random.Range(0, 7);
                    groundInfoList[i].TowerIndex = UserDataManager.Instance.TowerDeckList[r];
                    groundInfoList[i].TowerIndexOfList = r;
                    int ThisTowerIndex = groundInfoList[i].TowerIndex;

                    groundInfoList[n].TowerIndex = -1;  // 같은 곳은 타워가 사라진닷
                    groundInfoList[n].TowerLevel = -1;
                    groundInfoList[n].TowerIndexOfList = -1;

                    Destroy(UserTowerDic[i].gameObject);
                    UserTowerDic.Remove(i);
                    GameObject goTower = Instantiate(towerModelList[ThisTowerIndex]);
                    UserTowerDic.Add(i, goTower);
                    goTower.name = "tower" + i.ToString();
                    goTower.AddComponent<Tower>();
                    goTower.transform.position = groundInfoList[i].position;

                    Tower t = UserTowerDic[i].GetComponent<Tower>();
                    t.SetTowerStatus(towerStatusDic[towerTypeList[groundInfoList[i].TowerIndex].TowerName][groundInfoList[i].TowerLevel], groundInfoList[i].TowerIndex, i);
                    t.CurrKillCount = 0;

                    //Tower NewTower = ChangeManageMent(t);
                    //SetTowerManageMent(NewTower);
                    Destroy(UserTowerDic[n].gameObject);
                    UserTowerDic.Remove(n);
                    QuestManager.Instance.OccurToMergeMoment(Level, index1, Level, index2, Level + 1, r);

                    if(destructMostKillTower)
                    {
                        QuestManager.Instance.SetMostKillTowerInit();
                    }


                    QuestManager.Instance.GetInfomation(QuestManager.EMissionType.Construct);
                    QuestManager.Instance.GetInfomation(QuestManager.EMissionType.Merge);
                    break;
                }
            }
            if (!isSearch)
                print("같은 종류의 타워를 찾을 수 없습니다");
        }
        else
        {
            print("타워가 건설되지 않았습니다");
        }
    }
    private void DestructTower(int i)
    {
        int Level = groundInfoList[i].TowerLevel;
        

        switch (Level)
        {
            case 0:
                UpgradeManager.Instance.AddGold(50);
                break;

            case 1:
                UpgradeManager.Instance.AddGold(100);
                break;

            case 2:
                UpgradeManager.Instance.AddGold(200);
                break;

            case 3:
                UpgradeManager.Instance.AddGold(400);
                break;

            case 4:
                UpgradeManager.Instance.AddGold(800);
                break;
        }

        QuestManager.Instance.OccurToDestructMoment(Level, groundInfoList[i].TowerIndexOfList);

        bool destructMostKillTower = CheckMostKilledTower(i);

        groundInfoList[i].TowerIndex = -1;  
        groundInfoList[i].TowerLevel = -1;
        groundInfoList[i].TowerIndexOfList = -1;
        Destroy(UserTowerDic[i].gameObject);
        UserTowerDic.Remove(i);

        if(destructMostKillTower)
        {
            QuestManager.Instance.SetMostKillTowerInit();
        }
        QuestManager.Instance.GetInfomation(QuestManager.EMissionType.Construct);
    }


    private void LoadTowerModel()
    {
        GameObject go = new GameObject();
        //int ModelCount = DeckManager.Instance.PossesTowerDeckList.Count;

        for (int i = 0; i < nModelCount; ++i)
        {
            go = Resources.Load<GameObject>("TowerModel/" + i.ToString());
            towerModelList.Add(go);
        }
        //for (int i = 0; i < 10; ++i)
        //{
        //    go = Resources.Load<GameObject>("Towers/Tower" + i.ToString());
        //    towerModelList.Add(go);
        //}
    }

    private void LoadBulletModel()
    {

    }


    private void LoadTowerInfoLabel()
    {
        TowerLabel = Resources.Load<GameObject>("Label/ConstructInfoLabel");
    }

    private Tower ChangeManageMent(Tower t)
    {
        if (t.TowerType == (int)ConstructManager.ETowerType.AoE || t.TowerType == (int)ConstructManager.ETowerType.Splash || t.TowerType == (int)ConstructManager.ETowerType.Laser)
        {
            SplashAoEList.Remove(t);
        }
        else if (t.TowerType == (int)ConstructManager.ETowerType.Bouncing || t.TowerType == (int)ConstructManager.ETowerType.Multi)
        {
            MultiBouncingList.Remove(t);
        }
        else if (t.TowerType == (int)ConstructManager.ETowerType.Critical || t.TowerType == (int)ConstructManager.ETowerType.Lucky || t.TowerType == (int)ConstructManager.ETowerType.Range)
        {
            RangeList.Remove(t);
        }
        else if (t.TowerType == (int)ConstructManager.ETowerType.Dot || t.TowerType == (int)ConstructManager.ETowerType.Normal || t.TowerType == (int)ConstructManager.ETowerType.Rapid)
        {
            RapidList.Remove(t);
        }
        else
        {

        }
        Tower newT = new Tower();

        return newT;
    }

    private void SetTowerManageMent(Tower t) 
    {
        if (t.TowerType == (int)ConstructManager.ETowerType.AoE || t.TowerType == (int)ConstructManager.ETowerType.Splash || t.TowerType == (int)ConstructManager.ETowerType.Laser)
        {
            SplashAoEList.Add(t);
        }
        else if (t.TowerType == (int)ConstructManager.ETowerType.Bouncing || t.TowerType == (int)ConstructManager.ETowerType.Multi)
        {
            MultiBouncingList.Add(t);
        }
        else if (t.TowerType == (int)ConstructManager.ETowerType.Critical || t.TowerType == (int)ConstructManager.ETowerType.Lucky || t.TowerType == (int)ConstructManager.ETowerType.Range)
        {
            RangeList.Add(t);
        }
        else if (t.TowerType == (int)ConstructManager.ETowerType.Dot || t.TowerType == (int)ConstructManager.ETowerType.Normal || t.TowerType == (int)ConstructManager.ETowerType.Rapid)
        {
            RapidList.Add(t);
        }
        else
        {

        }
    }

    public void CheckManageMentNullAndDelete()
    {
        if(MultiBouncingList.Count != 0)
        {
            for(int i = 0; i<MultiBouncingList.Count; ++i)
            {
                if(MultiBouncingList[i] == null)
                {
                    MultiBouncingList.RemoveAt(i);
                    i--;
                }
            }
        }

        if (RapidList.Count != 0)
        {
            for (int i = 0; i < RapidList.Count; ++i)
            {
                if (RapidList[i] == null)
                {
                    RapidList.RemoveAt(i);
                    i--;
                }
            }
        }

        if (RangeList.Count != 0)
        {
            for (int i = 0; i < RangeList.Count; ++i)
            {
                if (RangeList[i] == null)
                {
                    RangeList.RemoveAt(i);
                    i--;
                }
            }
        }

        if (SplashAoEList.Count != 0)
        {
            for (int i = 0; i < SplashAoEList.Count; ++i)
            {
                if (SplashAoEList[i] == null)
                {
                    SplashAoEList.RemoveAt(i);
                    i--;
                }
            }
        }
    }
    public void CheckUpgradeTower(int i)
    {
        switch(i)
        {
            // 멀티 바운싱
            case 0:
                for(int n = 0; n < MultiBouncingList.Count; ++n)
                {
                    if(MultiBouncingList[n] != null)
                    {
                        MultiBouncingList[n].CurrUpgradeNum = UpgradeManager.Instance.MultiBouncingUpgrade;
                    }
                }
                break;

            // 래피드
            case 1:
                for (int n = 0; n < RapidList.Count; ++n)
                {
                    if (RapidList[n] != null)
                    {
                        RapidList[n].CurrUpgradeNum = UpgradeManager.Instance.RapidUpgrade;
                    }
                }
                break;
            
            // 레인지
            case 2:
                for (int n = 0; n < RangeList.Count; ++n)
                {
                    if (RangeList[n] != null)
                    {
                        RangeList[n].CurrUpgradeNum = UpgradeManager.Instance.RangeUpgrade;
                    }
                }
                break;

            // 스플래쉬 광역
            case 3:
                for (int n = 0; n < SplashAoEList.Count; ++n)
                {
                    if (SplashAoEList[n] != null)
                    {
                        SplashAoEList[n].CurrUpgradeNum = UpgradeManager.Instance.SplashAoEUpgrade;
                    }
                }
                break;
        }
    }
    private bool NeedGold(int n)
    {
        bool b = true;

        if(UpgradeManager.Instance.Gold < n)
        {
            b = false;
        }

        return b;
    }
    private void SetGroundInfo()
    {
        for (int i = 0; i < nGroundNum; ++i)
        {
            GroundInfo GI = new GroundInfo();
            GI.index = i;
            GI.TowerIndex = -1;
            GI.TowerLevel = -1;
            groundInfoList.Add(GI);
        }
    }

    public bool CheckMostKilledTower(int i)
    {
        if (groundInfoList[i].index == QuestManager.Instance.GetMostKillTowerIndex())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 코루틴 . 디비파싱
    public void TowerStatusDbParsing(string p)
    {
        string Filepath = Application.persistentDataPath + "/" + p;

        if (!File.Exists(Filepath))
        {
            Debug.LogWarning("File \"" + Filepath + "\" does not exist. Attempting to create from \"" +
                             Application.dataPath + "!/assets/" + p);

            WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/" + p);
            while (!loadDB.isDone) { }
            File.WriteAllBytes(Filepath, loadDB.bytes);
        }

        string connectionString = "URI=file:" + Filepath;

        // using을 사용함으로써 비정상적인 예외가 발생할 경우에도 반드시 파일을 닫히도록 할 수 있다.
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())  // EnterSqL에 명령 할 수 있다. 
            {

                string sqlQuery = "SELECT * FROM TowerStatus";

                dbCmd.CommandText = sqlQuery;

                using (IDataReader reader = dbCmd.ExecuteReader()) // 테이블에 있는 데이터들이 들어간다. 
                {
                    List<List<TowerStatus>> towerStatusList = new List<List<TowerStatus>>();
                    for(int n = 0; n < nTypeAmount; ++n)
                    {
                        List<TowerStatus> towerStatus = new List<TowerStatus>();
                        towerStatusList.Add(towerStatus);
                    }
                    int Count = 0;
                    int Id = 0;
                    while (reader.Read())
                    {
                        // Debug.Log(reader.GetString(1));  //  타입명 . (몇 열에있는것을 불를것인가)
                        TowerStatus t = new TowerStatus(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3) ,reader.GetFloat(4), reader.GetFloat(5), reader.GetInt32(6), reader.GetInt32(7), reader.GetFloat(8), reader.GetInt32(9),
                             reader.GetInt32(10), reader.GetFloat(11), reader.GetFloat(12), reader.GetFloat(13), reader.GetInt32(14), reader.GetFloat(15), reader.GetFloat(16), reader.GetFloat(17), reader.GetFloat(18), reader.GetFloat(19), reader.GetFloat(20),
                             reader.GetFloat(21), reader.GetFloat(22), reader.GetFloat(23), reader.GetFloat(24), reader.GetFloat(25), reader.GetFloat(26), reader.GetFloat(27), reader.GetFloat(28), reader.GetFloat(29),
                             reader.GetFloat(30), reader.GetFloat(31), reader.GetInt32(32), reader.GetFloat(33), reader.GetInt32(34));

                        if (Count <= nMaxLevelofTower)   
                        {
                            towerStatusList[Id].Add(t);
                            Count++;
                            if (Count > nMaxLevelofTower)
                            {
                                //towerStatusList[Id].Add(t);
                                Count = 0;
                                Id++;
                            }
                        }  
                    }
                    for(int i = 0; i < nTypeAmount; ++i)
                    {
                        towerStatusDic.Add(towerStatusList[i][0].Name, towerStatusList[i]); 
                    }
                    dbConnection.Close();
                    reader.Close();
                }
            }

            //foreach (KeyValuePair<string, TowerStatus> TS in towerStatusList)
            //{
            //    //print(TS.Key);
            //
            //}
        }

    }   // 타워딕셔너리에 dic[타워이름][타워리스트] 로 넣고 리스트에 인덱스 = 레벨에 동일한 타워 스테이터스가 들어감
    public void TowerTypeDbParsing(string p)
    {

        string Filepath = Application.persistentDataPath + "/" + p;

        if (!File.Exists(Filepath))
        {
            Debug.LogWarning("File \"" + Filepath + "\" does not exist. Attempting to create from \"" +
                             Application.dataPath + "!/assets/" + p);

            WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/" + p);
            while (!loadDB.isDone) { }
            File.WriteAllBytes(Filepath, loadDB.bytes);
        }

        string connectionString = "URI=file:" + Filepath;

        // using을 사용함으로써 비정상적인 예외가 발생할 경우에도 반드시 파일을 닫히도록 할 수 있다.
        using (IDbConnection dbConnection = new SqliteConnection(connectionString))
        {
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())  // EnterSqL에 명령 할 수 있다. 
            {

                string sqlQuery = "SELECT * FROM TowerType";


                dbCmd.CommandText = sqlQuery;

                using (IDataReader reader = dbCmd.ExecuteReader()) // 테이블에 있는 데이터들이 들어간다. 
                {
                    while (reader.Read())
                    {
                        // Debug.Log(reader.GetString(1));  //  타입명 . (몇 열에있는것을 불를것인가)
                        TowerType t = new TowerType(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2));
                        towerTypeList.Add(t);
                    }
                    dbConnection.Close();
                    reader.Close();
                }
            }
        }
    }
}
