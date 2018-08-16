using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public int nCurrentScene = 0;

    // ======================= 게임 세팅관련 ======================== // (나중에 옵션매니저에 갈것임)
    [Range(0.1f, 10.0f)]
    public float nGameSpeed = 1.0f;
    
    //============================================================================================
    private static object _Lock = new object();
    public static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            lock(_Lock)

            {
                if(instance == null)
                {
                    instance = GameObject.Find("Manager").AddComponent<GameManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }

    ////public static GameManager instance = null;
    //public static GameManager Instance
    //{
    //    get
    //    {
    //        return instance;
    //    }
    //}
    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else if (instance != this)
    //    {
    //        Destroy(gameObject);
    //    }
    //    
    //    DontDestroyOnLoad(gameObject);
    //}

    private void Start()
    {
        ConstructManager.Instance.TowerStatusDbParsing("DTD.sqlite");
        ConstructManager.Instance.TowerTypeDbParsing("DTD.sqlite");

        EnemyManager.Instance.EnemyStatusDbParsing("DTD.sqlite");
        EnemyManager.Instance.LoadModelList();

        BulletManager.Instance.LoadModelList();
    }

    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            StageManager.Instance.isGameStart = true;
        }
    }
}
