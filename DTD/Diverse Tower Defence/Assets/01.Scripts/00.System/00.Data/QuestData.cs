using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData
{
    public int MostKillCountTower;
    public int fCurrAllKillCount;
    public int CurrHp;

    //public TypeOfTower TOTLevelZero;        // 인덱스별 타워 수량.. 
    //public TypeOfTower TOTLevelOne;
    //public TypeOfTower TOTLevelTwo;
    //public TypeOfTower TOTLevelThree;
    //public TypeOfTower TOTLevelFour;


    
    


    public List<TypeOfTower> ToTLevelNumList = new List<TypeOfTower>();


    public struct TypeOfTower
    {
        public int[] TypeOfTowerLevel;
        public BitArray BitArrayTypeOfTowerLevel;

    }
    public QuestData()
    {
        MostKillCountTower = 0;
        fCurrAllKillCount = 0;
        CurrHp = 100;

        for (int i = 0; i < 5; i++)
        {
            TypeOfTower a = new TypeOfTower
            {
                TypeOfTowerLevel = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 }
            };
            ToTLevelNumList.Add(a);
        }
    }

    public void RenewTypeOfTowerPlus(int level, int IndexOfDeckList)
    {
        ToTLevelNumList[level].TypeOfTowerLevel[IndexOfDeckList]++;
        //RenewBitArray(level, IndexOfDeckList);
    }

    public void RenewTypeOfTowerMinus(int level, int IndexOfDeckList)
    {
        ToTLevelNumList[level].TypeOfTowerLevel[IndexOfDeckList]--;
        //RenewBitArray(level, IndexOfDeckList);
    }



    public int CheckNumberOfTower(ConstructManager.ETowerType type)
    {
        int num = 0;

        return num;
    }

    public bool CheckExistAllKindOfTower(int level)
    {
        bool isExist = true;
        for(int i = 0; i < 8; ++i)
        {
            if(ToTLevelNumList[level].TypeOfTowerLevel[i] <= 0)
            {
                isExist = false;
                break;
            }
        }
        return isExist;
    }

    public int CheckNumberOfTower(int level)
    {
        int num = 0;

        return num;
    }

    public int CheckNumberOfTower(ConstructManager.ETowerType type, int level)
    {
        int num = 0;

        return num;
    }

    public int CheckNumberOfUpgrades()
    {
        int num = 0;

        return num;
    }

    public int CheckNumberOfTowerInBolck(int BlockIndex)
    {
        int num = 0;

        return num;
    }

    public void InitQuest()     // 퀘스트 초기화
    {

    }


    public void DecreaseHp()
    {
        CurrHp--;
    }
}
