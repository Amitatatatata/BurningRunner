using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AmiGameManager : MonoBehaviour {
    //Scoreを表示させるテキスト
    [SerializeField] Text scoreText;
    [SerializeField] Image tweetButton; //ツイートボタン用テキスト　ゲーム終了時にアクティブにする
    [SerializeField] Button titleButton; //タイトルへ戻るボタン　ゲーム終了時にアクティブにする

    private int score = 0;

    //ゲーム終了かどうか
    private bool isEnd = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(!isEnd) AddScore();
	}

    public void  AddScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void FinishGame()
    {
        isEnd = true;
        Invoke("ShowTweetButtonAndRanking", 1.0f);
    }

    public void ShowTweetButtonAndRanking()
    {
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(score);
        tweetButton.gameObject.SetActive(true);
        titleButton.gameObject.SetActive(true);
    }

    public void Tweet()
    {
        naichilab.UnityRoomTweet.Tweet("burning_runner",
            "【Burning Runner】で" + score + "点だったよ！！",
            "unityroom", "unity1week");
    }

    public void OnClickTitleButton()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
