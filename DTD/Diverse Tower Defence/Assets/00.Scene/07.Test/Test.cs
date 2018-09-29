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
        StageManager.Instance.LoadNodeInfo();
    }

    private void Update()
    {
        if (ConstructManager.Instance.LabelList.Count != 0)
        {
            for (int i = 0; i < ConstructManager.Instance.nGroundNum; ++i)
            {
                Vector3 ScreenPos = Camera.main.WorldToViewportPoint(ConstructManager.Instance.groundInfoList[i].position);
                ScreenPos = UICamera.mainCamera.ViewportToWorldPoint(ScreenPos);
                ScreenPos.z = 0.0f;
                UILabel label = ConstructManager.Instance.LabelList[i].GetComponent<UILabel>();
                label.transform.position = ScreenPos;
            }
        }
    }

}
