using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShuzoManager : MonoBehaviour {
    [SerializeField] private GameObject frameImage;     //炎演出用Image。 SetActiveで表示、非表示を切り替える
    [SerializeField] private Camera camera;             //プレイヤーを追うカメラ
    [SerializeField] private LayerMask groundLayer;     //地面のレイヤー

    private Animator animator;
    private Rigidbody2D rBody;

    //trueの時にはジャンプできず、毎フレーム着地したか確認する。
    bool isJump = true;

    //ジャンプ後数フレームは地面に触れたままなので、10フレームはジャンプ負荷にする
    //そのためにフレーム数を数える変数
    int frames = 0;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        rBody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    void FixedUpdate()
    {
        //プレイヤーを一定速度で常に右に移動させる。
        rBody.velocity = new Vector2(10.0f, rBody.velocity.y);

        //プレイヤーをカメラの真ん中に常に捉える。
        camera.transform.position = new Vector3(transform.position.x, camera.transform.position.y, camera.transform.position.z);

        //Jump対応キーを押したらジャンプする。
        if (!isJump && Input.GetButtonDown("Jump")) Jump();

        //ジャンプ中の時、地面に着地したか確認する
        if (isJump && frames > 5)
        {
            //プレイヤーから二本の線を下に飛ばして地面に触れていたら着地している(isJump = false)とする。
            isJump = !(Physics2D.Linecast(transform.position + (Vector3.left * 0.3f),
                transform.position + (Vector3.down * 0.1f), groundLayer) ||
                    Physics2D.Linecast(transform.position + (Vector3.right * 0.3f),
                transform.position + (Vector3.down * 0.1f), groundLayer)
                );
        }
        else if (frames <= 5) frames++;
    }

    //炎演出を表示させる。
    public void OnFrameMode()
    {
        //プレイヤーの動きを激しくする。
        animator.speed = 2.0f;
        //炎をみえるようにする。
        frameImage.SetActive(true);
    }

    //炎演出を非表示にする。
    public void OffFrameMode()
    {
        //プレイヤーの動きを元に戻す。
        animator.speed = 1.0f;
        //炎を見えなくする。
        frameImage.SetActive(false);
    }

    public void Jump()
    {
        //rigidbodyに上向きの力を加えてジャンプさせる。
        rBody.AddForce(new Vector2(0.0f, 400.0f));

        //Jump中にする。
        isJump = true;
        frames = 0;
    }

    //プレイヤーがゴールに触れたか判定する。
    void OnTriggerEnter2D(Collider2D col)
    {
        //もし触れたものがGoalタグならプレイヤーの位置を元に戻す。
        if(col.gameObject.tag == "Goal")
        {
            transform.position = new Vector3(0, 0, 0);
        }
    }
}
