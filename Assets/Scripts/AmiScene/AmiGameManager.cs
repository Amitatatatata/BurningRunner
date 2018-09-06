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
        
	}

    //プレイヤーのX座標を点数にする。
    public void  AddScore(int score)
    {
        if (isEnd) return;
        this.score = score;
        scoreText.text = this.score.ToString();
    }

    //ゲームを止める（終了時）
    public void FinishGame()
    {
        isEnd = true;
        //今の音を止めて終了台詞を再生する
        audioSource.Stop();
        audioSource.PlayOneShot(shuzoResultClips[Random.Range(0, shuzoResultClips.Length)]);

        //0.8秒後に各種ボタンを表示させる
        Invoke("ShowButtons", 0.8f);
    }

    //各種ボタンを表示させる
    public void ShowButtons()
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
