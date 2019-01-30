using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class Bullet : MonoBehaviour
{
    //public ParticleSystem ps;
    public int nId;
    public GameObject Target;
    public int nTargetId;
    //public Animation anim;
    public int nTowerId;
    public int nMovingEffectId;                   // 이동중 적용될 파티클 이펙트의 ID 
    public int nExtinctionEffectId;         // 소멸 시 적용될 파티클 이펙트의 ID
    public Effect MovingEffect;
    //public Effect ExtinctionEffect;
    public bool doDamaged;    // 피해를 주었는가?

    public float BulletSpd = 0.05f;

    // 총알의 스탯
    public float fAttack;
    public int nTowerType;
    public int nAttackType;
    public float fSplashRange;
    public float AoESustainTime;          // 광역총알의 남은 지속시간
    public float nLife;
    public int nBouncingNum;
    public int nCurrBouncingNum = 0;
    public int nDotTime;                  // Dot 지속시간
    public float fDotDamage;
    public int nlevel;

    public List<int> nBouncingTargets = new List<int>();
    //public List<bool> isBouncingDoDamage = new List<bool>();
    public float fBouncePerDamage;
    public bool isCritical;

    public bool isAttackAoE = false;
    // ================ 총알이 날아가는데 필요 ======================== //
    public float fSpeed;
    public Vector3 vDir = new Vector3(0, 0, 0);
    public Vector3 vDestination;
    // ============================================================== //

    // ======================== 데미지 출력 ========================== //
    public Transform trLabelPool;
    public List<Enemy> EnemyList = new List<Enemy>();
    // ============================================================== //

    public GameObject HitPoint = null;
    public GameObject LaserBody = null;

    private void Awake()
    {
        if (nAttackType == (int)ConstructManager.EAttackType.Laser)
        {
            StartCoroutine("LaserAttack");
        }
    }

    private void Start()
    {
        if (nAttackType == (int)ConstructManager.EAttackType.Laser)
        {
            vDir = Vector3.Normalize(Target.transform.parent.transform.position - transform.position);
            LaserBody.transform.rotation = Quaternion.LookRotation(vDir);
        }
    }

    public void SetBullet(int towerId, int BulletId, int TargetListIndex, bool isCri)
    {
       
        //int TowerTypeIndex = ConstructManager.Instance.groundInfoList[towerId].TowerIndex;
        Tower t = ConstructManager.Instance.UserTowerDic[towerId].GetComponent<Tower>();
        nId = BulletId;
        nAttackType = t.BulletType;     // 부득이하게 위로 올림
        isCritical = isCri;         // 크리티컬인지는 타워에서 결정을 지어줘야한다 이것도 위로 올려야함
        nDotTime = (int)t.DotSustainTime;
        fDotDamage = t.DotDamage;
        if (t.TowerType != (int)ConstructManager.ETowerType.Normal) // 근접타워
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
        nMovingEffectId = t.MovingEffectId;
        nExtinctionEffectId = t.ExtinctionEffectId;
        nlevel = t.Level;

        // 총알의 스탯  ------- 일단 투사체일때만
        nTowerType = t.TowerType;
        fSplashRange = t.SplashRange;
        fBouncePerDamage = t.BounceDDR;

        if(nTowerType == (int)ConstructManager.ETowerType.Bouncing)
        {
            nLife = t.BounceNum;
        }
        else if (nTowerType == (int)ConstructManager.ETowerType.AoE)
        {
            nLife = (int)t.AOSustainTime;
        }
        else if (nTowerType == (int)ConstructManager.ETowerType.Laser)
        {
            nLife = (int)t.RayLength;
        }
        else
        {
            nLife = 1;
        }
        fSpeed = t.BulletSpd;

        if (nAttackType == (int)ConstructManager.EAttackType.Laser)
        {
            HitPoint = this.transform.Find("HitPoint").gameObject;
            LaserBody = this.transform.Find("LaserBody").gameObject;
        }

        fAttack = SetDamage(t);     // 데미지는 타워에서 결정하는구나, 최종으로 결정 
        //lbDamage.text = fAttack.ToString();
        
    }

   

    private void FixedUpdate()
    {
        if(nTowerType == 6)
            DestroyBullet();

        if (Target != null)
        {
            if (nAttackType != (int)ConstructManager.EAttackType.Laser)
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

            if (Vector3.Dot(vDir, (vDestination - transform.position).normalized) < 0)
            {
                if (nTowerType != (int)ConstructManager.ETowerType.AoE && nTowerType != (int)ConstructManager.ETowerType.Laser)
                {
                    DestroyBullet();                                            // 장판 총알이 아니라면 바로 삭제시키고
                }
                else
                {
                    //isAttackAoE = true;
                    StartCoroutine("AoEAttack");                                // 장판 총알이면 장판데미지 코루틴 발동
                }
            }
        }
        else
        {
            //vDir = Vector3.Normalize(vDestination - transform.position);        // 방향을 계속 설정한다. 타깃이 죽은 후라도
            if(Vector3.Dot(vDir, vDestination - transform.position) < 0)
            {
                if (nTowerType != (int)ConstructManager.ETowerType.AoE)
                {
                    DestroyBullet();                                            // 장판 총알이 아니라면 바로 삭제시키고
                }
                else
                {
                    //isAttackAoE = true;
                    StartCoroutine("AoEAttack");                                // 장판 총알이면 장판데미지 코루틴 발동
                }
            }

            //if (Vector3.Distance(transform.position, vDestination) <= 0.05f)    // 타깃이 죽은 자리에 총알이 도달했을 때
            //{
            //    if (nTowerType != (int)ConstructManager.ETowerType.AoE)
            //    {
            //        DestroyBullet();                                            // 장판 총알이 아니라면 바로 삭제시키고
            //    }
            //    else
            //    {
            //        //isAttackAoE = true;
            //        StartCoroutine("AoEAttack");                                // 장판 총알이면 장판데미지 코루틴 발동
            //    }
            //}
        }

        if (nAttackType != (int)ConstructManager.EAttackType.Laser)
        {
            vDir.y = 0.0f;
            transform.position += fSpeed * vDir * GameManager.Instance.nGameSpeed * 0.3f;
            transform.rotation = Quaternion.LookRotation(vDir); // 좋구연
        }
        else
        {
            vDir.y = 0.0f;
            HitPoint.transform.position += fSpeed * vDir * 1.0f * GameManager.Instance.nGameSpeed;
            //LaserBody.transform.rotation = Quaternion.LookRotation(vDir);
            LaserBody.transform.localScale += new Vector3(0, 0, 1) * fSpeed * GameManager.Instance.nGameSpeed * 1.0f;
            LaserBody.transform.position += fSpeed * vDir * 0.5f * GameManager.Instance.nGameSpeed;
        }
    }

    private void Update()
    {
        // 장판데미지는 이곳에서 할수밖에 없을듯..
        //AoEBullet();   아닌듯
    }

    

    private void OnTriggerEnter(Collider other)  // 데미지 입히는 부분
    {
        if(other.tag == "Enemy")
        {
            Enemy e = other.GetComponent<Enemy>();

            if (nTargetId == e.Id)      // 적의 Id와 타깃의 아이디가 동일하다면
            {
                if (nAttackType != (int)ConstructManager.EAttackType.Splash && nAttackType != (int)ConstructManager.EAttackType.Bouncing && nAttackType != (int)ConstructManager.EAttackType.Laser)   // 스플래쉬 및 바운싱 타입이 아니면
                {
                    EnemyList.Add(e);
                    DestroyBullet();
                }
                else if (nAttackType == (int)ConstructManager.EAttackType.Bouncing) // 바운싱 타워라면
                {
                    EnemyList.Add(e);                                               // 데미지 리스트에 추가
                    nBouncingTargets.Add(e.Id);                                     // 바운싱 리스트에 추가
                    if (!FindBouncingTarget() || nLife <= nCurrBouncingNum)         // 타깃찾는 함수를 거쳐서 없으면 혹은 바운싱 최대 넘버가 되면 총알을 삭제시킨다.
                    {                                                               // 그리고 중요한게 가던 중에 타깃이 죽으면 안팅긴다!!! 그것이 바로 설계
                        DestroyBullet();
                    }
                }
                else if(nAttackType == (int)ConstructManager.EAttackType.Splash)
                { 
                    if (nTowerType != (int)ConstructManager.ETowerType.AoE)  // 장판타워가 아니라면
                    {
                        DestroyBullet();
                    }
                    else                  // 장판타워라면
                    {
                        //isAttackAoE = true;
                        StartCoroutine("AoEAttack");
                    }
                }
            }

            if (nAttackType == (int)ConstructManager.EAttackType.Laser)
            {
                LaserDamage(e);
            }
        }
    }
    public float SetDamage(Tower t)
    {
        float Damage = 0;
        switch (t.TowerType)
        {
            case (int)ConstructManager.ETowerType.Range:     // Range계열
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;
                break;

            case (int)ConstructManager.ETowerType.Splash:     // 스프라시 계열
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;
                break;

            case (int)ConstructManager.ETowerType.Bouncing:     // 바운싱
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage; // 초기 데미지만 세팅
                break;

            case (int)ConstructManager.ETowerType.Lucky:     // 럭키
                Damage = Random.Range(t.MinDamage + t.UpgradePerMinD * t.CurrUpgradeNum, t.MaxDamage + t.UpgradePerMaxD * t.CurrUpgradeNum);
                break;

            case (int)ConstructManager.ETowerType.Critical:     // 크리티커르 타워
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;

                if (isCritical)
                {
                    Damage *= t.CriticalMagni;
                }
                break;

            case (int)ConstructManager.ETowerType.Rapid:     // 래휘드
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;
                break;

            case (int)ConstructManager.ETowerType.Normal:     // 근접
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;
                break;

            case (int)ConstructManager.ETowerType.Laser:     // 레이져
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;
                break;

            case (int)ConstructManager.ETowerType.Dot:     // 독 뎀
                Damage = t.Attack + t.CurrUpgradeNum * t.UPDD;
                break;

            case (int)ConstructManager.ETowerType.AoE:     // 장판
                Damage = t.AODamage + t.UPDAO * t.CurrUpgradeNum;
                break;

            case (int)ConstructManager.ETowerType.Multi:    // 멀티샷
                Damage = t.Attack + t.CurrUpgradeNum * t.UpgradePerDamage;
                break;

            case (int)ConstructManager.ETowerType.Random:    // 란돔
                break;

            case (int)ConstructManager.ETowerType.Metamon:    // 메타몬
                break;
        }
        


        return Damage;
    }

    private void OnDestroy()
    {
        if (nAttackType != (int)ConstructManager.EAttackType.Laser) // 레이저 타워가 아니라면
        {
            for (int i = 0; i < EnemyList.Count; ++i)
            {
                EnemyList[i].Damage(this);   // 데미지는 타워에서 결정
                MakeExtinctionEffect();
            }
        }
        //else
        //{
        //    Debug.Log("바보");
        //}
    }

    public void FindSplashTargets()
    {
        EnemyList.Clear();

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

    public IEnumerator AoEAttack()
    {
        FindSplashTargets();
        for (int i = 0; i < EnemyList.Count; ++i)
        {
            EnemyList[i].Damage(this);   // 데미지는 타워에서 결정
            
        }
        nLife--;

        yield return new WaitForSeconds(1.0f);
        if (nLife > 0)
        {
            StartCoroutine("AoEAttack");
        }
        else
        {
            DestroyBullet();
        }
    }

    public void LaserDamage(Enemy e)
    {
        bool isOverapped = false;

        if (EnemyList.Count != 0)
        {
            for(int i = 0; i < EnemyList.Count; ++i)
            {
                if(EnemyList[i] == e)
                {
                    isOverapped = true;
                    break;
                }
            }
            if (!isOverapped)
            {
                e.Damage(this);
                EnemyList.Add(e);
            }
        }
        else
        {
            e.Damage(this);
            EnemyList.Add(e);
        }

    }

    public IEnumerator LaserAttack()
    {
        nLife -= 0.2f;
        yield return new WaitForSeconds(0.2f);
        if(nLife <= 0)
        {
            DestroyBullet();
        }
        else
        {
            StartCoroutine("LaserAttack");
        }
    }

    public float BounceDamage()
    {
        if(nCurrBouncingNum > 0)
        {
            fAttack *= fBouncePerDamage;
        }

        return fAttack;
    }

    public bool FindBouncingTarget()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 4.0f); // 4.0f(임시) 범위 내의 충돌된 녀석들을 찾는다.
        float fRealRange = float.MaxValue;                                 
        GameObject target = null;                                          // 일단 타깃은 Null
        bool isExist = false;                                              // 고로 존재하지 않는다고 해놓은 뒤
        
        foreach (Collider col in cols)
        {
            GameObject Temptarget = col.gameObject;
            if (Temptarget.CompareTag("Enemy"))                            // Enemy태그를 갖는 녀석들을 찾는다. 범위내에 있다면
            {
                Enemy e = Temptarget.GetComponent<Enemy>();                // 그녀석의 Enemy 스크립트를 찾고
                 
                bool isOverlap = false;                                    // 기존에 바운스된 타깃과 겹치는지 알아본다.

                for (int i = 0; i < nBouncingTargets.Count; ++i)
                {
                    if (e.Id == nBouncingTargets[i])
                    {
                        isOverlap = true;
                    }
                }

                if (isOverlap)                                             // 바운스리스트와 겹치면 넘어간다. 안넘어가면 새로운 타깃을 찾은것
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
        }
        if (isExist)                                                    // 타깃이 존재한다면
        {
            EnemyList[0].Damage(this);                                  // 데미지를 주고\
            EnemyList.Clear();
            Target = target;                                            // 새로운 타깃을 설정
            nTargetId = target.GetComponent<Enemy>().Id;                // 새로운 타깃 아이디도 저장
            nCurrBouncingNum++;
        }
        return isExist;
    }

    public void DestroyBullet()
    {
        //if (nAttackType == (int)ConstructManager.EAttackType.Laser) // SPlash
        //{
        //    Debug.Log("q바보");
        //}
        if (nAttackType == (int)ConstructManager.EAttackType.Splash) // SPlash
        {
            FindSplashTargets();
        }
        GameObject Bullet = transform.parent.gameObject;
        Bullet.SetActive(false);
        DestroyMovingEffect();
        
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        if (GameManager.Instance.nCurrentScene == 2)
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

    public void DestroyMovingEffect()
    {
        if (nMovingEffectId != -1)
        {
            MovingEffect.DestroyEffect();
        }
    }

    public void MakeExtinctionEffect()
    {
        if(nExtinctionEffectId != -1)
        {
            if (Target != null)
            {
                EffectManager.instance.MakeEffect(EffectManager.Instance.EffectInfolist[nExtinctionEffectId], Target.transform.position);
            }
            else
            {
                EffectManager.instance.MakeEffect(EffectManager.Instance.EffectInfolist[nExtinctionEffectId], transform.position);
            }
            //EffectManager.instance.MakeEffect(EffectManager.Instance.EffectInfolist[nExtinctionEffectId], transform.parent);
        }
    }
}


