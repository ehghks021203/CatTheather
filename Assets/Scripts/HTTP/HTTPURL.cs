using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HTTPURL {
    public static readonly string SERVER_URL = "https://blue.kku.ac.kr:51203";
    
    // 하위 루트 경로
    public static readonly string CHECK_USER = "/check_user";
    public static readonly string CREATE_USER = "/create_user";
    public static readonly string GET_FOOD_DATA = "/get_in_game_data";
    public static readonly string GAIN_USER_GOODS = "/gain_user_goods";
}