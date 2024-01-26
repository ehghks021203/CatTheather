using System;
using System.Collections;
using TMPro;
using UnityEngine;


public class StartSceneManager : MonoBehaviour {
    [SerializeField] private GameObject waitMsgObj;
    [SerializeField] private GameObject guestPlayBtn;
    [SerializeField] private GameObject oauthPlayBtn;
    [SerializeField] private GameObject startBtn;
    [SerializeField] private TMP_Text uidText;
    private string waitMsg = "";
    private bool isClickUserCreate = false;

    void Start() {
        // 60프레임 고정
        Application.targetFrameRate = 60;

        // 유저의 UUID 받아오기
        var uuid = SystemInfo.deviceUniqueIdentifier;
        UserDAO.Instance.CheckUser(uuid: uuid);
        StartCoroutine(HTTPWait());
    }

    public void HandlerGuestBtn() {
        isClickUserCreate = true;
        guestPlayBtn.SetActive(false);
        oauthPlayBtn.SetActive(false);
        waitMsgObj.SetActive(true);
        // 유저의 UUID 받아오기
        var uuid = SystemInfo.deviceUniqueIdentifier;
        UserDAO.Instance.CreateUser(uuid: uuid);
        StartCoroutine(HTTPWait());
    }

    IEnumerator HTTPWait() {
        while (true) {
            if (!isClickUserCreate && UserDTO.Instance.UserUID == null) {
                if (waitMsg == "") {
                    waitMsg = "서버에서 유저 정보를 받아오는중.";
                } else {
                    waitMsg += ".";
                    if (waitMsg.Split(".").Length > 4) {
                        waitMsg = "서버에서 유저 정보를 받아오는중.";
                    }
                }
                waitMsgObj.GetComponent<TMP_Text>().text = waitMsg;
                yield return new WaitForSeconds(0.2f);
            } else if (!isClickUserCreate && UserDTO.Instance.UserUID == "undefined") {
                waitMsgObj.SetActive(false);
                waitMsg = "";
                uidText.text = "UID: " + UserDTO.Instance.UserUID;
                guestPlayBtn.SetActive(true);
                oauthPlayBtn.SetActive(true);
                yield return null;
                break;
            } else if (isClickUserCreate && UserDTO.Instance.UserUID == null) {
                if (waitMsg == "") {
                    waitMsg = "서버에 유저 정보를 생성하는중.";
                } else {
                    waitMsg += ".";
                    if (waitMsg.Split(".").Length > 4) {
                        waitMsg = "서버에 유저 정보를 생성하는중.";
                    }
                }
                waitMsgObj.GetComponent<TMP_Text>().text = waitMsg;
                yield return new WaitForSeconds(0.2f);
            } else {
                waitMsgObj.SetActive(false);
                waitMsg = "";
                uidText.text = "UID: " + UserDTO.Instance.UserUID;
                startBtn.SetActive(true);
                yield return null;
                break;
            }
        }
    }
}