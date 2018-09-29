using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class UserData : MonoBehaviour
{
    private int Ruby;
    private List<int> TowerDeckList = new List<int>();
    private List<int> PossesTowerList = new List<int>();

    private int PlayNum;
    private int HighestStage;
    private int ClearCount;

    private void Start()
    {
        InitUserData();
    }

    public void InitUserData()
    {
        Ruby = 0;

        TowerDeckList.Add(0);
        TowerDeckList.Add(5);
        TowerDeckList.Add(10);
        TowerDeckList.Add(14);
        TowerDeckList.Add(17);
        TowerDeckList.Add(20);
        TowerDeckList.Add(23);
        TowerDeckList.Add(25);

        PossesTowerList.Add(0);
        PossesTowerList.Add(5);
        PossesTowerList.Add(10);
        PossesTowerList.Add(14);
        PossesTowerList.Add(17);
        PossesTowerList.Add(20);
        PossesTowerList.Add(23);
        PossesTowerList.Add(25);

        PlayNum = 0;
        HighestStage = 0;
        ClearCount = 0;

        JObject jSaveData = new JObject();
        jSaveData["Ruby"] = Ruby;
        //jSaveData["TowerDeckList"] = TowerDeckList;
        //jSaveData["PossesTowerList"] = PossesTowerList;
        jSaveData["PlayNum"] = PlayNum;
        jSaveData["HighestStage"] = HighestStage;
        jSaveData["ClearCount"] = ClearCount;

        JArray jTowerDeckListData = new JArray();
        for(int i = 0; i < TowerDeckList.Count; ++i)
        {
            jTowerDeckListData.Add(TowerDeckList[i]);
        }
        JArray jPossesDeckListData = new JArray();
        for (int i = 0; i < PossesTowerList.Count; ++i)
        {
            jPossesDeckListData.Add(TowerDeckList[i]);
        }
        jSaveData["TowerDeckList"] = jTowerDeckListData;
        jSaveData["PossesTowerList"] = jPossesDeckListData;
        string SaveString = JsonConvert.SerializeObject(jSaveData, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/SaveData.json", SaveString);
    }


    public void Save()
    {
        JObject jSaveData = new JObject();
        jSaveData["Ruby"] = Ruby;
        //jSaveData["TowerDeckList"] = TowerDeckList;
        //jSaveData["PossesTowerList"] = PossesTowerList;
        jSaveData["PlayNum"] = PlayNum;
        jSaveData["HighestStage"] = HighestStage;
        jSaveData["ClearCount"] = ClearCount;
        JArray jTowerDeckListData = new JArray();
        for (int i = 0; i < TowerDeckList.Count; ++i)
        {
            jTowerDeckListData.Add(TowerDeckList[i]);
        }
        JArray jPossesDeckListData = new JArray();
        for (int i = 0; i < PossesTowerList.Count; ++i)
        {
            jPossesDeckListData.Add(TowerDeckList[i]);
        }
        jSaveData["TowerDeckList"] = jTowerDeckListData;
        jSaveData["PossesTowerList"] = jPossesDeckListData;
        string SaveString = JsonConvert.SerializeObject(jSaveData, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/SaveData.json", SaveString);
    }
}
