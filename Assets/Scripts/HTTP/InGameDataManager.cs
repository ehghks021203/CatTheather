using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class InGameDataManager : HttpServerBase {
    public static InGameDataManager Instance;

    public class InGameData {
        public List<Dictionary<string, object>> food_data;
        public int MAX_FOOD_ID;     // 데이터에 있는 음식 개수
        public Sprite [] food_img;  // 음식 이미지
        public List<Dictionary<string, object>> drink_data;
        public int MAX_DRINK_ID;    // 데이터에 있는 음식 개수
        public Sprite [] drink_img;  // 음식 이미지
        public List<Dictionary<string, object>> guest_data;
        public int MAX_GUEST_ID;     // 데이터에 있는 손님 수
        public Sprite [] guest_f_img;  // 손님 정면 이미지
        public Sprite [] guest_b_img;  // 손님 후면 이미지
        public int [] guest_score;   // 손님 점수 데이터
    }

    public InGameData inGameData;

    private void Awake() {
        Instance = this;
        GetInGameData();
    }

    #region GET_FOOD_DATA
    // 음식의 id에 맞는 이미지 리턴
    public Sprite GetFoodImage(int index) {
        return inGameData.food_img[index];
    }

    public int GetFoodScore(int index) {
        return int.Parse(inGameData.food_data[index]["gain_score"].ToString());
    }

    public int GetFoodGainFish(int index) {
        return int.Parse(inGameData.food_data[index]["gain_fish"].ToString());
    }

    public int GetFoodGainCan(int index) {
        return int.Parse(inGameData.food_data[index]["gain_can"].ToString());
    }
    #endregion

    #region GET_DRINK_DATA
    // 음료의 id에 맞는 이미지 리턴
    public Sprite GetDrinkImage(int index) {
        return inGameData.drink_img[index];
    }
    public int GetDrinkScore(int index) {
        return int.Parse(inGameData.drink_data[index]["gain_score"].ToString());
    }
    public int GetDrinkGainFish(int index) {
        return int.Parse(inGameData.drink_data[index]["gain_fish"].ToString());
    }
    public int GetDrinkGainCan(int index) {
        return int.Parse(inGameData.drink_data[index]["gain_can"].ToString());
    }
    #endregion

    # region GET_GUEST_DATA
    public Sprite GetGuestFrontImage(int index) {
        return inGameData.guest_f_img[index];
    }
    public Sprite GetGuestBackImage(int index) {
        return inGameData.guest_b_img[index];
    }
    public int GetGuestScore(int index) {
        return int.Parse(inGameData.guest_data[index]["gain_score"].ToString());
    }
    public int GetGuestGainFish(int index) {
        return int.Parse(inGameData.guest_data[index]["gain_fish"].ToString());
    }
    public int GetGuestGainCan(int index) {
        return int.Parse(inGameData.guest_data[index]["gain_can"].ToString());
    }
    #endregion

    public Coroutine GetInGameData(Action<Result> onSucceed = null, Action<Result> onFailed = null) {
        string url = HTTPURL.SERVER_URL + HTTPURL.GET_FOOD_DATA;
        inGameData = new InGameData();

        // 성공 시 콜백
        Action<Result> getInGameDataAction = (result) => {
            // Json 파싱
            var resultData = JObject.Parse(result.Json);
            inGameData.food_data = resultData["food_data"].ToObject<List<Dictionary<string, object>>>();
            inGameData.MAX_FOOD_ID = inGameData.food_data.Count;
            inGameData.food_img = new Sprite[inGameData.MAX_FOOD_ID];
            for (int i = 0; i < inGameData.MAX_FOOD_ID; i++) {
                inGameData.food_img[i] = Resources.Load<Sprite>(inGameData.food_data[i]["img_path"].ToString());
            }

            inGameData.drink_data = resultData["drink_data"].ToObject<List<Dictionary<string, object>>>();
            inGameData.MAX_DRINK_ID = inGameData.drink_data.Count;
            inGameData.drink_img = new Sprite[inGameData.MAX_DRINK_ID];
            for (int i = 0; i < inGameData.MAX_DRINK_ID; i++) {
                inGameData.drink_img[i] = Resources.Load<Sprite>(inGameData.drink_data[i]["img_path"].ToString());
            }

            inGameData.guest_data = resultData["guest_data"].ToObject<List<Dictionary<string, object>>>();
            inGameData.MAX_GUEST_ID = inGameData.guest_data.Count;
            inGameData.guest_f_img = new Sprite[inGameData.MAX_GUEST_ID];
            inGameData.guest_b_img = new Sprite[inGameData.MAX_GUEST_ID];
            for (int i = 0; i < inGameData.MAX_GUEST_ID; i++) {
                inGameData.guest_f_img[i] = Resources.Load<Sprite>(inGameData.guest_data[i]["img_f_path"].ToString());
                inGameData.guest_b_img[i] = Resources.Load<Sprite>(inGameData.guest_data[i]["img_b_path"].ToString());
            }

            FoodList.Instance.SpawnStart();
            GuestList.Instance.SpawnStart();
        };
        onSucceed += getInGameDataAction;
        return StartCoroutine(SendRequest(
            url: url, 
            sendType: "POST", 
            json: null,
            onSucceed: onSucceed, 
            onFailed: null
        ));
    }
}
