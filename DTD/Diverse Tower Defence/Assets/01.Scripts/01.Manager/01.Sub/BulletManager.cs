using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletStauts
{
    int Id;
    GameObject Target;
    Transform transform;
    Animation anim;
    int TowerId;
    int EffectId;
    bool DoDamaged;    // 피해를 주었는가?

    // 총알의 스탯
    float Attack;
    float AttackType;
    float SplashRange;
    int BounceTargetFirst;
    int BounceTargetSecond;
    int BounceTargetThird;
    bool IsCritical;
}



public class BulletManager : MonoBehaviour
{
    public Transform trStartPos;    // 총알 Parent Pos
    public Transform trLabelPos;
    public List<GameObject> bulletPool = new List<GameObject>();
    public List<GameObject> bulletModelList = new List<GameObject>();
    public List<GameObject> DamageLabelPool;
    public UILabel lbDamage;

    private static object _Lock = new object();
    public static BulletManager instance = null;
    public static BulletManager Instance
    {
        get
        {
            lock (_Lock)

            {
                if (instance == null)
                {
                    instance = GameObject.Find("Manager").AddComponent<BulletManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }
    private void Start()
    {
        lbDamage = Resources.Load<UILabel>("Damage");
    }

    public void MakeLabel()
    {
       
    }


    public void MakeBullet(int towerIndex, int TargetListIndex, bool isCri)
    {
        int n = PoolManager.Instance.GetPoolObject(bulletPool.Count, bulletPool); 

        if (n == -1)        // 모든 총알이 활성화 상태일때
        {
            int nSize = bulletPool.Count;
            GameObject gSrcBullet = new GameObject();
            gSrcBullet.transform.SetParent(trStartPos);
            gSrcBullet.SetActive(false);
            PoolManager.Instance.AddObjectPool(ref bulletPool, gSrcBullet);

            GameObject bullet = (GameObject)Instantiate(bulletModelList[0], bulletPool[nSize].transform);   // 풀이 하나 늘어났기 때문에 아까의 nSize는 지금의 마지막 인덱스 번호가 된다.
            bullet.AddComponent<Bullet>();
            Bullet b = bullet.GetComponent<Bullet>();
            b.SetBullet(towerIndex, nSize, TargetListIndex, isCri);
            bulletPool[nSize].SetActive(true);
        }
        else                // 활성화 안된 총알을 발견했을 때    
        {
            GameObject bullet = (GameObject)Instantiate(bulletModelList[0], bulletPool[n].transform);
            bullet.AddComponent<Bullet>();
            Bullet b = bullet.GetComponent<Bullet>();
            b.SetBullet(towerIndex, n, TargetListIndex, isCri);
            bulletPool[n].SetActive(true);
        }

        //for (int i = 0; i < nPoolNum; ++i)
        //{
        //    if(!bulletPool[i].activeSelf) // 활성화 안된 총알을 찾으면 새로운 총알로 세팅해준 후 활성화 시킨다
        //    {
        //        GameObject bullet = (GameObject)Instantiate(bulletModelList[0], bulletPool[i].transform);
        //        bullet.AddComponent<Bullet>();
        //        Bullet b = bullet.GetComponent<Bullet>();
        //        b.SetBullet(towerIndex, i);
        //        bulletPool[i].SetActive(true);
        //        break;
        //    }
        //    else if(i == nPoolNum -1)                          // 만약 모든 총알이 활성화 되었다면 
        //    {
        //        GameObject gSrcBullet = new GameObject();        // 사용할 적군 오브젝트
        //        gSrcBullet.SetActive(false);
        //        gSrcBullet.transform.SetParent(trStartPos);      // 풀 갯수를 한개 늘리고
        //        bulletPool.Add(gSrcBullet);
        //        nPoolNum = bulletPool.Count; 
        //    }
        //    else
        //    {
        //        continue;
        //    }
        //    //gSrcEnemy.SetActive(false);
        //    //gSrcEnemy.transform.SetParent(trStartPos);
        //    //EnemyPool.Add(gSrcEnemy);
        //}
        //nPoolNum = bulletPool.Count;
    }
    public void MakeBulletPool()
    {
        trStartPos = GameObject.Find("BulletPool").transform;
        GameObject gSrcBullet = new GameObject();
        gSrcBullet.SetActive(false);
        gSrcBullet.transform.SetParent(trStartPos);
        bulletPool.Add(gSrcBullet);
    }


    public void LoadModelList()
    {
        GameObject go = new GameObject();

        for (int i = 0; i < 1; ++i)
        {
            go = Resources.Load<GameObject>("Bullet");
            bulletModelList.Add(go);
        }
    }
}
