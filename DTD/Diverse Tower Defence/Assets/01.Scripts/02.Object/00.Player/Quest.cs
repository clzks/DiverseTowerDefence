using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public enum EQuestType
    {
        Global,
        Normal,
        OutBraek,
        Daily,
        Etc
    }

    public int Id;
    public int TrailingQuestIndex;
    public EQuestType Type;
    public QuestManager.EMissionType eDetailQuestType;
    public string Text;
    public int nGoalCount;                       // 필요한 퀘스트만
    public int nConstructLevel;
    public Reward.ERewardType eRewardtype;
    public int nRewardValue;
    //public bool isCompleted;                    // 퀘스트 완료
    public float fCooltime;

    public Quest(int id, int trailingquestindex,int type, int mission, string text, 
        int goalcount, int constructlevel, int rewardtype, int rewardvalue, float cooltime)
    {
        Id = id;
        TrailingQuestIndex = trailingquestindex;
        Type = (EQuestType)type;
        eDetailQuestType = (QuestManager.EMissionType)mission;
        Text = text;
        nGoalCount = goalcount;
        nConstructLevel = constructlevel;
        eRewardtype = (Reward.ERewardType)rewardtype;
        nRewardValue = rewardvalue;
        fCooltime = cooltime;
    }

    public int GetProgressCount()
    {
        int CurrValue = 0;
        switch(eDetailQuestType)
        {
            case (QuestManager.EMissionType)0:
                CurrValue = QuestManager.Instance.GetCurrAllKillCount();
                break;

            case (QuestManager.EMissionType)1:
                CurrValue = QuestManager.Instance.GetMostKillCountTower();
                break;

            case (QuestManager.EMissionType)2:
                CurrValue = QuestManager.Instance.GetConstructArray(nConstructLevel);
                break;

            case (QuestManager.EMissionType)3:

                break;

            case (QuestManager.EMissionType)4:

                break;

            case (QuestManager.EMissionType)5:

                break;
        }

        return CurrValue;
    }
}
