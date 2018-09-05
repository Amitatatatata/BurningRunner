using System.Collections;
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

    //自分自身のrigidbodyとcollider
    private Rigidbody2D rBody;
    private BoxCollider2D boxCollider;

    //死んだかどうか
    bool isDead = false;

    //ジャンプ可能かどうか
    bool canJump = false;

    //ジャンプの強さの倍率 (0.0から1.0)
    float jumpPowerRate = 0.0f;


    //元の横方向のスピード
    private float baseSpeedX;

	void Start () {
        rBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        baseSpeedX = speedX;
	}

    void FixedUpdate()
    {
        if(isDead)
        {
            rBody.velocity = new Vector2(0, -50f);
            return;
        }

        //プレイヤーを一定速度で常に右に移動させる。
        rBody.velocity = new Vector2(speedX, rBody.velocity.y);

        //プレイヤーをカメラの真ん中に常に捉える。
        camera.transform.position = new Vector3(transform.position.x, camera.transform.position.y, camera.transform.position.z);

        //プレイヤーの足元から下方向に2本の線を飛ばし、Blockに触れていればジャンプ可能(true)
        canJump = (Physics2D.Linecast(transform.position + (Vector3.left * 8f),
                transform.position + (Vector3.down * 1f), groundLayer) ||
                    Physics2D.Linecast(transform.position + (Vector3.right * 8f),
                transform.position + (Vector3.down * 1f), groundLayer)
                && rBody.velocity.y < 0);

        //ジャンプキーを押し始めたとき
        if (Input.GetButtonDown("Jump")) JumpButtonDown();
        //ジャンプキーを押しているとき
        if (Input.GetButton("Jump")) OnJumpButton();
        //ジャンプキーを離したときにジャンプする
        if (canJump && Input.GetButtonUp("Jump")) JumpButtonUp();


        
        
    }

    //炎演出を表示させる。
    public void OnFrameMode()
    {
        //プレイヤーの動きを激しくする。
        speedX *= 2;
        //炎をみえるようにする。
        frameImage.SetActive(true);
    }

    //炎演出を非表示にする。
    public void OffFrameMode()
    {
        //プレイヤーの動きを元に戻す。
        speedX = baseSpeedX;
        //炎を見えなくする。
        frameImage.SetActive(false);
    }

    //ジャンプキーを押し始めたとき
    void JumpButtonDown()
    {
        //すぐにキーを離してもある程度飛べるように初期値を0.7倍にする
        jumpPowerRate = 0.7f;
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
        if(col.gameObject.tag == "Needle" || col.gameObject.tag == "Larva")
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
