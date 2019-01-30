using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public class EffectManager : MonoBehaviour {


    //public List<ParticleSystem> ParticleList { set; get; }

    // 시작할때 불러올 파티클들
    public List<GameObject> BulletParticleList = new List<GameObject>();
    public List<GameObject> UIParticleList = new List<GameObject>();
    public List<GameObject> TestParticleList = new List<GameObject>();

    //public Effect OriginEffect;
    public List<Effect> EffectInfolist = new List<Effect>();            // 이펙트들이 저장된 리스트
    public List<GameObject> EffectPool = new List<GameObject>();        // 실제로 사용할 이펙트 풀
    public int nEffectNum = 4;

    public Transform trStartPos;
    public Transform trLoadedEffectpos;
    private static object _Lock = new object();
    public static EffectManager instance = null;
    public static EffectManager Instance
    {
        get
        {
            lock (_Lock)

            {
                if (instance == null)
                {
                    instance = GameObject.Find("Manager").AddComponent<EffectManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }

    public void InitManager()
    {
        LoadParticleEffect();
    }
    public void InitEffectPool()
    {
        InitPool();
        LoadEffectInfo();
    }

    public void InitPool()
    {
        //trStartPos = GameObject.Find("BulletPool").transform;
        //GameObject gSrcBullet = new GameObject();
        //gSrcBullet.SetActive(false);
        //gSrcBullet.transform.SetParent(trStartPos);
        //bulletPool.Add(gSrcBullet);
        trStartPos = GameObject.Find("EffectPool").transform;
        trLoadedEffectpos = GameObject.Find("LoadedEffect").transform;
        GameObject gSrcEffect = new GameObject();
        gSrcEffect.AddComponent<Effect>();
        //gSrcEffect.AddComponent<ParticleSystem>();
        gSrcEffect.SetActive(false);
        gSrcEffect.transform.SetParent(trStartPos);
        EffectPool.Add(gSrcEffect);
        //EffectPool[0].SetActive(false);
    }
    public void LoadEffectInfo()
    {
        string LoadString = File.ReadAllText(Application.persistentDataPath + "/EffectInfo.json");
        JObject LoadData = JObject.Parse(LoadString);
        GameObject g = Resources.Load<GameObject>("Effects/Effect");
        //g.transform.SetParent(trLoadedEffectpos);
        //g.AddComponent<Effect>();
        //Effect e = g.GetComponent<Effect>();
        for (int i = 0; i < nEffectNum; ++i)
        {
            GameObject gg = Instantiate(g);
            gg.transform.SetParent(trLoadedEffectpos);
            Effect ef = gg.GetComponent<Effect>();
            ef.name = i.ToString();
            ef.Id = (int)LoadData["info"][i]["id"];
            ef.ParticleId = (int)LoadData["info"][i]["particleid"];
            ef.fLife = (float)LoadData["info"][i]["life"];
            ef.nLifeType = (int)LoadData["info"][i]["lifetype"];   
            EffectInfolist.Add(ef);
        }
        //Destroy(g);
        g.SetActive(false);
    }

    public void LoadParticleEffect()
    {
        for(int i = 0; i < 4; ++i)
        {
            //GameObject g;
            //g.AddComponent<ParticleSystem>(
            GameObject p = Resources.Load<GameObject>(Define.TestEffectsAddress + i.ToString());
            TestParticleList.Add(p);
        }
    }

    public void MakeEffect(Effect e, Vector3 pos)
    {
        bool isActiveOn = false;
        for(int i = 0; i< EffectPool.Count; ++i)
        {
            if(EffectPool[i].activeSelf == true)
            {
                continue;
            }
            else
            {
                EffectPool[i].SetActive(true);
                Effect effect = EffectPool[i].GetComponent<Effect>();
                effect.InitEffect(e, pos, i);
                isActiveOn = true;
                break;
            }
        }

        if(!isActiveOn)
        {
            GameObject g = new GameObject();
            g.transform.SetParent(trStartPos);
            g.AddComponent<Effect>();
            //g.AddComponent<ParticleSystem>();
            EffectPool.Add(g);
            Effect effect = g.GetComponent<Effect>();
            effect.InitEffect(e, pos, EffectPool.Count - 1);
        }
    }

    
}
