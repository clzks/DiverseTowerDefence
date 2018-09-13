using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Camera camera;

    private void Start()
    {
        camera = Camera.main;
        ConstructManager.Instance.MainCamera = Camera.main;
        for (int i = 0; i < ConstructManager.Instance.nGroundNum; ++i)
        {
            ConstructManager.Instance.groundInfoList[i].position = GameObject.Find("Constructable Place/" + i.ToString()).transform.position;
        }

        
        

    }

    private void Update()
    {
        if (ConstructManager.Instance.LabelList.Count != 0)
        {
            for (int i = 0; i < ConstructManager.Instance.nGroundNum; ++i)
            {
                Vector3 ScreenPos = camera.WorldToScreenPoint(ConstructManager.Instance.groundInfoList[i].position);
                ConstructManager.Instance.LabelList[i].GetComponent<UILabel>().transform.position = ConstructManager.Instance.groundInfoList[i].position;
                //ConstructManager.Instance.LabelList[i].GetComponent<UILabel>().transform.loo
            }
        }
    }

}
