using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class InGameDAO : HTTPServerBase 
{
    #region SINGLETON
    private static InGameDAO instance;

    public static InGameDAO Instance 
    {
        get 
        {
            if (instance == null) 
            {
                GameObject obj;
                obj = GameObject.Find(typeof(InGameDAO).Name);
                if (obj == null) 
                {
                    obj = new GameObject(typeof(InGameDAO).Name);
                    instance = obj.AddComponent<InGameDAO>();
                } 
                else 
                {
                    instance = obj.GetComponent<InGameDAO>();
                }
            }
            return instance;
        }
    }
    #endregion

    private void Awake() 
    {
        FetchInGameDataFromServer();
    }    

    public Coroutine FetchInGameDataFromServer(Action<Result> onSucceed = null, Action<Result> onFailed = null) 
    {
        string url = HTTPURL.SERVER_URL + HTTPURL.GET_INGAME_DATA;

        // 성공 시 콜백
        Action<Result> getInGameDataAction = (result) => {
            // 음식 데이터 파싱
            ParseFoodData(result);
            InGameDTO.Instance.MAX_FOOD_ID = InGameDTO.Instance.FOOD_DATA.Count;

            // 음료 데이터 파싱
            ParseDrinkData(result);
            InGameDTO.Instance.MAX_DRINK_ID = InGameDTO.Instance.DRINK_DATA.Count;

            // 손님 데이터 파싱
            ParseGuestData(result);
            GameManager.Instance.GameStart();
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

    private void ParseFoodData(Result result)
    {
        var foodData = JObject.Parse(result.Json)["food_data"];
        InGameDTO.Instance.FOOD_DATA = MapJsonToFoodDTOList(foodData, EnumTypes.FoodType.Food);
    }

    private void ParseDrinkData(Result result)
    {
        var drinkData = JObject.Parse(result.Json)["drink_data"];
        InGameDTO.Instance.DRINK_DATA = MapJsonToFoodDTOList(drinkData, EnumTypes.FoodType.Drink);
    }

    private void ParseGuestData(Result result)
    {
        var guestData = JObject.Parse(result.Json)["guest_data"];
        InGameDTO.Instance.GUEST_DATA = MapJsonToGuestDTOList(guestData);
        (
            InGameDTO.Instance.COMMON_GUEST_ID_LIST, 
            InGameDTO.Instance.RARE_GUEST_ID_LIST,
            InGameDTO.Instance.UNIQUE_GUEST_ID_LIST,
            InGameDTO.Instance.HIDDEN_GUEST_ID_LIST,
            InGameDTO.Instance.SPECIAL_GUEST_ID_LIST
        ) = MapJsonToGuestDTOListClassifyByRarity(guestData);
    }

    private List<FoodDTO> MapJsonToFoodDTOList(JToken dataList, EnumTypes.FoodType foodType) 
    {
        List <FoodDTO> foodList = new();
        foreach (var data in dataList)
        {
            FoodDTO foodEntity = new()
            {
                Id = Convert.ToInt32(data["id"]),
                FoodType = foodType,
                GainFish = Convert.ToInt32(data["gain_fish"]),
                GainCan = Convert.ToInt32(data["gain_can"]),
                ReturnScore = Convert.ToInt32(data["gain_score"]),
                Image = Resources.Load<Sprite>(path: data["img_path"].ToString())
            };
            foodList.Add(foodEntity);
        }
        return foodList;
    }
    
    private GuestDTO MapJsonToGuestDTO(JToken data)
    {
        return new GuestDTO
        {
            Id = Convert.ToInt32(data["id"]),
            GainFish = Convert.ToInt32(data["gain_fish"]),
            GainCan = Convert.ToInt32(data["gain_can"]),
            ReturnScore = Convert.ToInt32(data["gain_score"]),
            FrontImage = Resources.Load<Sprite>(path: data["img_f_path"].ToString()),
            BackImage = Resources.Load<Sprite>(path: data["img_b_path"].ToString())
        };
    }

    private List<GuestDTO> MapJsonToGuestDTOList(JToken dataList)
    {
        List<GuestDTO> guestList = new();
        foreach (var data in dataList)
        {
            GuestDTO guestEntity = MapJsonToGuestDTO(data);
            guestList.Add(guestEntity);
        }
        return guestList;
    }

    private (List<int>, List<int>, List<int>, List<int>, List<int>) MapJsonToGuestDTOListClassifyByRarity(JToken dataList)
    {
        List<int> commonGuestList = new();
        List<int> rareGuestList = new();
        List<int> uniqueGuestList = new();
        List<int> hiddenGuestList = new();
        List<int> specialGuestList = new();
        foreach (var data in dataList)
        {
            GuestDTO guestEntity = MapJsonToGuestDTO(data);
            switch (data["grade"].ToString())
            {
                case "common":
                    commonGuestList.Add(guestEntity.Id);
                    break;
                case "rare":
                    rareGuestList.Add(guestEntity.Id);
                    break;
                case "unique":
                    uniqueGuestList.Add(guestEntity.Id);
                    break;
                case "hidden":
                    hiddenGuestList.Add(guestEntity.Id);
                    break;
                case "special":
                    specialGuestList.Add(guestEntity.Id);
                    break;
                default:
                    Debug.Log("Fail");
                    break;
            }
        }
        return (commonGuestList, rareGuestList, uniqueGuestList, hiddenGuestList, specialGuestList);
    }

}
