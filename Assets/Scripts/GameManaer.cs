using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameManaer : MonoBehaviour {
    public static GameManaer Instance { get; private set;}
    void Awake() => Instance = this;

    // 인게임 UI 및 텍스트
    [Header("InGame UI & Text")]
    [SerializeField] private Image gagebarFillUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TMP_Text scoreTMP;
    [SerializeField] private TMP_Text guestTMP;
    [SerializeField] private TMP_Text gainFishTMP;
    [SerializeField] private TMP_Text gainCanTMP;

    // 결과창 텍스트
    [Header("Result Page Text")]
    [SerializeField] private TMP_Text ticketScoreTMP;
    [SerializeField] private TMP_Text foodScoreTMP;
    [SerializeField] private TMP_Text drinkScoreTMP;
    [SerializeField] private TMP_Text thiefScoreTMP;
    [SerializeField] private TMP_Text totalScoreTMP;
    [SerializeField] private TMP_Text accuracyTMP;

    // 게임 스코어 변수
    private int totalScore = 0;
    private int ticketScore = 0;
    private int foodScore = 0;
    private int drinkScore = 0;
    private int thiefScore = 0;

    // 재화 변수
    private int gainFish = 0;
    private int gainCan = 0;

    // 정확도 계산을 위한 변수
    public int totalFood = 0;
    public int correctFood = 0;
    
    public int combo = 0;
    public bool showGagebar = false;
    public bool isGameStart = true;
    public bool isGameOver = false;
    private bool gameOverPanelOpen = false;
    private int curGuest = -1;
    private float maxTime = 8.0f;
    private float curTime = 8.0f;

    private void Update() {
        // 점수 표시
        scoreTMP.text = totalScore.ToString();
        guestTMP.text = GuestList.Instance.currentGuestCount.ToString();

        // 획득 재화 표시
        gainFishTMP.text = "+ " + gainFish.ToString();
        gainCanTMP.text = "+ " + gainCan.ToString();

        // 손님 대기시간 게이지바 표시
        if (showGagebar) {
            if (curGuest != GuestList.Instance.currentGuestCount) {
                gagebarFillUI.transform.parent.gameObject.SetActive(true);
                curGuest = GuestList.Instance.currentGuestCount;
                maxTime = 8.0f - 0.1f * curGuest;
                curTime = maxTime;
            }
            if (curTime < 0) {
                GameManaer.Instance.isGameOver = true;
            }
            curTime -= Time.deltaTime;
            gagebarFillUI.fillAmount = curTime / maxTime;
        } else {
            gagebarFillUI.transform.parent.gameObject.SetActive(false);
        }

        // 게임 오버 시 팝업 창 표시
        if (isGameOver && !gameOverPanelOpen) {
            gameOverPanelOpen = true;
            StartCoroutine(GameOverAnim());
        }
    }

    public void PlusScore(int score, string type) {
        if (type == "ticket") {
            ticketScore += score + 100*(combo/10);
            combo++;
        } else if (type == "food") {
            foodScore += score + 100*(combo/10);
            totalFood++;
            correctFood++;
        } else if (type == "drink") {
            drinkScore += score + 100*(combo/10);
            totalFood++;
            correctFood++;
        } else if (type == "thief") {
            thiefScore += score + 100*(combo/10);
        }
        totalScore = ticketScore + foodScore + drinkScore + thiefScore;
    }

    public void MinusScore(string type) {
        // 손님 대기시간 1초 차감
        curTime = curTime >= 1.0f ? curTime - 1.0f : 0.0f;
        if (type == "food") {
            foodScore -= 500 + 100*(combo/10);
            foodScore = foodScore >= 0 ? foodScore : 0;
        } else if (type == "drink") {
            drinkScore -= 500 + 100*(combo/10);
            drinkScore = drinkScore >= 0 ? drinkScore : 0;
        }
        correctFood--;
        totalScore = ticketScore + foodScore + drinkScore + thiefScore;

        // 콤보 보너스 초기화
        combo = 0;
    }

    public void GainFish(int gain) {
        gainFish += gain;
    }

    public void GainCan(int gain) {
        gainCan += gain;
    }

    public void GamePause(bool isPause) {
        if (isPause) { Time.timeScale = 0; }
        else { Time.timeScale = 1; }
        
    }

    public void LoadScene(string scene_name) {
        gagebarFillUI.transform.parent.gameObject.SetActive(false);
        SceneManager.LoadScene(scene_name);
    }

    public void GameQuit() {
        Application.Quit();
    }

    public IEnumerator GameOverAnim() {
        AuthUser.Instance.GainUserGoods(gainFish, gainCan);
        gameOverUI.SetActive(true);
        float accuracy = totalFood == 0 ? 0 : (float) correctFood/totalFood * 100;

        // 결과창 텍스트 값 변경
        ticketScoreTMP.text = ticketScore.ToString();
        foodScoreTMP.text = foodScore.ToString();
        drinkScoreTMP.text = drinkScore.ToString();
        thiefScoreTMP.text = thiefScore.ToString();
        totalScoreTMP.text = totalScore.ToString();
        accuracyTMP.text = string.Format("{0:0.##}", accuracy) + "%";


        gameOverUI.GetComponent<Image>().DOFade(0.5f, 1.0f);
        yield return new WaitForSeconds(1.0f);
        gameOverUI.transform.GetChild(0).GetComponent<RectTransform>().DOLocalMove(new Vector3(0, 0, 0), 0.7f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.7f);
        gameOverUI.transform.GetChild(1).DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.5f).SetEase(Ease.OutBack);
        gameOverUI.transform.GetChild(2).DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.5f).SetEase(Ease.OutBack);
        yield return null;
    }
}
