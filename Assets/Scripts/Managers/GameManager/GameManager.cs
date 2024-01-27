using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviourSingleton<GameManager> {
    [Header("Managers")]
    public ShowResultModal showResultModal;
    public GuestSpawner guestSpawner;
    public FoodSpawner foodSpawner;

    
    // 게임 스코어 변수
    Structs.ScoreData scoreData;

    // 재화 변수
    private int gainFish = 0;
    private int gainCan = 0;
    private int gainRep = 0;
    
    public int combo = 0;
    public bool isGameStarted = true;
    public bool isGameOver = false;
    
    

    [Header("UI & Text Components")]
    [SerializeField] private GameObject mainCanvasObj;
    [SerializeField] private TMP_Text scoreTMP;
    [SerializeField] private TMP_Text guestTMP;
    [SerializeField] private TMP_Text gainFishTMP;
    [SerializeField] private TMP_Text gainCanTMP;
    
    [Header("UI & Text Prefabs")]
    [SerializeField] private GameObject requireGoodsPrefab;


    public void GameStart() {
        // 손님 및 음식 생성 시작
        guestSpawner.SpawnStart();
        foodSpawner.SpawnStart();
        scoreData = new Structs.ScoreData();
    }
    
    public void ReturnFoodValues(FoodDTO data) {
        // Fish animation
        if (data.GainFish != 0 && data.GainCan == 0) {
            var obj = Instantiate(requireGoodsPrefab);
            obj.transform.SetParent(mainCanvasObj.transform, false);
            obj.GetComponent<RequireGoodAnimation>().StartAnimation(data.GainFish, EnumTypes.GoodsType.Fish);
        }
        else if (data.GainFish == 0 && data.GainCan != 0) {
            // Can animation
            var obj = Instantiate(requireGoodsPrefab);
            obj.transform.SetParent(mainCanvasObj.transform, false);
            obj.GetComponent<RequireGoodAnimation>().StartAnimation(data.GainFish, EnumTypes.GoodsType.Can);
        }
        else if (data.GainFish != 0 && data.GainCan != 0) {
            // Fish animation
            var fObj = Instantiate(requireGoodsPrefab);
            fObj.transform.SetParent(mainCanvasObj.transform, false);
            fObj.GetComponent<RequireGoodAnimation>().StartAnimation(data.GainFish, EnumTypes.GoodsType.Fish, 25.0f);
            // Can animation
            var cObj = Instantiate(requireGoodsPrefab);
            cObj.transform.SetParent(mainCanvasObj.transform, false);
            cObj.GetComponent<RequireGoodAnimation>().StartAnimation(data.GainFish, EnumTypes.GoodsType.Can, -25.0f);
        }

        scoreData.totalScore += data.ReturnScore;
        scoreData.foodScore += data.ReturnScore;
        scoreData.totalFood++;
        scoreData.correctFood++;

        gainFish += data.GainFish;
        gainCan += data.GainCan;
    }

    public void ReturnGuestValues(GuestDTO data) {
        if (data.GainFish != 0 && data.GainCan == 0) {
            // Fish animation
            var obj = Instantiate(requireGoodsPrefab);
            obj.transform.SetParent(mainCanvasObj.transform, false);
            obj.GetComponent<RequireGoodAnimation>().StartAnimation(data.GainFish, EnumTypes.GoodsType.Fish);
        }
        else if (data.GainFish == 0 && data.GainCan != 0) {
            // Can animation
            var obj = Instantiate(requireGoodsPrefab);
            obj.transform.SetParent(mainCanvasObj.transform, false);
            obj.GetComponent<RequireGoodAnimation>().StartAnimation(data.GainFish, EnumTypes.GoodsType.Can);
        }
        else if (data.GainFish != 0 && data.GainCan != 0) {
            // Fish animation
            var fObj = Instantiate(requireGoodsPrefab);
            fObj.transform.SetParent(mainCanvasObj.transform, false);
            fObj.GetComponent<RequireGoodAnimation>().StartAnimation(data.GainFish, EnumTypes.GoodsType.Fish, 25.0f);
            // Can animation
            var cObj = Instantiate(requireGoodsPrefab);
            cObj.transform.SetParent(mainCanvasObj.transform, false);
            cObj.GetComponent<RequireGoodAnimation>().StartAnimation(data.GainFish, EnumTypes.GoodsType.Can, -25.0f);
        }
        
        scoreData.totalScore += data.ReturnScore;
        scoreData.ticketScore += data.ReturnScore;
        
        gainFish += data.GainFish;
        gainCan += data.GainCan;
    }

    public void ReturnWrongFood() {
        scoreData.totalFood++;
    }

    public void GameOver() {
        if (!isGameOver) {
            isGameOver = true;
            showResultModal.ShowResult(scoreData);
            UserDAO.Instance.GainUserGoods(gainFish: gainFish, gainCan: gainCan, gainRep: gainRep);
        }
    }

    public void GameRestart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameEnd() {
        SceneManager.LoadScene("MainScene");
    }

    private void Update() {
        // 점수 표시
        scoreTMP.text = scoreData.totalScore.ToString();
        guestTMP.text = guestSpawner.currentGuestIndex.ToString();

        // 획득 재화 표시
        gainFishTMP.text = "+ " + gainFish.ToString();
        gainCanTMP.text = "+ " + gainCan.ToString();

        // 게임 오버 시 팝업 창 표시
        if (isGameOver) {
            showResultModal.ShowResult(scoreData);
        }
    }
}
