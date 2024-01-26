using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShowResultModal : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUIObj;

    // 결과창 텍스트
    [Header("Result Page Text")]
    [SerializeField] private TMP_Text ticketScoreTMP;
    [SerializeField] private TMP_Text foodScoreTMP;
    [SerializeField] private TMP_Text etcScoreTMP;
    [SerializeField] private TMP_Text totalScoreTMP;
    [SerializeField] private TMP_Text accuracyTMP;

    public void ShowResult(Structs.ScoreData scoreData)
    {
        if (!gameOverUIObj.activeSelf)
        {
            StartCoroutine(GameOverAnim(scoreData));
        }
    }

    public IEnumerator GameOverAnim(Structs.ScoreData scoreData) 
    {
        gameOverUIObj.SetActive(true);
        float accuracy = scoreData.totalFood == 0 ? 0 : (float) scoreData.correctFood/scoreData.totalFood*100;

        // 결과창 텍스트 값 변경
        ticketScoreTMP.text = scoreData.ticketScore.ToString();
        foodScoreTMP.text = scoreData.foodScore.ToString();
        etcScoreTMP.text = scoreData.etcScore.ToString();
        totalScoreTMP.text = scoreData.totalScore.ToString();
        accuracyTMP.text = string.Format("{0:0.##}", accuracy) + "%";


        gameOverUIObj.GetComponent<Image>().DOFade(0.5f, 1.0f);
        yield return new WaitForSeconds(1.0f);
        gameOverUIObj.transform.GetChild(0).GetComponent<RectTransform>().DOLocalMove(new Vector3(0, 0, 0), 0.7f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.7f);
        gameOverUIObj.transform.GetChild(1).DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.5f).SetEase(Ease.OutBack);
        gameOverUIObj.transform.GetChild(2).DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.5f).SetEase(Ease.OutBack);
        yield return null;
    }
}