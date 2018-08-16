using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static object _Lock = new object();
    public static PoolManager instance = null;
    public static PoolManager Instance
    {
        get
        {
            lock (_Lock)

            {
                if (instance == null)
                {
                    instance = GameObject.Find("Manager").AddComponent<PoolManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }

    public int GetPoolObject(int nSize, List<GameObject> list)      // 활성화 되지 않은 인덱스를 반환, 없으면 -1 반환
    {
        int nIndex = -1;
        for(int i = 0; i < nSize; ++i)
        {
            if (!list[i].activeSelf)
            {
                nIndex = i;
                break;
            }
        }

        return nIndex;
    }

    public void AddObjectPool(ref List<GameObject> list, GameObject addObject)
    {
        list.Add(addObject);
    }
}
