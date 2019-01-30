using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public int Id;
    public int ParticleId;
    public float fLife;
    public int nLifeType;
    //public ParticleSystem ps = new ParticleSystem();
    public Transform Tr;
    public GameObject ParticleObject;
    public void InitEffect(Effect e /*int id, int ptcId, float life, int lifeType*/, Vector3 pos, int id)
    {
        Tr = this.GetComponent<Transform>();
        Id = id;
        ParticleId = e.ParticleId;
        fLife = e.fLife;
        nLifeType = e.nLifeType;
        ParticleObject = Instantiate(EffectManager.Instance.TestParticleList[ParticleId], Tr);
        ParticleObject.transform.position = pos;
        if(nLifeType != -1)
        {
            StartCoroutine("Run");
        }
    }
    
    public void DestroyEffect()
    {
        //ps = null;
        Destroy(ParticleObject);
        EffectManager.Instance.EffectPool[Id].SetActive(false);
    }
    
    IEnumerator Run()
    {
        while(fLife > 0.0f)
        {
            fLife -= 0.2f;
            yield return new WaitForSeconds(0.2f);
        }

        DestroyEffect();

        yield return null;
    }
}
