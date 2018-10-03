using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class UserDataManager : MonoBehaviour
{

    public int Ruby;
    public List<int> TowerDeckList = new List<int>();
    public List<int> PossesTowerList = new List<int>();

    public int PlayNum;
    public int HighestStage;
    public int ClearCount;

    private static object _Lock = new object();
    public static UserDataManager instance = null;
    public static UserDataManager Instance
    {
        get
        {
            lock (_Lock)

            {
                if (instance == null)
                {
                    instance = GameObject.Find("Manager").AddComponent<UserDataManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }


    private void Start()
    {
        InitUserData();
    }

    public void InitUserData()
    {
        Ruby = 0;
        TowerDeckList.Clear();
        PossesTowerList.Clear();

        TowerDeckList.Add(0);
        TowerDeckList.Add(5);
        TowerDeckList.Add(10);
        TowerDeckList.Add(14);
        TowerDeckList.Add(17);
        TowerDeckList.Add(20);
        TowerDeckList.Add(23);
        TowerDeckList.Add(25);

        //PossesTowerList.Add(0);
        //PossesTowerList.Add(5);
        //PossesTowerList.Add(10);
        //PossesTowerList.Add(14);
        //PossesTowerList.Add(17);
        //PossesTowerList.Add(20);
        //PossesTowerList.Add(23);
        //PossesTowerList.Add(25);
        for(int i = 0; i < 31; ++i)
        {
            PossesTowerList.Add(i);
        }


        PlayNum = 0;
        HighestStage = 0;
        ClearCount = 0;

        JObject jSaveData = new JObject();
        jSaveData[Define.Ruby] = Ruby;
        //jSaveData["TowerDeckList"] = TowerDeckList;
        //jSaveData["PossesTowerList"] = PossesTowerList;
        jSaveData[Define.PlayNum] = PlayNum;
        jSaveData[Define.HighestStage] = HighestStage;
        jSaveData[Define.ClearCount] = ClearCount;

        JArray jTowerDeckListData = new JArray();
        
        for (int i = 0; i < TowerDeckList.Count; ++i)
        {
            jTowerDeckListData.Add(TowerDeckList[i]);
        }
        JArray jPossesDeckListData = new JArray();
        for (int i = 0; i < PossesTowerList.Count; ++i)
        {
            jPossesDeckListData.Add(PossesTowerList[i]);
        }
        jSaveData[Define.TowerDeckList] = jTowerDeckListData;
        jSaveData[Define.PossesTowerList] = jPossesDeckListData;
        string SaveString = JsonConvert.SerializeObject(jSaveData, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/SaveData.json", SaveString);
    }

    public void Load()
    {
        string LoadString = File.ReadAllText(Application.persistentDataPath + "/SaveData.json");
        JObject LoadData = JObject.Parse(LoadString);

        Ruby = (int)LoadData[Define.Ruby];
        PlayNum = (int)LoadData[Define.PlayNum];
        HighestStage = (int)LoadData[Define.HighestStage];
        //PossesTowerList = LoadData[Define.PossesTowerList];
        JArray j1 = (JArray)LoadData[Define.PossesTowerList];
        PossesTowerList = j1.ToObject<List<int>>();
        //foreach (var item in j1)
        //{
        //    PossesTowerList.Add((int)item);
        //}

        JArray j2 = (JArray)LoadData[Define.TowerDeckList];
        TowerDeckList = j2.ToObject<List<int>>();
    }

    public void SaveAllData()
    {
        JObject jSaveData = new JObject();
        jSaveData[Define.Ruby] = Ruby;
        //jSaveData["TowerDeckList"] = TowerDeckList;
        //jSaveData["PossesTowerList"] = PossesTowerList;
        jSaveData[Define.PlayNum] = PlayNum;
        jSaveData[Define.HighestStage] = HighestStage;
        jSaveData[Define.ClearCount] = ClearCount;
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
        jSaveData[Define.TowerDeckList] = jTowerDeckListData;
        jSaveData[Define.PossesTowerList] = jPossesDeckListData;
        string SaveString = JsonConvert.SerializeObject(jSaveData, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/SaveData.json", SaveString);
    }

}
