using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{


    private int nCurrMaxQuestIndex = 0;           // 현재 진행된 퀘스트 
    //private float fFirstQuestCooltime;          // 첫번째 퀘스트 쿨타임
    //private float fSecondQuestCooltime;         // 두번째 퀘스트 쿨타임
    //private float fThirdQuestCooltime;          // 세번째 퀘스트 쿨타임
    private List<float> QuestCooltimeList;        // 퀘스트들의 쿨타임을 관리할 List 
    private List<bool> QuestInProgressList;       // 퀘스트가 진행중인지의 여부
    private List<bool> ReadyToQuestList;          // 퀘스트를 진행할 수 있는지의 여부

    private static object _Lock = new object();
    public static QuestManager instance = null;
    public static QuestManager Instance
    {
        get
        {
            lock (_Lock)

            {
                if (instance == null)
                {
                    instance = GameObject.Find("Manager").AddComponent<QuestManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }

    public void InitQuest()     // 퀘스트 초기화
    {
        QuestCooltimeList.Add(0.0f);        // 퀘스트 3개 전부 쿨타임 0
        QuestCooltimeList.Add(0.0f);        // 퀘스트 3개 전부 쿨타임 0
        QuestCooltimeList.Add(0.0f);        // 퀘스트 3개 전부 쿨타임 0
        QuestInProgressList.Add(false);     // 퀘스트 3개 전부 진행중 아님
        QuestInProgressList.Add(false);     // 퀘스트 3개 전부 진행중 아님
        QuestInProgressList.Add(false);     // 퀘스트 3개 전부 진행중 아님
        ReadyToQuestList.Add(true);         
        ReadyToQuestList.Add(false);        
        ReadyToQuestList.Add(false);        // 퀘스트는 맨 첫번째 것만 일단 진행 할 수 있다.
    }
}
