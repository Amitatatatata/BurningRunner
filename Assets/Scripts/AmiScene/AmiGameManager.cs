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
    [SerializeField] Button retryButton; //リトライボタン　シーンを読み直してもう一度遊ぶ
    [SerializeField] Button rankingButton; //ランキングボタン ランキングを表示する

    private int score = 0;

    //ゲーム終了かどうか
    private bool isEnd = false;

    //自分のAudioSource
    private AudioSource audioSource;

    //効果音関連
    [SerializeField] AudioClip[] shuzoResultClips;



    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
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
        audioSource.Stop();
        audioSource.PlayOneShot(shuzoResultClips[Random.Range(0, shuzoResultClips.Length)]);
        Invoke("ShowTweetButtonAndRanking", 0.8f);
    }

    public void ShowTweetButtonAndRanking()
    {
        tweetButton.gameObject.SetActive(true);
        titleButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        rankingButton.gameObject.SetActive(true);
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

    public void OnClickRetryButton()
    {
        SceneManager.LoadScene("TestScene2");
    }

    public void OnClickRankingButton()
    {
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(score);
    }
}
