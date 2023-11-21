using System;
using System.Collections;
using Newtonsoft.Json.Linq;
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
        // 유저의 UUID 받아오기
        var uuid = SystemInfo.deviceUniqueIdentifier;
        AuthUser.Instance.CheckUser(uuid: uuid);
        StartCoroutine(HTTPWait());
    }

    public void HandlerGuestBtn() {
        isClickUserCreate = true;
        guestPlayBtn.SetActive(false);
        oauthPlayBtn.SetActive(false);
        waitMsgObj.SetActive(true);
        // 유저의 UUID 받아오기
        var uuid = SystemInfo.deviceUniqueIdentifier;
        AuthUser.Instance.CreateUser(uuid: uuid);
        StartCoroutine(HTTPWait());
    }

    IEnumerator HTTPWait() {
        while (true) {
            if (!isClickUserCreate && AuthUser.Instance.user.currUserUID == null) {
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
            } else if (!isClickUserCreate && AuthUser.Instance.user.currUserUID == "undefined") {
                waitMsgObj.SetActive(false);
                waitMsg = "";
                uidText.text = "UID: " + AuthUser.Instance.user.currUserUID;
                guestPlayBtn.SetActive(true);
                oauthPlayBtn.SetActive(true);
                yield return null;
                break;
            } else if (isClickUserCreate && AuthUser.Instance.user.currUserUID == null) {
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
                uidText.text = "UID: " + AuthUser.Instance.user.currUserUID;
                startBtn.SetActive(true);
                yield return null;
                break;
            }
        }
    }
}