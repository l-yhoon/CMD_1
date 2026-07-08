using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public float max_speed;
    SpriteRenderer SpriteRenderer;
    Animator anim;
    public float jump_power;
    bool is_jump = false;
    bool is_damaged = false;
    public GameManager gameManager;
    Collider2D Collider;
    public AudioClip jumpAudio;
    public AudioClip attackAudio;
    public AudioClip itemAudio;
    public AudioClip damagedAudio;
    public AudioClip dieAudio;
    public AudioClip finishAudio;
    AudioSource audioSource;
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        Collider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        
        // 키 떼면 멈춤
        if (Input.GetButtonUp("Horizontal") && !is_damaged)
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x*0.5f, rigid.linearVelocity.y);
        }

        //점프
        if (Input.GetButtonDown("Jump") && !is_jump && !is_damaged)
        {
            rigid.AddForce(Vector2.up * jump_power, ForceMode2D.Impulse);
            anim.SetBool("IsJump", true);
            is_jump = true;
            AudioPlay("JUMP");
        }

        //방향전환
        if(Input.GetButton("Horizontal"))
            SpriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        if(Mathf.Abs(rigid.linearVelocity.x) < 0.3)
            anim.SetBool("IsRun", false);
        else
            anim.SetBool("IsRun", true);

        
    }
    
    void FixedUpdate()
    {
        if (is_damaged) return;

        // 이동
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.linearVelocity.x > max_speed)
            rigid.linearVelocity = new Vector2(max_speed, rigid.linearVelocity.y);
        if (rigid.linearVelocity.x < max_speed * (-1))
            rigid.linearVelocity = new Vector2(max_speed * (-1), rigid.linearVelocity.y);

        // 레이 캐스트(2단점프 방지)
        if (rigid.linearVelocity.y < 0 && is_jump)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayhit = Physics2D.Raycast(rigid.position, Vector3.down, 0.7f, LayerMask.GetMask("PlatForm"));

            if (rayhit.collider != null)
            {
                is_jump = false;
                anim.SetBool("IsJump", false);
            }        
        }

    }
    //적이랑 충돌할때 무슨 함수 쓸지
        void OnCollisionEnter2D(Collision2D collision)
    {
    
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyMove enemyMove = collision.gameObject.GetComponent<EnemyMove>();
            if(enemyMove != null && rigid.linearVelocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else
                OnDameged(collision.transform.position);
        }

    }
    // 동전이랑 출구 충돌
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item") //이름으로 동전 구별
        {
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            AudioPlay("ITEM");
            if(isBronze)
            {
                gameManager.stagePoint += 50;
            }
            else if(isSilver)
            {
                gameManager.stagePoint += 100;
            }
            else if(isGold)
            {
                gameManager.stagePoint += 150;
            }
            
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Finish")
        {
            //다음 스테이지
            gameManager.NextStage();
            AudioPlay("FINISH");
        }
    }
    //움직이는 적 머리 밟을때
    void OnAttack(Transform enemy)
    {
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
        AudioPlay("ATTACK");
    }
    // 피격 이벤트
    void OnDameged(Vector2 targetPos)
    {   
        AudioPlay("DAMAGED");
        //체력 다운
        gameManager.HealthDown();

        //레이어 바꿔서 피격판정
        gameObject.layer = 11;
        
        //반투명으로 변경
        SpriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //애니메이션
        anim.SetTrigger("IsDamaged");

        //반응
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc * 8, 5), ForceMode2D.Impulse);

        //피격후 경직
        is_damaged = true;

        Invoke("MoveReStart", 0.5f);
        Invoke("OffDameged", 0.7f);
    }

    // 피격후 무적 해제 함수
    void OffDameged()
    {
        gameObject.layer = 10;
        SpriteRenderer.color = new Color(1, 1, 1);
    }

    // 경직 해제
    void MoveReStart()
    {
        is_damaged = false;
    }

    //죽을때
    public void OnDie() 
    {
        AudioPlay("DIE");
        SpriteRenderer.color = new Color(1, 1, 1, 0.4f);
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        Collider.enabled = false;
        SpriteRenderer.flipY = true;

        gameManager.RestartBtn.SetActive(true);
    }

    public void VelocityZero()
    {
        rigid.linearVelocity = Vector2.zero;
    }

    void AudioPlay(string s)
    {
        if (s == "JUMP") audioSource.clip = jumpAudio;
        else if (s == "ATTACK") audioSource.clip = attackAudio;
        else if (s == "ITEM") audioSource.clip = itemAudio;
        else if (s == "DAMAGED") audioSource.clip = damagedAudio;
        else if (s == "DIE") audioSource.clip = dieAudio;
        else if (s == "FINISH") audioSource.clip = finishAudio;

    audioSource.Play();
    }
}
