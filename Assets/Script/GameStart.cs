using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject gameStartUI; // 인스펙터에서 GameStart 할당
    public GameObject gameUI;      // 인스펙터에서 GameUI 할당

    void Start()
    {
        // 시작 화면에서는 시간을 멈춰 캐릭터와 제한시간이 흐르지 않게 함
        Time.timeScale = 0f;
        
        // 초기 UI 상태 강제 설정
        gameStartUI.SetActive(true);
        gameUI.SetActive(false);
    }

    // Start 버튼에 연결할 함수
    public void GameStart()
    {
        // 시간 흐름 재개
        Time.timeScale = 1f;

        // UI 전환
        gameStartUI.SetActive(false);
        gameUI.SetActive(true);
    }

    // Exit 버튼에 연결할 함수
    public void GameExit()
    {   
        Debug.Log("종료 버튼 눌림!");
        Application.Quit();
    }
}