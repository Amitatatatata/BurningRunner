using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShuzoManager : MonoBehaviour {
    [SerializeField] private GameObject frameImage;     //炎演出用Image。 SetActiveで表示、非表示を切り替える
    [SerializeField] private Camera camera;             //プレイヤーを追うカメラ
    [SerializeField] private LayerMask groundLayer;     //地面のレイヤー
    [SerializeField] private float speedX = 15.0f;
    [SerializeField] private float jumpPower = 4000.0f;

    private Rigidbody2D rBody;
    private BoxCollider2D boxCollider;

    bool isDead = false;

    //jump可能かどうか
    bool canJump = false;

    //ジャンプの強さの倍率 (0.0から1.0)
    float jumpPowerRate = 0.0f;


    //元のスピード
    private float baseSpeedX;

	// Use this for initialization
	void Start () {
        rBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        baseSpeedX = speedX;
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    void FixedUpdate()
    {
        if(isDead)
        {
            rBody.velocity = new Vector2(0, -50f);
            return;
        }

        canJump = (Physics2D.Linecast(transform.position + (Vector3.left * 0.3f),
                transform.position + (Vector3.down * 0.1f), groundLayer) ||
                    Physics2D.Linecast(transform.position + (Vector3.right * 0.3f),
                transform.position + (Vector3.down * 0.1f), groundLayer)
                && rBody.velocity.y < 0);
        //プレイヤーを一定速度で常に右に移動させる。
        rBody.velocity = new Vector2(speedX, rBody.velocity.y);

        //プレイヤーをカメラの真ん中に常に捉える。
        camera.transform.position = new Vector3(transform.position.x, camera.transform.position.y, camera.transform.position.z);

        if (Input.GetButtonDown("Jump")) JumpButtonDown();

        if (Input.GetButton("Jump")) OnJumpButton();

        //Jump対応キーを押したらジャンプする。
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

    void JumpButtonDown()
    {
        jumpPowerRate = 0.5f;
    }

    void OnJumpButton()
    {
        if(jumpPowerRate <= 1.0f)
        jumpPowerRate += 0.02f;
    }

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
            Debug.Log("Dead.");
            boxCollider.enabled = false;
            isDead = true;
        }
    }
}
