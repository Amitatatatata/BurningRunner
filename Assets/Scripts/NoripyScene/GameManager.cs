using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public int stageNo;//ステージナンバー

    public GameObject textGameOver;//「ゲームオーバー」テキスト

    public enum GAME_MODE//ゲーム状態
    {
        PLAY,//プレイ中
        GAMEOVER,//ゲームオーバー
    };

    public GAME_MODE gameMode = GAME_MODE.PLAY;

    public AudioClip gameoverSE;//効果音:ゲームオーバー

    private AudioSource audioSource;//オーディオソース

	// Use this for initialization
	void Start () {
        audioSource = this.gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //ゲームオーバー処理
    public void GameOver()
    {
        audioSource.PlayOneShot(gameoverSE);
        textGameOver.SetActive(true);

        //naichilab.RankingLoader.Instance.SendScoreAndShowRanking( score );

        //naichilab.UnityRoomTweet.Tweet("YOUR-GAMEID","あなたの得点は" + score + "です","unityroom","unity1week","WeeybleGame");
    }
}
