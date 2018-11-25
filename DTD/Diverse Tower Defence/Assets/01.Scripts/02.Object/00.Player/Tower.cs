using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class Tower : MonoBehaviour
{
    public string Name;
    public int ID;                       // 타워 ID
    public int Level;
    public int TowerType;                
    public float Attack;
    public float AtkSpd;
    public int SellCost;
    public int TowerIndex;               // 타워 타입에 따른 ID
    public float UpgradePerDamage;
    public int BulletType;                  // 공격타입 (즉발, 투사체, 광선, 멀티샷, 스플)
    public float Range;                  // 공격사거리
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
    public float AODamage;               // 장판데미지  Area Of Effect
    public float UPDAO;                  // 장판데미지 증가량
    public int MultiNum;                 // 멀티샷 추가 갯수
    public float MultiRate;              // 멀티샷 데미지 비율
    public int CurrUpgradeNum;           // 현재업글 숫자
    
    // =============================================================== //
    //public GameObject Target;   // 설정된 타깃
    public List<GameObject> Target = new List<GameObject>();    // 리스트의 0번이 타워가 바라보게 될 타겟
    public Vector3 vPosition;
    
    // =============================================================== //

    // ========================= 총알 발사 관련 ============================ //
    public Vector3 vMuzzlePos;      // 총알 시작위치
    public bool isCritical;         // 크리티컬 유무
    public float fCurrCooltime;
    public bool isStart = false;
    // =============================================================== //

    // =========================== 임시 UI 관련 ============================== //
    //public UILabel lbTowerName;
    //public UILabel lbTowerType;
    //public UILabel lbTowerLevel;
    //public UILabel lbTowerAtk;
    //public UILabel lbTowerUpgrade;
    //public UIPanel pnTowerInfo;
    // ====================================================================== // 

    private void Awake()
    {
        vMuzzlePos = GameObject.Find(this.name + "/Muzzle").transform.position;
        fCurrCooltime = AtkSpd;
       
    }
    private void Start()
    {
        if (GameManager.Instance.nCurrentScene == 1)
        {
            isStart = true;
        }
        StartCoroutine("AttackTimer");
    }

    public void Update()
    {
        CheckTowerUpgrade();
    }

    public void OnTriggerEnter(Collider other)
    {
        //if(Physics.OverlapSphere())
    }


    public void CheckTowerUpgrade()
    {
        if(TowerType == (int)ConstructManager.ETowerType.AoE || TowerType == (int)ConstructManager.ETowerType.Splash || TowerType == (int)ConstructManager.ETowerType.Laser)
        {
            CurrUpgradeNum = UpgradeManager.Instance.SplashAoEUpgrade;
        }
        else if(TowerType == (int)ConstructManager.ETowerType.Bouncing || TowerType == (int)ConstructManager.ETowerType.Multi)
        {
            CurrUpgradeNum = UpgradeManager.Instance.MultiBouncingUpgrade;
        }
        else if(TowerType == (int)ConstructManager.ETowerType.Critical || TowerType == (int)ConstructManager.ETowerType.Lucky || TowerType == (int)ConstructManager.ETowerType.Range)
        {
            CurrUpgradeNum = UpgradeManager.Instance.RangeUpgrade;
        }
        else if(TowerType == (int)ConstructManager.ETowerType.Dot || TowerType == (int)ConstructManager.ETowerType.Normal || TowerType == (int)ConstructManager.ETowerType.Rapid)
        {
            CurrUpgradeNum = UpgradeManager.Instance.RapidUpgrade;
        }
        else
        {

        }
    }

    public void SetTarget()
    {
        if(Target.Count != 0)
        {
            if(Target[0] == null)
            {
                Target.Clear();
            }
        }

        if (BulletType != (int)ConstructManager.EAttackType.Multi)
        {
            if (Target.Count == 0) // 타겟이 없으면 타깃을 설정한다
            {
                Collider[] cols = Physics.OverlapSphere(transform.position, Range, 1 << 15);
                
                float fRealRange = float.MaxValue;
                GameObject target = null;
                GameObject Dest = null;
                foreach (Collider col in cols)
                {
                    GameObject Temptarget = col.gameObject;
                    if (Temptarget.CompareTag("Enemy"))
                    {
                        Vector3 dir = Temptarget.transform.position - transform.position;
                        float fRange = dir.magnitude;
                        if (fRange <= fRealRange)
                        {
                            fRealRange = fRange;
                            target = Temptarget;
                        }
                    }
                    Dest = target;
                }
                if(Dest != null)
                Target.Add(Dest);

                if (Target.Count != 0)
                {
                    transform.LookAt(new Vector3(Target[0].transform.position.x, 0, Target[0].transform.position.z));
                }
            }
            else    // 타겟이 있으면
            {
                transform.LookAt(new Vector3(Target[0].transform.position.x, 0, Target[0].transform.position.z));
                if (Vector3.Distance(vPosition, Target[0].transform.position) > Range)
                {
                    Target[0] = null;
                }
            }
        }
        else        // 멀티샷일때
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, Range);
            int TargetNum = 0;
            int TargetNum2 = 0;

            if (Target.Count == 0) // 타겟이 없으면 타깃을 설정한다
            {  
                float fRealRange = float.MaxValue;
                GameObject target = null;
                foreach (Collider col in cols)
                {
                    GameObject Temptarget = col.gameObject;
                    if (Temptarget.CompareTag("Enemy"))
                    {
                        Vector3 dir = Temptarget.transform.position - transform.position;
                        float fRange = dir.magnitude;
                        if (fRange <= fRealRange)
                        {
                            fRealRange = fRange;
                            target = Temptarget;
                        }
                        TargetNum++;
                    }
                }
                if(target != null)
                Target.Add(target);

                if (Target.Count != 0)
                {
                    transform.LookAt(new Vector3(Target[0].transform.position.x, 0, Target[0].transform.position.z));
                }
            }

            if(Target.Count != 0)    // 타겟이 있으면 0번 인덱스가 타깃
            {
                TargetNum = 0;
                foreach (Collider col in cols)
                {
                    GameObject Temptarget = col.gameObject;
                    if (Temptarget.CompareTag("Enemy"))
                    {
                        TargetNum++; 
                    }
                }
                
                Collider[] cols2 = new Collider[TargetNum];
                
                foreach (Collider col in cols)
                {
                    if (col.tag == "Enemy")
                    {
                        cols2[TargetNum2] = col;
                        TargetNum2++;
                    }
                }
                
                if (cols2.Length > MultiNum)
                {
                    for (int i = 0; i < cols2.Length - 1; ++i)            // 버블정렬로 cols를 가까운 순서대로 재정렬한다.
                    {
                        for (int j = 0; j < cols2.Length - 1; ++j)
                        {
                            float Range = (cols2[j].gameObject.transform.position - transform.position).magnitude;
                            float Range2 = (cols2[j + 1].gameObject.transform.position - transform.position).magnitude;
                
                            if (Range2 < Range)
                            {
                                Collider Temp = cols2[j];
                                cols2[j] = cols2[j + 1];
                                cols2[j + 1] = Temp;
                            }
                        }
                    }
                }
                
                for (int i = 0; i < cols2.Length; ++i) // for문을 통해 0번자리와 겹치지 않는 녀석들을 전부 넣어준다.
                {
                    if (cols2[i].gameObject != Target[0])
                    {
                        Target.Add(cols2[i].gameObject);
                    }
                }
                
                while(Target.Count > MultiNum + 1)      // 멀티샷 포함 갯수보다 타깃이 많을경우 지워준다
                {
                    Target.Remove(Target[Target.Count-1]);
                }

                transform.LookAt(new Vector3(Target[0].transform.position.x, 0, Target[0].transform.position.z));
                if (Vector3.Distance(vPosition, Target[0].transform.position) > Range)
                {
                    Target[0] = null;
                }
            }
        }
        
    } // 일단 타깃 1마리만 설정댐

   // public void MakeBullet(GameObject target)
   // { 
   //     Enemy e = target.GetComponent<Enemy>();
   //     int targetIndex = e.Id; 
   //     BulletManager.Instance.MakeBullet(ID, e.Id, 0);
   //     fCurrCooltime = AtkSpd;
   // }

    public void MakeBullet(List<GameObject> targets)
    {
        if(TowerType == (int)ConstructManager.ETowerType.Critical)  // 크리티컬 타워
        {
            float n = Random.Range(0.0f, 1.0f);
            
            if (n >= CriticalProb)
                isCritical = false;
            else
                isCritical = true;
            
        }

        if (targets != null)
        {
            if (BulletType != (int)ConstructManager.EAttackType.Laser)
            {
                for (int i = 0; i < targets.Count; ++i)
                {
                    //Enemy e = targets[i].GetComponent<Enemy>();
                    BulletManager.Instance.MakeBullet(ID, i, isCritical);
                }
            }
            else
            {
                for (int i = 0; i < targets.Count; ++i)
                {
                    //Enemy e = targets[i].GetComponent<Enemy>();
                    BulletManager.Instance.MakeLaserBullet(ID, i, isCritical);
                }
            }
            fCurrCooltime = AtkSpd;
        }
        else    // 근접타워는 타깃이 피료없다!
        {
            BulletManager.Instance.MakeBullet(ID, 0, false);
            fCurrCooltime = AtkSpd;
        }
        
        if(TowerType == (int)ConstructManager.ETowerType.Multi)
        {
            targets.Clear();
        }
    }


    public void SetTowerStatus(TowerStatus ts, int towerIndex /* 쓸모가 없음.. ts에 정보가 들어있으 */ , int groundIndex)
    {
        Name = ts.Name;
        ID = groundIndex;
        Level = ts.Level;
        TowerType = ts.TowerType;
        Attack = ts.Attack;
        AtkSpd = ts.AtkSpd;
        SellCost = ts.SellCost;
        TowerIndex = ts.TowerIndex;
        UpgradePerDamage = ts.UpgradePerDamage;
        BulletType = ts.BulletType;
        Range = ts.Range;
        BounceDDR = ts.BounceDDR;
        BulletSpd = ts.BulletSpd;
        BounceNum = ts.BounceNum;
        MinDamage = ts.MinDamage;
        MaxDamage = ts.MaxDamage;
        UpgradePerMinD = ts.UpgradePerMinD;
        UpgradePerMaxD = ts.UpgradePerMaxD;
        SplashRange = ts.SplashRange;
        CriticalProb = ts.CriticalProb;
        CriticalMagni = ts.CriticalMagni;
        RayLength = ts.RayLength;
        RayDirX = ts.RayDirX;
        RayDirZ = ts.RayDirZ;
        DotSustainTime = ts.DotSustainTime;
        DotDamage = ts.DotDamage;
        UPDD = ts.UPDD;
        MaxDotST = ts.MaxDotST;
        AOSustainTime = ts.AOSustainTime;
        AODamage = ts.AODamage;
        UPDAO = ts.UPDAO;
        MultiNum = ts.MultiNum;
        MultiRate = ts.MultiRate;
        CurrUpgradeNum = ts.CurrUpgradeNum;
        vPosition = ConstructManager.Instance.groundInfoList[groundIndex].position;
        vPosition.y = 0.0f;
    }

    public void SetTowerStatus(TowerStatus ts)
    {
        Name = ts.Name;
        ID = 0;
        Level = ts.Level;
        TowerType = ts.TowerType;
        Attack = ts.Attack;
        AtkSpd = ts.AtkSpd;
        SellCost = ts.SellCost;
        TowerIndex = ts.TowerIndex;
        UpgradePerDamage = ts.UpgradePerDamage;
        BulletType = ts.BulletType;
        Range = ts.Range;
        BounceDDR = ts.BounceDDR;
        BulletSpd = ts.BulletSpd;
        BounceNum = ts.BounceNum;
        MinDamage = ts.MinDamage;
        MaxDamage = ts.MaxDamage;
        UpgradePerMinD = ts.UpgradePerMinD;
        UpgradePerMaxD = ts.UpgradePerMaxD;
        SplashRange = ts.SplashRange;
        CriticalProb = ts.CriticalProb;
        CriticalMagni = ts.CriticalMagni;
        RayLength = ts.RayLength;
        RayDirX = ts.RayDirX;
        RayDirZ = ts.RayDirZ;
        DotSustainTime = ts.DotSustainTime;
        DotDamage = ts.DotDamage;
        UPDD = ts.UPDD;
        MaxDotST = ts.MaxDotST;
        AOSustainTime = ts.AOSustainTime;
        AODamage = ts.AODamage;
        UPDAO = ts.UPDAO;
        MultiNum = ts.MultiNum;
        MultiRate = ts.MultiRate;
        CurrUpgradeNum = ts.CurrUpgradeNum;
    }
    public void UpgradeTowerAttack()
    {
        CurrUpgradeNum++;
    }


    private void OnDrawGizmos()
    {
        if (GameManager.Instance.nCurrentScene == 2)
        {
            Gizmos.color = Color.red;

            if (TowerType == (int)ConstructManager.ETowerType.Normal)
            {
                Gizmos.DrawWireSphere(transform.position, SplashRange);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, Range);
            }
        }
    }

    private IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(0.2f);
        if (fCurrCooltime > 0.0f)
        {
            fCurrCooltime -= 0.2f;
        }
        else if(isStart && fCurrCooltime <= 0)       // 타겟이 있고 쿨타임이 0보다 줄어들었다면
        {
            vMuzzlePos = GameObject.Find(this.name + "/Muzzle").transform.position;

            if (TowerType != (int)ConstructManager.ETowerType.Normal)
            {
                SetTarget();

                if (Target.Count != 0 && Target[0] != null)
                {
                    MakeBullet(Target);
                }
            }
            else
            {
                MakeBullet(null);
            }
        }
        StartCoroutine("AttackTimer");
    }
}
