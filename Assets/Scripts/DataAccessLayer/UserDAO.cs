using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserDAO : HTTPServerBase {
    // 싱글톤으로 사용하기 위한 로직
    #region SINGLETON
    private static UserDAO instance;

    public static UserDAO Instance {
        get {
            if (instance == null) {
                GameObject obj;
                obj = GameObject.Find(typeof(UserDAO).Name);
                if (obj == null) {
                    obj = new GameObject(typeof(UserDAO).Name);
                    instance = obj.AddComponent<UserDAO>();
                } else {
                    instance = obj.GetComponent<UserDAO>();
                }
            }
            return instance;
        }
    }

    protected void Awake() {
        DontDestroyOnLoad(gameObject);
        if (SceneManager.GetActiveScene().name != "StartScene") {
            // TODO: Start Scene에서 UserDAO 컴포넌트가 최초 생성된 것이 아니라면, StartScene으로 화면 전환
            Debug.Log("시작 화면이 StartScene이 아니라서 User의 데이터를 받아오지 못하였습니다.");
        }
    }
    #endregion

    public Coroutine CheckUser(string uuid, Action<Result> onSucceed = null, Action<Result> onFailed = null) 
    {
        string url = HTTPURL.SERVER_URL + HTTPURL.CHECK_USER;
        JObject json = new()
        {
            ["uuid"] = uuid
        };

        // 성공 시 콜백
        Action<Result> checkUserAction = (result) => {
            // Json 파싱
            var resultData = JObject.Parse(result.Json);
            if (resultData["msg"].ToString() == "user exist") 
            {
                UserDTO.Instance.IsLogin = true;
                UserDTO.Instance.UserUID = resultData["user_data"]["user_id"].ToString();
                UserDTO.Instance.Fish = int.Parse(resultData["user_data"]["fish"].ToString());
                UserDTO.Instance.Can = int.Parse(resultData["user_data"]["can"].ToString());
                UserDTO.Instance.Rep = int.Parse(resultData["user_data"]["rep"].ToString());
            } 
            else 
            {
                UserDTO.Instance.UserUID = "undefined";
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

    public Coroutine CreateUser(string uuid, Action<Result> onSucceed = null, Action<Result> onFailed = null) 
    {
        string url = HTTPURL.SERVER_URL + HTTPURL.CREATE_USER;
        JObject json = new()
        {
            ["uuid"] = uuid
        };

        // 성공 시 콜백
        Action<Result> createUserAction = (result) => {
            // Json 파싱
            var resultData = JObject.Parse(result.Json);
            if (resultData["msg"].ToString() == "create user") 
            {
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

    public Coroutine GainUserGoods(int gainFish = 0, int gainCan = 0, int gainRep = 0, Action<Result> onSucceed = null, Action<Result> onFailed = null) 
    {
        var uuid = SystemInfo.deviceUniqueIdentifier;
        string url = HTTPURL.SERVER_URL + HTTPURL.GAIN_USER_GOODS;
        JObject json = new()
        {
            ["uuid"] = uuid,
            ["gain_fish"] = gainFish,
            ["gain_can"] = gainCan,
            ["gain_rep"] = gainRep
        };

        // 성공 시 콜백
        Action<Result> gainUserGoodsAction = (result) => {
            // Json 파싱
            var resultData = JObject.Parse(result.Json);
            if (resultData["msg"].ToString() == "update user data") 
            {
                UserDTO.Instance.Fish += gainFish;
                UserDTO.Instance.Can += gainCan;
                UserDTO.Instance.Rep += gainRep;
            }
        };
        onSucceed += gainUserGoodsAction;
        return StartCoroutine(SendRequest(
            url: url, 
            sendType: "POST", 
            json: json,
            onSucceed: onSucceed, 
            onFailed: null
        ));
    }
}