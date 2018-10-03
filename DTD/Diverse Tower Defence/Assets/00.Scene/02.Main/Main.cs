using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public void ClickNewGameScene()
    {
        GameManager.Instance.nCurrentScene = 3;
        SceneManager.LoadScene("DeckSetting");
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
