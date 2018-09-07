using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AmiGameManager : MonoBehaviour {
    [SerializeField] Text scoreText;    //Scoreを表示させるテキスト
    [SerializeField] Image tweetButton; //ツイートボタン用テキスト　ゲーム終了時にアクティブにする
    [SerializeField] Button titleButton; //タイトルへ戻るボタン　ゲーム終了時にアクティブにする
    [SerializeField] Button retryButton; //リトライボタン　シーンを読み直してもう一度遊ぶ
    [SerializeField] Button rankingButton; //ランキングボタン ランキングを表示する
    [SerializeField] GameObject levelResultLabel; //結果表示用ラベル
    [SerializeField] Text levelResultText;  //結果表示用テキスト
    [SerializeField] GameObject[] mapUnitPrefabs;  //マップ生成用　マップユニット群
    [SerializeField] float[] mapUnitWidths;  //マップユニットの横幅 mapUnitPrefabsの添字とリンクしている
    [SerializeField] int maximumLevel;      //最大レベル数 (=ユニットの種類数）
    [SerializeField] GameObject shuzo;

    private int score = 0;
    private int nowLevel = 0;  //現在のマップレベル (-1されているので注意）

    //マップ関連
    [SerializeField] int unitNum = 10;  //１マップ中のマップユニットの数
    private float destroyAndCreateMapPoint = 0.0f;
    private GameObject[] prevMap;       //今いるマップのひとつ前のマップ
    private GameObject[] nowMap;        //今いるマップ
    private GameObject[] nextMap;       //今いるマップの一つ先のマップ
    private const float FIXED_Y = -21.4f;    //マップのY座標（常に固定）
    private float mapLeftX = -224.0f;        //マップ中の左端ユニットのX座標

    //ゲーム終了かどうか
    private bool isEnd = false;

    //自分のAudioSource
    private AudioSource audioSource;

    //効果音関連
    [SerializeField] AudioClip[] shuzoResultClips;



    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        InitMap();
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.DrawLine(new Vector3(destroyAndCreateMapPoint, 200), new Vector3(destroyAndCreateMapPoint, -200));

        
        if (!isEnd && shuzo.transform.position.x > destroyAndCreateMapPoint) UpdateMap();
	}

    void InitMap()
    {
        //マップの配列を初期化する
        prevMap = new GameObject[unitNum];
        nowMap = new GameObject[unitNum];
        nextMap = new GameObject[unitNum];

        
        destroyAndCreateMapPoint = mapLeftX;
        //一つ目のマップを作る
        CreateMap();
    }

    void CreateMap()
    {
        //nowLevelがmaximumLevel以下の時はマップの並びをランダムにする
        if(nowLevel < maximumLevel)
        {
            destroyAndCreateMapPoint = mapLeftX;
            for (int i = 0; i < unitNum; i++)
            {
                nextMap[i] = (GameObject)Instantiate(mapUnitPrefabs[nowLevel]);
                nextMap[i].transform.position = new Vector3(mapLeftX + mapUnitWidths[nowLevel], FIXED_Y, 0);
                mapLeftX += mapUnitWidths[nowLevel];

                if (i < unitNum / 2) destroyAndCreateMapPoint += mapUnitWidths[nowLevel];
                
            }
        }
        else
        {
            destroyAndCreateMapPoint = mapLeftX;
            for (int i = 0; i < unitNum; i++)
            {
                int randomNum = Random.Range(0, maximumLevel);
                nextMap[i] = (GameObject)Instantiate(mapUnitPrefabs[randomNum]);
                nextMap[i].transform.position = new Vector3(mapLeftX + mapUnitWidths[randomNum], FIXED_Y, 0);
                mapLeftX += mapUnitWidths[randomNum];

                if (i < unitNum / 2) destroyAndCreateMapPoint += mapUnitWidths[randomNum];

            }
        }
    }

    void UpdateMap()
    {
        nowLevel++;

        DestroyMap();

        for (int i = 0; i < unitNum; i++)
        {
            prevMap[i] = nowMap[i];
        }

        for (int i = 0; i < unitNum; i++)
        {
            nowMap[i] = nextMap[i];
        }

        CreateMap();

    }

    void DestroyMap()
    {
        foreach(GameObject unit in prevMap)
        {
            if(unit != null) Destroy(unit);
        }
    }

    //プレイヤーのX座標を点数にする。
    public void  AddScore(int score)
    {
        if (isEnd) return;
        this.score += score;
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
        levelResultLabel.gameObject.SetActive(true);

        levelResultText.text = "あなたのレベルは " + nowLevel.ToString();
    }

    public void Tweet()
    {
        naichilab.UnityRoomTweet.Tweet("burning_runner",
            "【Burning Runner】で" + score + "点だったよ！！レベルは" + nowLevel + "！！！",
            "unityroom", "unity1week");
    }

    public void OnClickTitleButton()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void OnClickRetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickRankingButton()
    {
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(score);
    }
}
