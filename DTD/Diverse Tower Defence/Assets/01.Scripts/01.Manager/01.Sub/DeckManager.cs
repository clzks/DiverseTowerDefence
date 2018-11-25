using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<int> PossesTowerDeckList = new List<int>();                 // 보유한 모든 덱 리스트
    public List<int> SelectedTowerList = new List<int>();                  // 선택된 타워들 리스트
    public List<Sprite> DeckSpriteList = new List<Sprite>();               // 타워들 텍스쳐

    public int nCurrPage = 1;
    public int nTotalPageNum;
    public int nCheckNum;

    

    private static object _Lock = new object();
    public static DeckManager instance = null;
    public static DeckManager Instance
    {
        get
        {
            lock (_Lock)
            {
                if (instance == null)
                {
                    UserDataManager.Instance.LoadDeckList();
                    instance = GameObject.Find("Manager").AddComponent<DeckManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }

    public void Init()
    {
        PossesTowerDeckList = UserDataManager.Instance.PossesTowerList;

        nTotalPageNum = (PossesTowerDeckList.Count / 8) + (PossesTowerDeckList.Count % 8 != 0 ? 1 : 0); //

        nCurrPage = 0;

        nCheckNum = 0;

    }

    private void Start()
    {
        
    }

    //private void Update()
    //{
    //    if(GameManager.Instance.nCurrentScene == 3)
    //    {
    //
    //    }
    //}
   
}
