using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    private void Update()
    {
        //if(Input.GetKey(KeyCode.Alpha1))
        //{
        //    GameManager.Instance.nCurrentScene = 1;
        //    LoadTestScene();
        //}
        //else if (Input.GetKey(KeyCode.Alpha2))
        //{
        //    GameManager.Instance.nCurrentScene = 2;
        //    LoadBulletTestScene();
        //}
        GameManager.Instance.nCurrentScene = 0;
        LoadMainScene();
    }

    //public void LoadTestScene()
    //{
    //    SceneManager.LoadScene("Test");
    //}
    //public void LoadBulletTestScene()
    //{
    //    SceneManager.LoadScene("BulletTest");
    //}

    public void LoadMainScene()
    {
        SceneManager.LoadScene("Main");
    }

}
