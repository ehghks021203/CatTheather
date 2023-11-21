using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthUser : HttpServerBase<AuthUser> {
    public class User {
        public bool isLogin;
        public string currUserUID;
        public int fish;
        public int can;
        public int rep;
    }

    protected override void Awake() {
        base.Awake();
        if (SceneManager.GetActiveScene().name != "StartScene") {
            // TODO: Start Scene에서 AuthUser 컴포넌트가 최초 생성된 것이 아니라면, StartScene으로 화면 전환
            Debug.Log("시작 화면이 StartScene이 아니라서 User의 데이터를 받아오지 못하였습니다.");
        }
    }

    public User user;

    public Coroutine CheckUser(string uuid, Action<Result> onSucceed = null, Action<Result> onFailed = null) {
        user = new User();
        string url = HTTPURL.SERVER_URL + HTTPURL.CHECK_USER;
        JObject json = new JObject();
        json["uuid"] = uuid;

        // 성공 시 콜백
        Action<Result> checkUserAction = (result) => {
            // Json 파싱
            var resultData = JObject.Parse(result.Json);
            if (resultData["msg"].ToString() == "user exist") {
                user.isLogin = true;
                user.currUserUID = resultData["user_data"]["user_id"].ToString();
                user.fish = int.Parse(resultData["user_data"]["fish"].ToString());
                user.can = int.Parse(resultData["user_data"]["can"].ToString());
                user.rep = int.Parse(resultData["user_data"]["rep"].ToString());
            } else {
                user.currUserUID = "undefined";
            }
        };
        onSucceed += checkUserAction;
        return StartCoroutine(SendRequest(
            url: url, 
            sendType: "POST", 
            json: json,
            onSucceed: onSucceed, 
            onFailed: null
        ));
    }

    public Coroutine CreateUser(string uuid, Action<Result> onSucceed = null, Action<Result> onFailed = null) {
        user = new User();
        string url = HTTPURL.SERVER_URL + HTTPURL.CREATE_USER;
        JObject json = new JObject();
        json["uuid"] = uuid;

        // 성공 시 콜백
        Action<Result> createUserAction = (result) => {
            // Json 파싱
            var resultData = JObject.Parse(result.Json);
            if (resultData["msg"].ToString() == "create user") {
                CheckUser(uuid: uuid);
            }
        };
        onSucceed += createUserAction;
        return StartCoroutine(SendRequest(
            url: url, 
            sendType: "POST", 
            json: json,
            onSucceed: onSucceed, 
            onFailed: null
        ));
    }
}