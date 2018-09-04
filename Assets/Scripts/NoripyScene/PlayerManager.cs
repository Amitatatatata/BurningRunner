using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;//DOTween (HOTween v2)をAsset Storeからインポート

public class PlayerManager : MonoBehaviour {
    public GameObject gameManager;//ゲームマネージャー

    public LayerMask blockLayer;//ブロックレイヤー

    private Rigidbody2D rbody;//プレイヤー制御用rigidbody2D

    private const float MOVE_SPEED = 3;//移動速度固定値
    private float moveSpeed;//移動速度
    private float jumpPower = 400;//ジャンプの力
    private bool goJump = false;//ジャンプしたか否か
    private bool canJump = false;//ブロックに設置しているか否か
    private bool usingButtons = false;//ボタンを押しているか否か

    public enum MOVE_DIR
    {//移動方向
        STOP,
        LEFT,
        RIGHT,
    };

    private MOVE_DIR moveDirection = MOVE_DIR.STOP;//移動方向

    public AudioClip jumpSE;//効果音:ジャンプ
    public AudioClip getSE;//効果音:アイテムゲット時
    public AudioClip stampSE;//効果音:踏みつけ

    private AudioSource audioSource;//オーディオソース

 	// Use this for initialization
	void Start () {
        audioSource = gameManager.GetComponent<AudioSource>();
        rbody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        canJump =
            Physics2D.Linecast(transform.position - (transform.right * 0.3f),
                transform.position - (transform.up * 0.1f), blockLayer) ||
            Physics2D.Linecast(transform.position + (transform.right * 0.3f),
                transform.position - (transform.up * 0.1f), blockLayer);

        if (!usingButtons)
        {
            float x = Input.GetAxisRaw("Horizontal");

            if(x == 0)
            {
                moveDirection = MOVE_DIR.STOP;
            }
            else
            {
                if (x < 0)
                {
                    moveDirection = MOVE_DIR.LEFT;
                }
                else
                {
                    moveDirection = MOVE_DIR.RIGHT;
                }
            }

            if (Input.GetKeyDown("space"))
            {
                PushJumpButton();
            }
        }
	}

    void FixedUpdate()
    {
        //移動方向で処理を分岐
        switch (moveDirection)
        {
            case MOVE_DIR.STOP://停止
                moveSpeed = 0;
                break;
            case MOVE_DIR.LEFT://左に移動
                moveSpeed = MOVE_SPEED * -1;
                transform.localScale = new Vector2(-1,1);
                break;
            case MOVE_DIR.RIGHT://右に移動
                moveSpeed = MOVE_SPEED;
                transform.localScale = new Vector2(1,1);
                break;
        }

        rbody.velocity = new Vector2(moveSpeed, rbody.velocity.y);

        //ジャンプ処理
        if (goJump)
        {
            audioSource.PlayOneShot(jumpSE);
            rbody.AddForce(Vector2.up * jumpPower);
            goJump = false;
        }
    }

    //ジャンプボタンを押した
    public void PushJumpButton()
    {
        if (canJump)
        {
            goJump = true;
        }
    }

    //左ボタンを押した
    public void PushLeftButton()
    {
        moveDirection = MOVE_DIR.LEFT;
        usingButtons = true;
    }

    //右ボタンを押した
    public void PushRightButton()
    {
        moveDirection = MOVE_DIR.RIGHT;
        usingButtons = true;
    }

    //移動ボタンを放した
    public void ReleaseMoveButton()
    {
        moveDirection = MOVE_DIR.STOP;
        usingButtons = false;
    }
}
