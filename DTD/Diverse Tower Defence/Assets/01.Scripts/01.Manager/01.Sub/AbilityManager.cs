using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private static object _Lock = new object();
    public static AbilityManager instance = null;
    public static AbilityManager Instance
    {
        get
        {
            lock (_Lock)

            {
                if (instance == null)
                {
                    instance = GameObject.Find("Manager").AddComponent<AbilityManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }
}
