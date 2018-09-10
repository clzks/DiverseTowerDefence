using System.Collections;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;

                if (_instance == null)// 해당 클래스를 찾았는데도 널이면 에러
                {
                    Debug.LogError("There's no active" + typeof(T) + "in this scene");
                }
                else // 아닐 경우는 해당 오브젝트가 삭제 안되게 
                {
                    Debug.Log(_instance.name);
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }

            return _instance;
        }
    }
}