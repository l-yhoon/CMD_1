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

    void Update()
    {
        UIpoint.text = (totalPoint + stagePoint).ToString();
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
