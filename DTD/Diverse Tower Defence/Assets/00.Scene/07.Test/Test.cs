using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
  
    private void Start()
    {
        ConstructManager.Instance.MainCamera = Camera.main;
        for (int i = 0; i < ConstructManager.Instance.nGroundNum; ++i)
        {
            ConstructManager.Instance.groundInfoList[i].position = GameObject.Find("Constructable Place/" + i.ToString()).transform.position;
        }
    }

}
