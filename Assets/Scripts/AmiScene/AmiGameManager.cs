using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmiGameManager : MonoBehaviour {
    [SerializeField] Text scoreText;

    private int score = 0;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        AddScore();
	}

    public void  AddScore()
    {
        score++;
        scoreText.text = score.ToString();
    }
}
