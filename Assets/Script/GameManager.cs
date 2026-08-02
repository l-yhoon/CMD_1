using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
// 점수랑 스테이지, 체력, 추락 관리용
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public TextMeshProUGUI UIpoint;
    public TextMeshProUGUI UIstage;
    public GameObject RestartBtn;

    // --- [추가] 제한시간 관련 변수 ---
    public TextMeshProUGUI UItime; 
    public float maxTime; 
    float currentTime; 
    // --------------------------------

    void Start()
    {
        // 시작 시 시간 초기화
        currentTime = maxTime;
    }


    void Update()
    {
        UIpoint.text = (totalPoint + stagePoint).ToString();

        // --- [추가] 제한시간 계산 및 UI 적용 ---
        if (health > 0 && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UItime.text = Mathf.CeilToInt(currentTime).ToString(); // 소수점 올림하여 정수로 표시

            // 시간 초과 시 사망 처리
            if (currentTime <= 0) 
            {
                currentTime = 0;
                health = 1; // 체력이 몇이든 즉사하도록 1로 만듦
                HealthDown(); 
            }
        }
        // --------------------------------------
    }

    public void NextStage()
    {
        if(stageIndex < Stages.Length-1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();
            UIstage.text = "Stage " + (stageIndex + 1).ToString();
        }
        else // 게임 클리어
        {
            int timeBonus = Mathf.CeilToInt(currentTime) * 10;
            stagePoint += timeBonus;
            Time.timeScale = 0;
            RestartBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Restart?";
            RestartBtn.SetActive(true);
        }

        //점수 계산
        totalPoint += stagePoint;
        stagePoint = 0;
    }    

    //낭떠러지 떨어질때 충돌로 판정, 
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            health = 1;
            HealthDown(); //체력 시스템 쓸거면 활성화

            //원래 위치로 돌려보내기
            if(health > 0)
            {
                PlayerReposition();
            }
        }   
    }

//피 감소 함수
    public void HealthDown()
    {   
        UIhealth[health - 1].color = new Color(1, 1, 1, 0.4f);

            stagePoint -= 100;
            health--;
        if(health <= 0)
        {
            player.OnDie();
            Debug.Log("죽었습니다");
        }
    }
    
    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, 0);
        player.VelocityZero();
    }

    public void BtnAction()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}
