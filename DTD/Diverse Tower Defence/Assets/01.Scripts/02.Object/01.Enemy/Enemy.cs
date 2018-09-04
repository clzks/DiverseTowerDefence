﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[SelectionBase]
public class Enemy : MonoBehaviour
{
    //delegate T Mydelegate<T>(T a, T b);

    public int Id;
    public int Round;
    public float Curr_HP;
    public float Speed;
    public int CurrNode;
    public int RemainDotTime;
    public float CurrDotDamage;
    public float Max_HP;

    public UISlider ProgressBar;        // 체력바
    public Vector2 ProgressBar_Size;
    

    public void Start()
    {
        GameObject Go = NGUITools.AddChild(EnemyManager.Instance.BarParent.gameObject, EnemyManager.Instance.ProgressBar);
        ProgressBar = Go.GetComponent<UISlider>();
        ProgressBar_Size.x = Go.GetComponentInChildren<UISprite>().width ;
        ProgressBar_Size.y = Go.GetComponentInChildren<UISprite>().height;
    }

    public void SetEnemy(EnemyStatus es, int id)
    {

        Id = id;
        Round = es.nRound;
        Max_HP = es.fHP;
        Curr_HP = es.fHP;
        Speed = es.fSpeed;
        CurrNode = es.nCurrNode;
        RemainDotTime = es.nRemainDotTime;
        CurrDotDamage = es.fCurrDotDamage;
        
    }

    public void Damage(float f) // 기본피해
    {
        Curr_HP -= f;
        Debug.Log("입은 피해 : " + f.ToString()); 
    }

    public void Damage(Bullet b)
    {
        switch (b.nTowerType)
        {
            case (int)ConstructManager.ETowerType.Range: 
                Curr_HP -= b.fAttack;
                Debug.Log("입은 피해 : " + b.fAttack.ToString());
            break;

            case (int)ConstructManager.ETowerType.Splash:
                Curr_HP -= b.fAttack;
                Debug.Log("입은 피해 : " + b.fAttack.ToString());
                break;

            case (int)ConstructManager.ETowerType.Bouncing:
                float Atk = b.BounceDamage();
                Curr_HP -= Atk;
                Debug.Log("입은 피해 : " + Atk.ToString());
                break;

            case (int)ConstructManager.ETowerType.Lucky:             // 렄키
                Curr_HP -= b.fAttack;
                Debug.Log("입은 피해 : " + b.fAttack.ToString());
                break;

            case (int)ConstructManager.ETowerType.Critical:             // 크리티컬
                Curr_HP -= b.fAttack;
                Debug.Log("입은 피해 : " + b.fAttack.ToString());
                break;

            case (int)ConstructManager.ETowerType.Rapid:             // 래피드
                Curr_HP -= b.fAttack;
                Debug.Log("입은 피해 : " + b.fAttack.ToString());
                break;

            case (int)ConstructManager.ETowerType.Normal:             // 노멀
                Curr_HP -= b.fAttack;
                Debug.Log("입은 피해 : " + b.fAttack.ToString());
                break;

            case (int)ConstructManager.ETowerType.Laser:             // 레이저
                Curr_HP -= b.fAttack;
                Debug.Log("입은 피해 : " + b.fAttack.ToString());
                break;

            case (int)ConstructManager.ETowerType.Dot:             // 도트뎀
                //Curr_HP -= b.fAttack;
                //Debug.Log("입은 피해 : " + b.fAttack.ToString());
                if(RemainDotTime == 0)
                {
                    RemainDotTime = b.nDotTime;
                    StartCoroutine(DotAttack(b));
                }
                else
                {
                    RemainDotTime = b.nDotTime;
                }
                break;

            case (int)ConstructManager.ETowerType.AoE:             // 광역 아직 안됐네;;
                Curr_HP -= b.fAttack;
                Debug.Log("입은 피해 : " + b.fAttack.ToString());
                break;

            case (int)ConstructManager.ETowerType.Multi:             // 멀티샷
                Curr_HP -= b.fAttack;
                Debug.Log("입은 피해 : " + b.fAttack.ToString());
                break;

            case (int)ConstructManager.ETowerType.Random:             // 랜덤
                Curr_HP -= b.fAttack;
                Debug.Log("입은 피해 : " + b.fAttack.ToString());
                break;

            case (int)ConstructManager.ETowerType.Metamon:             // 메타몬
                Curr_HP -= b.fAttack;
                Debug.Log("입은 피해 : " + b.fAttack.ToString());
                break;
        }
    }

    public IEnumerator DotAttack(Bullet b)
    {
        if(RemainDotTime > 0)
        {
            Curr_HP -= b.fDotDamage;
            Debug.Log("입은 피해 : " + b.fAttack.ToString());
            RemainDotTime--;
        }
        yield return new WaitForSeconds(1.0f);
        if(RemainDotTime > 0)
        {
            StartCoroutine(DotAttack(b));
        }
    }

    public void FixedUpdate()
    {
        
        if (GameManager.Instance.nCurrentScene == 2)
        {
            Vector3 ScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);
            ScreenPos.z = 0.0f;
            ScreenPos.x -= ProgressBar_Size.x * 0.5f;
            ScreenPos.y += ProgressBar_Size.y * 0.5f;
            Vector3 viewPos = Camera.main.ScreenToViewportPoint(ScreenPos);
            viewPos.z = 0.0f;
            Vector3 uiScreenPos = UICamera.currentCamera.ScreenToWorldPoint(ScreenPos);
           //uiScreenPos.x -= 0.22f;
           //uiScreenPos.y += 0.13f;
           
            ProgressBar.transform.position = uiScreenPos;
        }
        else if(GameManager.Instance.nCurrentScene == 1)
        {
            Vector3 ScreenPos = Camera.main.WorldToScreenPoint(this.transform.parent.position);
            ScreenPos.z = 0.0f;
            ScreenPos.x -= ProgressBar_Size.x * 0.5f;
            ScreenPos.y += ProgressBar_Size.y * 0.5f;
            Vector3 viewPos = Camera.main.ScreenToViewportPoint(ScreenPos);
            viewPos.z = 0.0f;
            Vector3 uiScreenPos = UICamera.currentCamera.ScreenToWorldPoint(ScreenPos);
            uiScreenPos.x -= 0.22f;
            uiScreenPos.y += 0.13f;

            ProgressBar.transform.position = uiScreenPos;
        }

        ProgressBar.value = Curr_HP / Max_HP;

        if (Curr_HP <= 0)
        {
            if (GameManager.Instance.nCurrentScene == 1)
            {
                Destroy(EnemyManager.Instance.EnemyModelPool[Id].gameObject);
                EnemyManager.Instance.EnemyPool[Id].SetActive(false);
            }
            else if(GameManager.Instance.nCurrentScene == 2)
            {
                Destroy(this.gameObject);
                Destroy(ProgressBar.gameObject);
            }
        }
    }

    public void StepOnGoalLine()
    {
        StageManager.Instance.nLife -= 1;
        GameObject Go = GetComponent<GameObject>();
        Destroy(this.gameObject);
        Go.SetActive(false);
    }
}