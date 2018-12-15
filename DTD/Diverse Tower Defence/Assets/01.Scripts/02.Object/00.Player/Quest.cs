using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public enum EQuestType
    {
        Global,
        Normal,
        OutBraek,
        Daily,
        Etc
    }

    public QuestManager.EGlobalQuestInfoType DetailQuestType;
    public EQuestType Type;
    public int Id;
    public string text;
    public Reward reward;
    public bool isCompleted;                    // 퀘스트 완료
    public float fCooltime;
    public int nMergeNum;                       // 필요한 퀘스트만

}
