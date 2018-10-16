using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.IO;

public class DeckSetting : MonoBehaviour
{
    public UILabel CurrPage;
    public UILabel TotalPage;

    public List<UISprite> DeckSpriteList = new List<UISprite>();        // 보유한 모든 타워들의 스프라이트 
    public List<UILabel> DeckTowerNameList = new List<UILabel>();       // 보유한 모든 타워들의 라벨

    public List<UIPanel> DeckList = new List<UIPanel>();                // 덱 메이킹의 덱들
    public List<UIButton> PlayerDeckList = new List<UIButton>();        // 오른쪽에 나와있는 덱 선택용 즉, 임시적으로 선택된 덱들의 리스트라 할 수 있음
    public List<int> TempDeckList = new List<int>();

    public UIPanel DeckInfo;
    /* 
    ============================ 덱 선택 ===================================
    - 덱 매니저의 SelectedDeckList가 갱신된다. (List의 최대 크기는 8)
    - SelectedDeckList를 갱샌 시킬때마다 PlayerDeckList도 같이 갱신된다. 
      (SDL의 크기에 따라 PDL의 Active와 라벨, 그림 업데이트 시킴)
    ========================================================================
    */
    public void Start()
    {
        CurrPage.text = (DeckManager.Instance.nCurrPage + 1).ToString();
        TotalPage.text = DeckManager.Instance.nTotalPageNum.ToString();
        TempDeckList = UserDataManager.Instance.TowerDeckList;
        PageTextUpdate();
        CurrDeckUpdate();
        DeckInfo = GameObject.Find("UI Root/Camera/DeckInfo").GetComponent<UIPanel>();
    }

    //public void SetTowerDecks(List<int> deckIndexList)
    //{
    //    UserDataManager.Instance.TowerDeckList.Clear();
    //    foreach (var item in deckIndexList)
    //    {
    //        UserDataManager.Instance.TowerDeckList.Add(item);
    //    }
    //}

    public void ClickGameStart()
    {
        if (TempDeckList.Count == 8) // TempDeckList가 8개로 가득차야 시작이 된다.
        {
            UserDataManager.Instance.TowerDeckList = TempDeckList;
            GameManager.Instance.nCurrentScene = 1;
            SceneManager.LoadScene("Test");
            //UserDataManager.Instance
            //SetTowerDecks(DeckManager.Instance.SelectedTowerList);
        }
        else
        {
            // 나중에 게임을 시작할 수 없습니다 라고 나올거 만들기
        }
    }

    public void ClickNextPage()
    {
        DeckManager.Instance.nCurrPage++;
        PageTextUpdate();
    }

    public void ClickPrevPage()
    {
        DeckManager.Instance.nCurrPage--;
        PageTextUpdate();
    }
    
    private void PageTextUpdate()
    {
        if(DeckManager.Instance.nCurrPage < 0)  
        {
            DeckManager.Instance.nCurrPage = DeckManager.Instance.nTotalPageNum - 1;
        }
        DeckManager.Instance.nCurrPage = DeckManager.Instance.nCurrPage % DeckManager.Instance.nTotalPageNum;

        CurrPage.text = (DeckManager.Instance.nCurrPage + 1).ToString();
        TotalPage.text = DeckManager.Instance.nTotalPageNum.ToString();
        
        DeckNameSetting();
    }

    public void ClickDeckMaking() // 보유 목록 타워들 중 하나를 선택한것
    {
        int SelectedDMIndex = Convert.ToInt32(UIButton.current.transform.parent.name);
        int nCurrP = DeckManager.Instance.nCurrPage;

        int TowerIndex = DeckManager.Instance.PossesTowerDeckList[nCurrP * 8 + SelectedDMIndex]; // 타워 자체의 인덱스


        if(TempDeckList.Count != 0)
        {
            if(TempDeckList.Count < 8 && !TempDeckList.Contains(TowerIndex))
            {
                TempDeckList.Add(TowerIndex);
                TempDeckList.Sort();
            }
        }
        else
        {
            TempDeckList.Add(TowerIndex);
        }
        
        CurrDeckUpdate();
    }

    public void ClickPlayerDeckList() // 현재 선택된 타워들 목록중에서 하나를 선택한것 
    {
        int SelectedPDLIndex = Convert.ToInt32(UIButton.current.name); // TempDeckList 내의 인덱스
        TempDeckList.RemoveAt(SelectedPDLIndex);
        CurrDeckUpdate();
    }   

    public void ClickDeckInfo()
    {
        int SelectedDMIndex = Convert.ToInt32(UIButton.current.transform.parent.name);
        int nCurrP = DeckManager.Instance.nCurrPage;

        int TowerIndex = DeckManager.Instance.PossesTowerDeckList[nCurrP * 8 + SelectedDMIndex]; // 타워 자체의 인덱스

        SetDeckInfo(TowerIndex);
        DeckInfo.gameObject.SetActive(true);
    }

    public void ClickQuitDeckInfo()
    {
        DeckInfo.gameObject.SetActive(false);
    }

    private void SetDeckInfo(int towerIndex)
    {
        UILabel Explanation = GameObject.Find("UI Root/Camera/DeckInfo/Explanation").GetComponent<UILabel>();
        UILabel Type = GameObject.Find("UI Root/Camera/DeckInfo/Type").GetComponent<UILabel>();
        UILabel TowerName = GameObject.Find("UI Root/Camera/DeckInfo/TowerName").GetComponent<UILabel>();

        TowerName.text = ConstructManager.Instance.towerTypeList[towerIndex].TowerName;
        Type.text = ConstructManager.Instance.towerTypeList[towerIndex].TypeIndex.ToString();
        Explanation.text = "";
        
    }

    private void CurrDeckUpdate()
    {
        for (int i = 0; i < 8; ++i)
        {
            if (TempDeckList.Count <= i)
            {
                PlayerDeckList[i].gameObject.SetActive(false);
            }
            else // 아직 미완
            {
                PlayerDeckList[i].gameObject.SetActive(true);
                UISprite sprite = PlayerDeckList[i].GetComponentInChildren<UISprite>();
                UILabel label = PlayerDeckList[i].GetComponentInChildren<UILabel>();
                label.text = ConstructManager.Instance.towerTypeList[TempDeckList[i]].TowerName;
                //DeckTowerNameList[i].text = ConstructManager.Instance.towerTypeList[v[i]].TowerName;
            }
        }
    }

    private void DeckNameSetting()
    {

        int nCurrP = DeckManager.Instance.nCurrPage;

        var v = DeckManager.Instance.PossesTowerDeckList.Skip(nCurrP * 8).Take(8).ToList();

        for (int i = 0; i < 8; ++i)
        {
            if(v.Count <= i)
            {
                DeckList[i].gameObject.SetActive(false);
            }
            else
            {
                DeckList[i].gameObject.SetActive(true);
                DeckTowerNameList[i].text = ConstructManager.Instance.towerTypeList[v[i]].TowerName;
            }
        }
    }
}
