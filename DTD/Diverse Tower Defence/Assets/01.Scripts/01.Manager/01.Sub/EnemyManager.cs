using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mono.Data.SqliteClient;
using System.IO;
using System.Data;

public class EnemyStatus
{
    public int nId;
    public int nRound;
    public float fHP;
    public float fSpeed;
    public int nCurrNode;
    public int nRemainDotTime;
    public float fCurrDotDamage;

    public EnemyStatus(int id, int round, float hp, float speed, int node, int dotTime, float dotDamage)
    {
        nId = id;
        nRound = round;
        fHP = hp;
        fSpeed = speed;
        nCurrNode = node;
        nRemainDotTime = dotTime;
        fCurrDotDamage = dotDamage;
    }
}

public class EnemyManager : MonoBehaviour
{
    // ========================= 불러올 정보들 ===============================// 
    public List<GameObject> EnemyModelList = new List<GameObject>();
    public List<GameObject> BossModelList = new List<GameObject>();
    public List<EnemyStatus> EnemyStatusList = new List<EnemyStatus>();
    
    public Resources rModel;                        // 프리팹 불러올때 쓸거임 ㅋ
    // =======================================================================




    // ====================== 몬스터 젠 속도 =============================
    private float fTimer = 0.5f;  // 젠속도
    private float fCurrTimer = 0; // 사이클 돌릴 시간
    // ==================================================================


    // ========================= 적의 이동경로 ==========================
    public List<Transform> trNodeList = new List<Transform>();
    public int nNodeNum = 2; // 최대 노드 수 
    // =================================================================

  
    // =========================== 몬스터 풀 ===========================
    public List<GameObject> EnemyPool = new List<GameObject>();
    public List<GameObject> EnemyModelPool = new List<GameObject>();
    public GameObject Boss;                      // 보스전용 오브젝트도 하나 만드네 ㅋ 
    public int nPoolNum = 40;                    // 최대 풀링 갯수
    public int nMadeMonsterNumber = 0;           // 생성된 몬스터 숫자
    public int EliminatedNum = 0;                // 생성숫자랑 죽은숫자가 40일때 라운드가 끝난다고 해놓은것인데 이것도 맘에들진 않네요.. 
    public Transform tStartPosition;             // 적군 시작 위치.
    public int nPoolIndex = 0;                   // 적군의 인덱스 번호
    //public Enemy enem;                          
 
    // ================================================================

    
    // ============================ 보스생성시 필요한것들 (맘에 안듬) ===============
    public bool isBossGen = false;
    public bool isBossDead = true;
    // ============================================================================


    // ======================= 몬스터 피격시 필요한 것들 ============================
    //public List<GameObject> DamageLabelPool = new List<GameObject>();
    public GameObject ProgressBar;
    public Transform BarParent;
    // ============================================================================

    private static object _Lock = new object();
    public static EnemyManager instance = null;
    public static EnemyManager Instance
    {
        get
        {
            lock (_Lock)

            {
                if (instance == null)
                {
                    instance = GameObject.Find("Manager").AddComponent<EnemyManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
       
    }





    public void MakeEnemyPool()
    {
        tStartPosition = GameObject.Find("MonsterPool").transform;
        for (int i = 0; i < nPoolNum; ++i)
        {
            GameObject gSrcEnemy = new GameObject();       // 사용할 적군 오브젝트
            gSrcEnemy.name = "Enemy" + i.ToString();
            gSrcEnemy.SetActive(false);
            gSrcEnemy.transform.SetParent(tStartPosition);
            EnemyPool.Add(gSrcEnemy);
        }
    }

    //public void MakeEnemyDamagePool()
    //{
    //    tStartPosition = GameObject.Find("DamageLabelPool").transform;
    //    GameObject gSrcLabel = new GameObject();       // 사용할 적군 오브젝트
    //    gSrcLabel.name = "DamageLabel";
    //    gSrcLabel.SetActive(false);
    //    gSrcLabel.transform.SetParent(tStartPosition);
    //    EnemyPool.Add(gSrcLabel);
    //}
    
    public void RunEnemy()
    {
        if (!isBossGen)
        {
            for (int i = 0; i < nPoolNum; ++i)
            {
                if (EnemyPool[i].activeSelf)
                {
                    Enemy enem = EnemyModelPool[i].GetComponent<Enemy>();
                    EnemyPool[i].transform.position += new Vector3(-3.0f * GameManager.Instance.nGameSpeed , 0, 0) * Time.deltaTime;
                    if ((EnemyPool[i].transform.position - trNodeList[1].position).magnitude <= 0.1f && EnemyPool[i].activeSelf)   // 몹이 지나가면
                    {
                        EnemyPool[i].SetActive(false);
                        
                        Destroy(EnemyModelPool[i].gameObject);
                        StageManager.Instance.nLife -= 1;
                        Destroy(enem.ProgressBar.gameObject);
                        //enem.StepOnGoalLine();
                    }
                }
            }
        }
        else
        {
            Boss.transform.position += new Vector3(-3.0f * GameManager.Instance.nGameSpeed , 0, 0) * Time.deltaTime;
            if((Boss.transform.position - trNodeList[1].position).magnitude <= 0.1f && Boss.activeSelf)    // 보스 죽으면
            {
                Boss.SetActive(false);
                StageManager.Instance.nLife -= 10;
            }
        }
    }

    private void OnEnable()
    {

    }

    public void ResponeEnemy(int round, float timer, Vector3 v)
    {
        fCurrTimer += Time.deltaTime * GameManager.Instance.nGameSpeed;
        if(fCurrTimer >= fTimer && nPoolIndex < nPoolNum)
        {
            fCurrTimer = 0.0f;
            EnemyPool[nPoolIndex].SetActive(true);

                
            GameObject go = (GameObject)Instantiate(EnemyModelList[StageManager.Instance.nStage], EnemyPool[nPoolIndex].transform);
            go.AddComponent<Enemy>();
            Enemy enem = go.GetComponent<Enemy>();
            enem.SetEnemy(EnemyStatusList[StageManager.Instance.nStage], nPoolIndex);


            EnemyModelPool.Add(go);

            EnemyPool[nPoolIndex].transform.position = v;
            nMadeMonsterNumber++;
            nPoolIndex++;
        }
    }

    public void ResponeBoss(int round, Vector3 v)
    {
        if(!isBossGen && isBossDead)
        {
            Boss = Instantiate(new GameObject());
            GameObject bs = Instantiate(BossModelList[round], Boss.transform);
            bs.AddComponent<Enemy>();
            Enemy BossStatus = bs.GetComponent<Enemy>();
            BossStatus.SetEnemy(EnemyStatusList[StageManager.Instance.nStage], 100);

            Boss.transform.position = v;
            isBossGen = true;
            isBossDead = false;
        }
    }
    
    public void TakeDamage(int damage)
    {
        for(int i = 0; i < 40; ++i)
        {
            if(EnemyPool[i].activeSelf)
            {
                Enemy enem = EnemyModelPool[i].GetComponentInChildren<Enemy>();
                enem.Curr_HP -= damage;
            }
        }
        print("데미지 -" + damage.ToString() + "적용 완료");
    }

    public void SetSpot()
    {
        for (int i = 0; i < nNodeNum; ++i)
        {
            trNodeList.Add(GameObject.Find("MonsterRoute/Node" + i.ToString()).transform);
        }
    }

    public void LoadModelList()
    {
        GameObject go = new GameObject();
            
        for(int i = 0; i < 12; ++i)
        {
            if (i != 9)
            {
                go = Resources.Load<GameObject>("Enemy" + i.ToString());
            }
            EnemyModelList.Add(go);
        }
        for (int i = 0; i < 1; ++i)
        {
            go = Resources.Load<GameObject>("Boss" + i.ToString());
            BossModelList.Add(go);
        }

        ProgressBar = Resources.Load<GameObject>("ProgressBar");
       
    }

    public bool isEliminate()
    {
        if (!isBossGen) // 보스 라운드 아닐때
        {
            EliminatedNum = 0;
            for (int i = 0; i < 40; ++i)
            {
                if (EnemyPool[i].activeSelf == false)
                {
                    EliminatedNum++;
                }
            }
            if (EliminatedNum == 40 && nMadeMonsterNumber == 40 && EnemyModelPool.Count != 0)
            {
                //for (int i = 0; i < EnemyModelPool.Count; ++i)
                //{
                //    //Destroy(EnemyModelPool[i].gameObject);
                //}
                EnemyModelPool.Clear();
                nPoolIndex = 0;
                return true;
            }
            return false;
        }
        else   // 보스 라운드 일때
        {
            if(!Boss.activeSelf)
            {
                return true;
            }
            return false;
        }
    }

    public void EnemyStatusDbParsing(string p)
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

                string sqlQuery = "SELECT * FROM EnemyStatus";


                dbCmd.CommandText = sqlQuery;

                using (IDataReader reader = dbCmd.ExecuteReader()) // 테이블에 있는 데이터들이 들어간다. 
                {
                    while (reader.Read())
                    {
                        EnemyStatus es = new EnemyStatus(0, reader.GetInt32(0), reader.GetFloat(1), reader.GetFloat(2), 0, 0, 0);
                        
                        //Enemy e = Enemy SetEnemy(0, reader.GetInt32(0), reader.GetFloat(1), reader.GetFloat(2), 0, 0, 0);
                        EnemyStatusList.Add(es);
                    }
                    dbConnection.Close();
                    reader.Close();
                }
            }
        }
    }
}
