using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public void ClickNewGameScene()
    {
        GameManager.Instance.nCurrentScene = 1;
        SceneManager.LoadScene("Test");
    }

    public void ClickContinueScene()
    {

    }

    public void ClickEquipScene()
    {

    }

    public void ClickShopScene()
    {

    }
}
