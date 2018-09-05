using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {
    [SerializeField] Text description;  //Click to Startのテキスト

    //1 = 1frame 60frameごとにdescriptionを点滅させる
    int timer = 0;

    //descriptionがアクティブかどうか
    bool isDescriptionActive = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    
    void FixedUpdate()
    {
        timer++;
        if(timer >= 30)
        {
            timer = 0;
            isDescriptionActive = !isDescriptionActive;
            description.gameObject.SetActive(isDescriptionActive);
        }

        if (Input.GetButtonDown("Fire1")) SceneManager.LoadScene("TestScene2");
    }
}
