using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animatorCon;
    public Rigidbody2D target;

    bool isLive;
    Rigidbody2D rigid;
    SpriteRenderer spriteR;
    Animator anim;
    WaitForFixedUpdate waitFix;
    Collider2D coll;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        waitFix = new WaitForFixedUpdate();
        coll = GetComponent<Collider2D>();
    }


    void FixedUpdate()
    {
        if (!GameManager.instance.isLive) return;

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;


        //update huong di chuyen cua enemy ve phia target
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }
    private void LateUpdate()
    {
        if (!GameManager.instance.isLive) return;

        //update huong nhin cua nhan vat enemy ve phia target
        spriteR.flipX = target.position.x < rigid.position.x;
    }
    private void OnEnable()
    {
        //spawn
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriteR.sortingOrder = 6;
        anim.SetBool("Dead", false);
        health = maxHealth;
        
    }
    public void Init(SpawnData data)
    {
        //gan thong tin animation, mau, toc do cho enemy
        anim.runtimeAnimatorController = animatorCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
        {
            return;
        }
        // khi cham vao bullet cua nguoi choi, enemy bi tru mau bang damage cua bullet
        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());
        if (health <= 0)
        {
            isLive = false;

            //khien khong the tuong tac voi object da chet
            coll.enabled = false;
            rigid.simulated = false;
            spriteR.sortingOrder = 5;
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            for (int i = 0; i < animatorCon.Length; i++)
            {
                if (anim.runtimeAnimatorController == animatorCon[i])
                {
                    switch (i)
                    {
                        case 0:
                        case 2:
                            GameManager.instance.GetExp();
                            break;
                        case 1:
                        case 3:
                            GameManager.instance.GetExp();
                            GameManager.instance.GetExp();
                            break;
                        case 4:
                            GameManager.instance.GetExp();
                            GameManager.instance.GetExp();
                            GameManager.instance.GetExp();
                            GameManager.instance.GetExp();
                            GameManager.instance.GetExp();
                            break;
                    }
                    break;
                }
            }
            
            if (GameManager.instance.isLive)    AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);


            /*    Dead();*/ // mau xuong 0, kich hoat event chet cua enemy
        }
        else {
            anim.SetTrigger("Hit");//khi mau chua xuong hong. chay anim get hit
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);

        }
    }
    IEnumerator KnockBack()
    {

        yield return waitFix;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }
    private void Dead()
    {

        //xu ly event chet cua enemy
        
        gameObject.SetActive(false);

    }
}
