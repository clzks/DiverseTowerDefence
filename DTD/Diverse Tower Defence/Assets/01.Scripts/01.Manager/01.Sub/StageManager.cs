using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StageManager : MonoBehaviour
{
  
    public bool isGameStart = false;
    public int nLife = 100;
    public int nNodeNum = 24; // 최대 노드 수 

    // ========== 스테이지 세팅 관련 =============== \\

    public int nStage = 0;
    public List<Transform> trNodeList = new List<Transform>();         // 각각의 노드
    // ============================================ \\ 



    // ========== UI 세팅 관련 ===================== \\

    private UILabel timer;
    private UILabel life;
    private UILabel StageNumber;
      private bool isSetLabel = false;
    private bool isSetSpot = false;

    //private UILabel lbTower0Info;
    //private UILabel lbTower1Info;
    //private UILabel lbTower2Info;
    //private UILabel lbTower3Info;
    //private List<UILabel> listLbTowerInfo = new List<UILabel>();

    // ============================================= \\ 



    // =========== 라운드 진행 관련 ================= \\

    private float fWatingTime;
    public float fSetWaitTime = 5.0f;

    public int nPhase = 0;
    // 0:   대기시간이 가는 단계
    // 1:   유닛이 하나씩 등장하는 단계
    // 2:   유닛이 모두 등장한 단계
    // 3:   유닛이 모두 죽거나 사라진 단계(퀘스트도)
    // 4:   대기시간 세팅 -> phase1로
    // 5:   모든 단계 클리어 
    //    

    // ============================================= \\

    private static object _Lock = new object();
    public static StageManager instance = null;
    public static StageManager Instance
    {
        get
        {
            lock (_Lock)

            {
                if (instance == null)
                {
                    instance = GameObject.Find("Manager").AddComponent<StageManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }

    private void Start()
    {
        for (int i = 0; i < nNodeNum; ++i)
        {
            trNodeList.Add(GameObject.Find("MonsterRoute/Node" + i.ToString()).transform);
        }

    }

    private void Update()
    {
        if (isSetSpot && isGameStart)
        {
            StageController();
        }
    }
    private void FixedUpdate()
    {

        switch (GameManager.Instance.nCurrentScene)
        {
            case 1: // Test
            SetPool(); //한번만하고싶음
            SetLabel();  //한번만하고싶음
            if (isSetSpot && isGameStart)
            {
            //StageController();

            switch (nPhase)
            {
                case 0:
                    fWatingTime -= Time.deltaTime * GameManager.Instance.nGameSpeed;
                    if (fWatingTime <= 0)
                    {
                        fWatingTime = 0;
                        nPhase = 1;
                    }
                break;

                case 1: // ResponseEnemy 할때 살짝 더러운 느낌 
                    if (nStage % 10 != 9)
                    {
                        if (EnemyManager.Instance.nPoolIndex < 40)
                        {
                            EnemyManager.Instance.ResponeEnemy(nStage, trNodeList[EnemyManager.Instance.MonsterRouteList[0][0]].position, 0);
                        }
                        else
                        {
                            nPhase = 2;
                        }
                    }
                    else // 보스 젠
                    {
                        EnemyManager.Instance.ResponeBoss(nStage / 10, trNodeList[EnemyManager.Instance.MonsterRouteList[0][0]].position, 0);
                        nPhase = 2;
                    }
                    //EnemyManager.Instance.RunEnemy();
                    break;

                case 2:
                    //EnemyManager.Instance.RunEnemy();
                    if (EnemyManager.Instance.IsEliminate())
                    {
                        nPhase = 3;
                    }
                    break;

                case 3:
                    if (nStage % 10 == 9)
                    {
                        fWatingTime = 4.0f * fSetWaitTime;
                    }
                    else
                    {
                        fWatingTime = fSetWaitTime;
                    }
                    EnemyManager.Instance.isBossGen = false;
                    nStage++;
                    nPhase = 4;
                break;

                case 4:
                    EnemyManager.Instance.nMadeMonsterNumber = 0;
                    nPhase = 0;
                break;

                case 5:

                break;
            }
            

            //if (isWaitTime)
            //    fWatingTime -= Time.deltaTime;
            //
            //if (fWatingTime <= 0.0f && isRegenTime == false)
            //{
            //    isWaitTime = false;
            //    isRegenTime = true;
            //    EnemyManager.Instance.nMadeMonsterNumber = 0;
            //}
            //if (isRegenTime)
            //{
            //    if (nStage % 10 != 9)
            //    {
            //        if (EnemyManager.Instance.nPoolIndex < 40)
            //        {
            //            EnemyManager.Instance.ResponeEnemy(nStage, 2.0f, EnemyManager.Instance.trNodeList[0].position);
            //        }
            //    }
            //    else // 보스 젠
            //    {
            //        EnemyManager.Instance.ResponeBoss(nStage / 10, EnemyManager.Instance.trNodeList[0].position);
            //    }
            //}
            //
            //if (EnemyManager.Instance.isEliminate() && isWaitTime == false)
            //{
            //    isWaitTime = true;
            //    isRegenTime = false;
            //    fWatingTime = fSetWaitTime;
            //    EnemyManager.Instance.isBossGen = false;
            //    nStage++;
            //}
            //else
            //{
            //    EnemyManager.Instance.RunEnemy();
            //}
            }
            break;

            case 2: // BulletTest

            break;
        }
        

    }

    public void SetUI()
    {
        timer = GameObject.Find("UI Root/Camera/Timer").GetComponent<UILabel>();
        life = GameObject.Find("UI Root/Camera/Life").GetComponent<UILabel>();
        StageNumber = GameObject.Find("UI Root/Camera/StageNumber").GetComponent<UILabel>();
        EnemyManager.Instance.BarParent = GameObject.Find("UI Root/Camera/ProgressBar").transform;
        //for (int i = 0; i < 4; ++i)
        //{
        //    UILabel lb = GameObject.Find("UI Root/Camera/ConstructInfo/" + i.ToString()).GetComponent<UILabel>();
        //    listLbTowerInfo.Add(lb);
        //}
        for (int i = 0; i < ConstructManager.Instance.nGroundNum; ++i)
        {
            GameObject g = Instantiate(ConstructManager.Instance.TowerLabel, GameObject.Find("UI Root/Camera/ConstructInfo").transform);
            //UILabel Label = g.GetComponent<UILabel>();
            //Label.transform.parent = GameObject.Find("UI Root/Camera/ConstructInfo").transform;
            ConstructManager.Instance.LabelList.Add(g);
        }


        isSetLabel = true;
    }

    public void SetLabel()
    {
        if (isSetLabel)                              // 라벨 세팅 
        {
            timer.text = fWatingTime.ToString();
            life.text = nLife.ToString();
            StageNumber.text = (nStage+1).ToString();

#if DEBUG
            for (int i = 0; i < ConstructManager.Instance.nGroundNum; ++i)
            {
               if (ConstructManager.Instance.groundInfoList[i].TowerIndex != -1)
               {
                   Tower t = ConstructManager.Instance.UserTowerDic[i].GetComponent<Tower>();
                   ConstructManager.Instance.LabelList[i].GetComponent<UILabel>().text = t.Name + "\r\n레벨 : " + t.Level.ToString() + "\r\n공격력 : " + t.Attack.ToString() + "\r\n업글 : " + t.CurrUpgradeNum.ToString();
               }
               else
               {
                    ConstructManager.Instance.LabelList[i].GetComponent<UILabel>().text = "타워 없음";
               }
            }
#endif
        }
    }
    public void SetPool()
    {
        if (GameManager.Instance.nCurrentScene == 1 && !isSetSpot)   // 적군생성위치 세팅 및 이네미 풀 만들기
        {
            SetUI();
            EnemyManager.Instance.SetSpot();
            isSetSpot = true;
            EnemyManager.Instance.MakeEnemyPool();
            //EnemyManager.Instance.MakeEnemyDamagePool();
            BulletManager.Instance.MakeBulletPool();
        }
    }
    
    // 현재 보스 전 스테이지에서 보스 스테이지 넘어갈때 문제있슴
    private void StageController()
    {
        if(Input.GetKeyDown(KeyCode.R))         // 스테이지 넘기기
        {
            if (EnemyManager.Instance.EliminatedNum != 40)
            {
                EnemyManager.Instance.nPoolIndex = 40;
                EnemyManager.Instance.nMadeMonsterNumber = 40;
                EnemyManager.Instance.EliminatedNum = 40;
                nStage++;

                fWatingTime = 5.0f;
                //print("스테이지 넘기기");
                Debug.Log("스테이지 넘기기");
            }
            else
            {
                fWatingTime = 5.0f;
                nStage++;
                Debug.Log("스테이지 넘기기");
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            EnemyManager.Instance.TakeDamage(100);
        }
    }
}
