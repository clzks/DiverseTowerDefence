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

    public TowerStatus(string n, int id, int lev, int type, float a, float s, int c, int t, float upd, int bt, float range, float bddr, float bs, int bn, float minD, float maxD, float upmd, float upxd, 
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
    private static object _Lock = new object();
    public static ConstructManager instance = null;
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

        //SetTowerStatus();

        SetGroundInfo();

    }
#if DEBUG
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
                            ConstructTower(GroundIndex);
                        }
                    }
                }
#if UNITY_EDITOR
                if (Input.GetMouseButtonDown(1))
                {
                    target = GetClickedObject();
                    if (target != null)
                    {
                        if (target.tag == "TowerPlace")
                        {
                            int GroundIndex = System.Convert.ToInt32(target.name);
                            DestructTower(GroundIndex);
                        }
                    }
                }
#endif
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
        if(other.tag == "TowerPlace")
        {
            int GroundIndex = System.Convert.ToInt32(other.name);
            ConstructTower(GroundIndex);
        }
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

            GameObject goTower = Instantiate(towerModelList[0]);
            goTower.name = "tower" + i.ToString();
            goTower.AddComponent<Tower>();
            Tower t = goTower.GetComponent<Tower>();
            t.SetTowerStatus(towerStatusDic[towerTypeList[groundInfoList[i].TowerIndex].TowerName][groundInfoList[i].TowerLevel], groundInfoList[i].TowerIndex, i);

            goTower.transform.position = groundInfoList[i].position;
            
            UserTowerDic.Add(i, goTower);
        }
        else if (groundInfoList[i].TowerIndex != -1)  //  타워가 건설되어 있을 때
        {
            // 해당지역의 타워인덱스와 레벨을 받아온다.
            int TowerIndex = groundInfoList[i].TowerIndex;
            int Level = groundInfoList[i].TowerLevel;
            bool isSearch = false;
            for(int n = 0; n < nGroundNum; ++n)
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
                    isSearch = true;
                    groundInfoList[i].TowerLevel += 1;  // 클릭한 지역의 타워 레벨은 1업~

                    int r = Random.Range(0, 7);
                    groundInfoList[i].TowerIndex = UserDataManager.Instance.TowerDeckList[r];

                    groundInfoList[n].TowerIndex = -1;  // 같은 곳은 타워가 사라진닷
                    groundInfoList[n].TowerLevel = -1;

                    Tower t = UserTowerDic[i].GetComponent<Tower>();
                    t.SetTowerStatus(towerStatusDic[towerTypeList[groundInfoList[i].TowerIndex].TowerName][groundInfoList[i].TowerLevel], groundInfoList[i].TowerIndex, i);
                    Destroy(UserTowerDic[n].gameObject);
                    UserTowerDic.Remove(n);
                    break;
                }
            }
            if (!isSearch)
                print("같은 종류의 타워를 찾을 수 없습니다");
        }
    }
    private void DestructTower(int i)
    {
        groundInfoList[i].TowerIndex = -1;  // 같은 곳은 타워가 사라진닷
        groundInfoList[i].TowerLevel = -1;
        Destroy(UserTowerDic[i].gameObject);
        UserTowerDic.Remove(i);
    }


    private void LoadTowerModel()
    {
        GameObject go = new GameObject();
        go = Resources.Load<GameObject>("Tower");
        towerModelList.Add(go);
    }

    private void LoadTowerInfoLabel()
    {
        TowerLabel = Resources.Load<GameObject>("Label/ConstructInfoLabel");
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
                             reader.GetFloat(10), reader.GetFloat(11), reader.GetFloat(12), reader.GetInt32(13), reader.GetFloat(14), reader.GetFloat(15), reader.GetFloat(16), reader.GetFloat(17), reader.GetFloat(18), reader.GetFloat(19),
                             reader.GetFloat(20), reader.GetFloat(21), reader.GetFloat(22), reader.GetFloat(23), reader.GetFloat(24), reader.GetFloat(25), reader.GetFloat(26), reader.GetFloat(27), reader.GetFloat(28),
                             reader.GetFloat(29), reader.GetFloat(30), reader.GetInt32(31), reader.GetFloat(32), reader.GetInt32(33));

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
#else

#endif
}
