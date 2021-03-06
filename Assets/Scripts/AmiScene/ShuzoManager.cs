﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShuzoManager : MonoBehaviour {
    [SerializeField] private GameObject frameImage;     //炎演出用Image。 SetActiveで表示、非表示を切り替える
    [SerializeField] private Camera camera;             //プレイヤーを追うカメラ
    [SerializeField] private LayerMask groundLayer;     //地面のレイヤー
    [SerializeField] private float speedX = 15.0f;      //プレイヤーの横方向の速さ
    [SerializeField] private float jumpPower = 4000.0f; //プレイヤーのジャンプ力
    [SerializeField] private AmiGameManager amiGameManager; //ゲーム制御用スクリプト（ゲームの終了、スコアの計測など）
    [SerializeField] private float smallJumpRate = 0.5f;   //小ジャンプの強さ

    //自分自身のrigidbodyとcollider、AudioSource
    private Rigidbody2D rBody;
    private BoxCollider2D boxCollider;
    private AudioSource audioSource;

    //死んだかどうか
    bool isDead = false;

    //ジャンプ可能かどうか
    bool canJump = false;

    //ジャンプの強さの倍率 (0.0から1.0)
    float jumpPowerRate = 0.0f;


    //効果音関連
    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip powerUpClip;
    [SerializeField] AudioClip[] shuzoJumpClips;


    int scoreRate = 1;  //スコアの倍率
    [SerializeField] int scoreRatio = 10; //scoreRateにかけてどれくらい増加させるか
    int prevX = 0;

    //ジャンプボタンが押されているか
    bool onButton = false;
    bool onSpaceButton = false;
    bool onMouseButton = false;

    //元の横方向のスピード
    private float baseSpeedX;

    //最初のX座標
    private float firstX;

	void Start () {
        rBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        baseSpeedX = speedX;
        firstX = transform.position.x;
	}

    //Input処理はFixedUpdateに入れるとうまく動かない
    void Update()
    {
        

        if (isDead) return;

        int deltaX = (int)(transform.position.x - firstX) - prevX;

        amiGameManager.AddScore(deltaX * scoreRate);

        prevX = (int)(transform.position.x - firstX);

        //プレイヤーの足元から下方向に2本の線を飛ばし、Blockに触れていればジャンプ可能(true)
        canJump = (Physics2D.Linecast(transform.position + (Vector3.left * 8.2f),
                transform.position + (Vector3.down * 1f) + (Vector3.left * 3f), groundLayer) ||
                    Physics2D.Linecast(transform.position + (Vector3.right * 1.4f),
                transform.position + (Vector3.down * 1f) + (Vector3.left * 3f), groundLayer)
                && rBody.velocity.y < 0);

        /*
        Debug.Log(canJump);

        Debug.DrawLine(transform.position + (Vector3.left * 8.2f),
                transform.position + (Vector3.down * 1f) + (Vector3.left * 3f));
        Debug.DrawLine(transform.position + (Vector3.right * 1.4f),
                transform.position + (Vector3.down * 1f) + (Vector3.left * 3f));
        */

        //ジャンプキーを押し始めたとき
        if (!onButton && !onSpaceButton && Input.GetButtonDown("Jump"))
        {
            onButton = true;
            onSpaceButton = true;
            JumpButtonDown();
        }
        //ジャンプキーを押しているとき
        else if (onSpaceButton && onButton && Input.GetButton("Jump")) OnJumpButton();
        //ジャンプキーを離したときにジャンプする
        else if (onSpaceButton && onButton && canJump && Input.GetButtonUp("Jump"))
        {
            JumpButtonUp();
            onButton = false;
            onSpaceButton = false;
        }

        if (!onButton && !onMouseButton && Input.GetButtonDown("Fire1"))
        {
            onButton = true;
            onMouseButton = true;
            JumpButtonDown();
        }
        //ジャンプキーを押しているとき
        else if (onMouseButton && onButton && Input.GetButton("Fire1")) OnJumpButton();
        //ジャンプキーを離したときにジャンプする
        else if (onMouseButton && onButton && canJump && Input.GetButtonUp("Fire1"))
        {
            JumpButtonUp();
            onButton = false;
            onMouseButton = false;
        }
    }

    void FixedUpdate()
    {
        
        if(isDead)
        {
            rBody.velocity = new Vector2(0, -50f);
            if (transform.position.y < -300.0f) Destroy(gameObject);
            return;
        }

        //プレイヤーを一定速度で常に右に移動させる。
        rBody.velocity = new Vector2(speedX, rBody.velocity.y);

        //プレイヤーをカメラの真ん中に常に捉える。
        camera.transform.position = new Vector3(transform.position.x, camera.transform.position.y, camera.transform.position.z);

    }

    //炎演出を表示させる。
    public void OnFrameMode()
    {
        //プレイヤーの動きを激しくする。
        speedX *= 2;

        scoreRate *= scoreRatio;

        //炎をみえるようにする。
        frameImage.SetActive(true);

        audioSource.PlayOneShot(shuzoJumpClips[Random.Range(0, shuzoJumpClips.Length)], 0.25f);
    }

    //炎演出を非表示にする。
    public void OffFrameMode()
    {
        //プレイヤーの動きを元に戻す。
        speedX = baseSpeedX;

        scoreRate = 1;

        //炎を見えなくする。
        frameImage.SetActive(false);
    }

    //ジャンプキーを押し始めたとき
    void JumpButtonDown()
    {
        //すぐにキーを離してもある程度飛べるように初期値を0.7倍にする
        jumpPowerRate = smallJumpRate;
    }

    //ジャンプキーを押しているときジャンプ力の倍率を0.01ずつ増やす
    void OnJumpButton()
    {
        if(jumpPowerRate <= 1.0f)
        jumpPowerRate += 0.01f;
    }

    //ジャンプキーを離したときにジャンプする
    void JumpButtonUp()
    {

        //rigidbodyに上向きの力を加えてジャンプさせる。
        rBody.AddForce(new Vector2(0.0f, jumpPower * jumpPowerRate));

        audioSource.PlayOneShot(jumpClip, 0.3f);
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        //もし触れたものがGoalタグならプレイヤーの位置を元に戻す。
        if(col.gameObject.tag == "Goal")
        {
            transform.position = new Vector3(0, 0, 0);
        }

        //ラケットに触れたら興奮モード
        if(col.gameObject.tag == "Racket")
        {
            Destroy(col.gameObject);
            OnFrameMode();
            Invoke("OffFrameMode", 1.0f);
        }
        
        //トゲかマグマに当たったら死亡
        if(col.gameObject.tag == "Needle")
        {
            Dead();
        }
        
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Larva")
        {
            Dead();
        }
    }

    //死んだときの処理
    private void Dead()
    {
        //プレイヤーの当たり判定を消す。
        boxCollider.enabled = false;
        //死亡中にする
        isDead = true;
        //ゲームを終了する
        amiGameManager.FinishGame();
    }
}
