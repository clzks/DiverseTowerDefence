using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class Bullet : MonoBehaviour
{

    public int nId;
    public GameObject Target;
    public int nTargetId;
    public Animation anim;
    public int nTowerId;
    public int nEffectId;     // ??? 뭔디 아마도 이펙트에 id값을 줘서 불러올라고 했던것같음 (파티클) 
    public bool doDamaged;    // 피해를 주었는가?

    // 총알의 스탯
    public float fAttack;
    public int nTowerType;
    public int nAttackType;
    public float fSplashRange;
    public int nBounceTargetFirst;        // 필요한가..?
    public int nBounceTargetSecond;       // 필요한가..?
    public int nBounceTargetThird;        // 필요한가..?
    public float AoESustainTime;          // 광역총알의 남은 지속시간


    public int nBouncingNum;
    public int nCurrBouncingNum = 0;
    public List<int> nBouncingTargets = new List<int>();
    public List<bool> isBouncingDoDamage = new List<bool>();
    public float fBouncePerDamage;

    public bool nBounceFirstDoDamage = false;
    public bool nBounceSecondDoDamage = false;
    public bool nBounceThirdDoDamage = false;

    public bool isCritical;

    public bool isAttackAoE = false;
    float nTimer = 0.5f;    // 광역 타이머
    // ================ 총알이 날아가는데 필요 ======================== //
    public float fSpeed;
    public Vector3 vDir = new Vector3(0, 0, 0);
    public Vector3 vDestination;
    // ============================================================== //

    // ======================== 데미지 출력 ========================== //
    public Transform trLabelPool;
    public List<Enemy> EnemyList = new List<Enemy>();
    // ============================================================== //

    private void Awake()
    {
        
    }

    public void SetBullet(int towerId, int BulletId, int TargetListIndex, bool isCri)
    {
        //int TowerTypeIndex = ConstructManager.Instance.groundInfoList[towerId].TowerIndex;
        Tower t = ConstructManager.Instance.UserTowerDic[towerId].GetComponent<Tower>();
        nId = BulletId;
        nAttackType = t.BulletType;     // 부득이하게 위로 올림
        isCritical = isCri;         // 크리티컬인지는 타워에서 결정을 지어줘야한다 이것도 위로 올려야함

        if (t.TowerType != 6)
        {
            Target = t.Target[TargetListIndex];
            Enemy e = Target.GetComponent<Enemy>();
            nTargetId = e.Id;
        }
        else
        {
            Target = null;
        }

        

        transform.position = t.vMuzzlePos;
        AoESustainTime = t.AOSustainTime;
        nTowerId = towerId;
        nEffectId = 0;
        doDamaged = false;    // 피해를 주었는가?

        // 총알의 스탯  ------- 일단 투사체일때만
        nTowerType = t.TowerType;
        fSplashRange = t.SplashRange;
        nBounceTargetFirst = -1;           // 바운싱 관련 타깃
        nBounceTargetSecond = -1;          // 바운싱 관련 타깃
        nBounceTargetThird = -1;           // 바운싱 관련 타깃
        fBouncePerDamage = t.BounceDDR;
        nBouncingNum = t.BounceNum;
        fSpeed = t.BulletSpd;

        fAttack = SetDamage(t);     // 데미지는 타워에서 결정하는구나, 최종으로 결정 
        //lbDamage.text = fAttack.ToString();
    }

   

    private void FixedUpdate()
    {
        if(nTowerType == 6)
            DestroyBullet();

        if (Target != null)
        {
            if (GameManager.Instance.nCurrentScene == 1) // Test맵에서는 이네미가 상위 개체를 가지고 있어서 그
            {
                vDir = Vector3.Normalize(Target.transform.parent.transform.position - transform.position);
            }
            else if (GameManager.Instance.nCurrentScene == 2)   // BulletTest에서는 상위개체를 안가지고 있다.
            {
                vDir = Vector3.Normalize(Target.transform.transform.position - transform.position);
            }
            vDestination = Target.transform.position;
        }
        else
        {
            vDir = Vector3.Normalize(vDestination - transform.position);

            if (Vector3.Distance(transform.position, vDestination) <= 0.05f)
            {
                if (nTowerType != 9)
                {
                    DestroyBullet();
                }
                else
                {
                    isAttackAoE = true;
                }
            }

        }

        transform.position += fSpeed * vDir * 0.1f * GameManager.Instance.nGameSpeed;

        //switch (nAttackType)
        //{
        //    case 0:
        //
        //    if (Target != null)
        //    {
        //        transform.position = Target.transform.position;
        //        vDestination = Target.transform.position;
        //    }
        //    else
        //    {
        //        vDir = Vector3.Normalize(vDestination - transform.position);
        //
        //        if (Vector3.Distance(transform.position, vDestination) <= 0.05f)
        //        {
        //            doDamaged = true;
        //        }
        //
        //    }
        //
        //    break;
        //
        //    case 1:
        //
        //    if (Target != null)
        //    {
        //        if (GameManager.Instance.nCurrentScene == 1) // Test맵에서는 이네미가 상위 개체를 가지고 있어서 그녀석을 따라가는것이며
        //        {
        //            vDir = Vector3.Normalize(Target.transform.parent.transform.position - transform.position);       
        //        }
        //        else if (GameManager.Instance.nCurrentScene == 2)   // BulletTest에서는 상위개체를 안가지고 있다.
        //        {
        //            vDir = Vector3.Normalize(Target.transform.transform.position - transform.position);
        //        }
        //        vDestination = Target.transform.position;
        //    }
        //    else
        //    {
        //        vDir = Vector3.Normalize(vDestination - transform.position);
        //
        //        if (Vector3.Distance(transform.position , vDestination) <= 0.05f)
        //        {
        //            doDamaged = true;
        //        }
        //
        //    }
        //    break;
        //
        //    case 5:                         // 바운싱 뷸렛
        //
        //    if (Target != null)             // 바운싱 뷸렛도 타깃을 바꿔가면서 계속 쫒아간다.
        //    {
        //        if (GameManager.Instance.nCurrentScene == 1) // Test맵에서는 이네미가 상위 개체를 가지고 있어서 그녀석을 따라가는것이며
        //        {
        //            vDir = Vector3.Normalize(Target.transform.parent.transform.position - transform.position);
        //        }
        //        else if (GameManager.Instance.nCurrentScene == 2)   // BulletTest에서는 상위개체를 안가지고 있다.
        //        {
        //            vDir = Vector3.Normalize(Target.transform.transform.position - transform.position);
        //        }
        //        vDestination = Target.transform.position;
        //        isArriveDest = false;
        //    }
        //    else
        //    {
        //        vDir = Vector3.Normalize(vDestination - transform.position);
        //
        //        if (Vector3.Distance(transform.position, vDestination) <= 0.05f)
        //        {
        //            isArriveDest = true;
        //        }
        //
        //        if(nCurrBouncingNum >= nBouncingNum)
        //        {
        //            doDamaged = true;
        //        }
        //        if(isArriveDest) // 타깃이 도중에 죽어서 사라진 그 자리에 도달했을 경우나 타깃에게 도달했을 경우
        //        {
        //            if(!FindBouncingTarget())
        //            {
        //                doDamaged = true;
        //            }
        //        }
        //    }
        //
        //    break;
        //
        //    case 3:             // 멀티샷
        //
        //    if (Target != null)
        //    {
        //        if (GameManager.Instance.nCurrentScene == 1) // Test맵에서는 이네미가 상위 개체를 가지고 있어서 그녀석을 따라가는것이며
        //        {
        //            vDir = Vector3.Normalize(Target.transform.parent.transform.position - transform.position);
        //        }
        //        else if (GameManager.Instance.nCurrentScene == 2)   // BulletTest에서는 상위개체를 안가지고 있다.
        //        {
        //            vDir = Vector3.Normalize(Target.transform.transform.position - transform.position);
        //        }
        //        vDestination = Target.transform.position;
        //    }
        //    else
        //    {
        //        vDir = Vector3.Normalize(vDestination - transform.position);
        //
        //        if (Vector3.Distance(transform.position, vDestination) <= 0.05f)
        //        {
        //                doDamaged = true;
        //        }
        //    }
        //    break;
        //
        //    case 4:                 // 스플래쉬       
        //        if (Target != null)
        //        {
        //            if (GameManager.Instance.nCurrentScene == 1) // Test맵에서는 이네미가 상위 개체를 가지고 있어서 그녀석을 따라가는것이며
        //            {
        //                vDir = Vector3.Normalize(Target.transform.parent.transform.position - transform.position);
        //            }
        //            else if (GameManager.Instance.nCurrentScene == 2)   // BulletTest에서는 상위개체를 안가지고 있다.
        //            {
        //                vDir = Vector3.Normalize(Target.transform.transform.position - transform.position);
        //            }
        //            vDestination = Target.transform.position;
        //        }
        //        else
        //        {
        //            vDir = Vector3.Normalize(vDestination - transform.position);
        //
        //            if (Vector3.Distance(transform.position, vDestination) <= 0.05f)
        //            {
        //                isArriveDest = true;
        //            }
        //        }
        //        break;
        //}

    }

    private void Update()
    {
        // 장판데미지는 이곳에서 할수밖에 없을듯..
        AoEBullet();
    }

    

    private void OnTriggerEnter(Collider other)  // 데미지 입히는 부분
    {
        if(!doDamaged && other.tag == "Enemy")
        {
            Enemy e = other.GetComponent<Enemy>();

            if (nTargetId == e.Id)      // 적의 Id와 타깃의 아이디가 동일하다면
            {
                if (nAttackType != 4)   // 스플래쉬 타입이 아니면
                {
                    EnemyList.Add(e);
                    DestroyBullet();
                }
                else                    // 스플래쉬 타입중에
                {
                    if (nTowerType != 9)  // 장판타워가 아니라면
                    {
                        DestroyBullet();
                    }
                    else                  // 장판타워라면
                    {
                        isAttackAoE = true;
                    }
                }
                //if(nTowerType == 1)
                //{
                //    isArriveDest = true;
                //    Collider[] cols = Physics.OverlapSphere(transform.position, fSplashRange);
                //
                //    foreach (Collider col in cols)
                //    {
                //        GameObject Temptarget = col.gameObject;
                //        if (Temptarget.CompareTag("Enemy"))
                //        {
                //            Enemy ee = Temptarget.GetComponent<Enemy>();
                //            ee.Damage(fAttack);
                //        }
                //    }
                //}
                //doDamaged = true;
                //e.Damage(this);

                //switch (nTowerType)
                //{
                //    case 0:             // 레인지
                //        e.Damage(fAttack);
                //        break;
                //
                //    case 1:             // 스플래쉬
                //        isArriveDest = true;
                //        Collider[] cols = Physics.OverlapSphere(transform.position, fSplashRange);
                //
                //        foreach (Collider col in cols)
                //        {
                //            GameObject Temptarget = col.gameObject;
                //            if (Temptarget.CompareTag("Enemy"))
                //            {
                //                Enemy ee = Temptarget.GetComponent<Enemy>();
                //                ee.Damage(fAttack);
                //            }
                //        }
                //        doDamaged = true;
                //        break;
                //
                //    //case 2:             // 바운싱
                //    //e.Damage(fAttack);
                //    //break;
                //
                //    case 3:             // 렄키
                //        e.Damage(fAttack);
                //        break;
                //
                //    case 4:             // 크리티컬
                //        e.Damage(fAttack);
                //        break;
                //
                //    case 5:             // 래피드
                //        e.Damage(fAttack);
                //        break;
                //
                //    case 6:             // 노멀
                //        e.Damage(fAttack);
                //        break;
                //
                //    case 7:             // 레이저
                //        e.Damage(fAttack);
                //        break;
                //
                //    case 8:             // 도트뎀
                //        e.Damage(fAttack);
                //        break;
                //
                //    case 9:             // 광역
                //        e.Damage(fAttack);
                //        break;
                //
                //    case 10:             // 멀티샷
                //        e.Damage(fAttack);
                //        break;
                //
                //    case 11:             // 랜덤
                //        e.Damage(fAttack);
                //        break;
                //
                //    case 12:             // 메타몬
                //        e.Damage(fAttack);
                //        break;
                //
                //    case 13:             // 랜덤
                //        e.Damage(fAttack);
                //        break;
                //
                //    case 14:             // 랜덤
                //        e.Damage(fAttack);
                //        break;
                //
                //    case 15:             // 랜덤
                //        e.Damage(fAttack);
                //        break;
                //}
                //if (nTowerType != 2)
                //{
                //    doDamaged = true;
                //}
                //else    // 바운싱 타워일때 데미지 입히는 것
                //{
                //    isArriveDest = true;
                //    nBouncingTargets.Add(e.Id);
                //    e.Damage(BounceDamage(fAttack, nCurrBouncingNum));
                //    nCurrBouncingNum++;
                //    Target = null;
                //}
            }
        }
    }
    public float SetDamage(Tower t)
    {
        float Damage = 0;
        switch (t.TowerType)
        {

            case 0:     // Range계열
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;
                break;

            case 1:     // 스프라시 계열
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;
                break;

            case 2:     // 바운싱
                Damage = BounceDamage(t.Attack, t.BounceNum); // 수정필요 하겠지
                break;

            case 3:     // 럭키
                Damage = Random.Range(t.MinDamage + t.UpgradePerMinD * t.CurrUpgradeNum, t.MaxDamage + t.UpgradePerMaxD * t.CurrUpgradeNum);
                break;

            case 4:     // 크리티커르 타워
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;

                if (isCritical)
                {
                    Damage *= t.CriticalMagni;
                }
                break;

            case 5:     // 래휘드
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;
                break;

            case 6:     // 근접
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;
                break;

            case 7:     // 레이져
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;
                break;

            case 8:     // 독 뎀
                Damage = t.Attack + t.CurrUpgradeNum * t.UPDD;
                break;

            case 9:     // 장판
                Damage = t.AODamage + t.UPDAO * t.CurrUpgradeNum;
                break;

            case 10:    // 멀티샷
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;
                break;

            case 11:    // 란돔
                break;

            case 12:    // 메타몬
                break;
        }
        


        return Damage;
    }

    private void OnDestroy()
    {
        for(int i = 0; i< EnemyList.Count; ++i)
        {
            EnemyList[i].Damage(fAttack);   // 데미지는 타워에서 결정
        }
    }

    public void FindSplashTargets()
    {
        if (nAttackType == 4) // SPlash
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, fSplashRange);

            foreach (Collider col in cols)
            {
                GameObject Temptarget = col.gameObject;
                if (Temptarget.CompareTag("Enemy"))
                {
                    Enemy ee = Temptarget.GetComponent<Enemy>();
                    EnemyList.Add(ee);
                }
            }
        }
    }

    public void AoEBullet()
    {
        
        if (nTowerType == 9 && isAttackAoE)
        {
            if (AoESustainTime > 0)
            {
                Collider[] cols = Physics.OverlapSphere(transform.position, fSplashRange);

                foreach (Collider col in cols)
                {
                    GameObject Temptarget = col.gameObject;
                    if (Temptarget.CompareTag("Enemy"))
                    {
                        Enemy ee = Temptarget.GetComponent<Enemy>();
                        EnemyList.Add(ee);
                    }
                }

                if (nTimer <= 0.5f)
                {
                    for (int i = 0; i < EnemyList.Count; ++i)
                    {
                        EnemyList[i].Damage(fAttack);   // 데미지는 타워에서 결정
                    }
                }
                nTimer -= Time.deltaTime;
                AoESustainTime -= Time.deltaTime;
            }
            if(nTimer <= 0.0f)
            {
                nTimer = 1.0f;
            }

            if(AoESustainTime <= 0)
            {
                DestroyBullet();
            }
        }
    }

    public float BounceDamage(float Atk, int nBounceNum)
    {
        if(nBounceNum > 0)
        {
            for(int i = 0; i < nBounceNum; ++i)
            {
                Atk *= fBouncePerDamage;
            }
        }

        return Atk;
    }

    public bool FindBouncingTarget()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 4.0f);
        float fRealRange = float.MaxValue;
        GameObject target = null;
        bool isExist = false;
        
        foreach (Collider col in cols)
        {
            GameObject Temptarget = col.gameObject;
            if (Temptarget.CompareTag("Enemy"))
            {
                Enemy e = Temptarget.GetComponent<Enemy>();
                 
                bool isOverlap = false;

                for (int i = 0; i < nBouncingTargets.Count; ++i)
                {
                    if (e.Id == nBouncingTargets[i])
                    {
                        isOverlap = true;
                    }
                }

                if (isOverlap)
                {
                    continue;
                }

                Vector3 dir = Temptarget.transform.position - transform.position;
                float fRange = dir.magnitude;
                if (fRange <= fRealRange)
                {
                    fRealRange = fRange;
                    target = Temptarget;
                }
                isExist = true;
            }
            Target = target;
            if (Target)
            {
                nTargetId = target.GetComponent<Enemy>().Id;
            }
        }
        return isExist;
    }

    public void DestroyBullet()
    {
        FindSplashTargets();

        GameObject Bullet = transform.parent.gameObject;
        Bullet.SetActive(false);
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        switch (nAttackType)
        {

            case 5:
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 4.0f);
                break;

            case 4:
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, fSplashRange);
                break;
        }
    }

}


