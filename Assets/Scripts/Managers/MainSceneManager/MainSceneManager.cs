using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;


public class MainSceneManager : MonoBehaviour {
    [SerializeField] private GameObject fishBar;
    [SerializeField] private GameObject canBar;
    [SerializeField] private GameObject repBar;

    void Start() {
        // 유저 정보 UI에 반영
        fishBar.transform.GetChild(1).GetComponent<TMP_Text>().text = string.Format("{0:#,0}", UserDTO.Instance.Fish);
        canBar.transform.GetChild(1).GetComponent<TMP_Text>().text = string.Format("{0:#,0}", UserDTO.Instance.Can);
        repBar.transform.GetChild(1).GetComponent<TMP_Text>().text = string.Format("{0:#,0}/100", UserDTO.Instance.Rep);
        // TODO: 평판은 일정 수치 넘어가면 레벨업하도록 
        /*
        ex) 현재 평판 수치가 300이면 3 레벨 조건인 평판 수치 200을 달성하였기 
            때문에 3레벨로 표시하고, 나머지 평판 수치 100 표시하는 방식.
        */
    }
}