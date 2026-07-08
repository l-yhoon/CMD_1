using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D collider;

    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CapsuleCollider2D>();
        Invoke("Move", 3);
    }

    void Update()
    {

        // 적 움직임 애니메이션
        anim.SetInteger("WalkSpeed", nextMove);

        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;
    }

    void FixedUpdate()
    {
        // 적 움직임
        rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);

        // 적 낭떨어지 방지
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D hit = Physics2D.Raycast(frontVec, Vector3.down, 1);
        if (hit.collider == null)
        {
            nextMove *= -1;
            CancelInvoke();
            Invoke("Move", 3);
        }
    }

    public void OnDamaged()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        collider.enabled = false;
        spriteRenderer.flipY = true;
        Invoke("DeActive", 2);
        Destroy(gameObject, 2f);
    }   

    void DeActive()
    {
        gameObject.SetActive(false);
    }

    // 적 움직임 함수
    void Move()
    {
        nextMove = Random.Range(-1, 2);

        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Move", nextThinkTime);
    }
}