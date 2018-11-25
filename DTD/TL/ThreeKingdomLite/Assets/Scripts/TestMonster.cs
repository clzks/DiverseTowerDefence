using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonster : MonoBehaviour
 {
    public Transform tr;

	// Use this for initialization
	void Start ()
    {
        tr = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown("a"))
        {
            tr.position += new Vector3(-1.0f, 0.0f, 0.0f);
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown("d"))
        {
            tr.position += new Vector3(1.0f, 0.0f, 0.0f);
        }
    }
}
