using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameDataManager : MonoBehaviour {
    public static InGameDataManager Instance;
    public List<Dictionary<string,object>> food_data;
    public int MAX_FOOD_ID;     // 데이터에 있는 음식 개수
    public Sprite [] food_img;  // 음식 이미지
    public List<Dictionary<string,object>> drink_data;
    public int MAX_DRINK_ID;    // 데이터에 있는 음료 개수
    public Sprite [] drink_img; // 음료 이미지

    // 손님 데이터
    public List<Dictionary<string,object>> guest_data;
    public int MAX_GUEST_ID;     // 데이터에 있는 음식 개수
    public Sprite [] guest_f_img;  // 손님 정면 이미지
    public Sprite [] guest_b_img;  // 손님 후면 이미지
    public int [] guest_score;   // 손님 점수 데이터

    private void Awake() {
        Instance = this;
        // 음식 데이터 로드
        food_data = CSVReader.Read("DB/food_list");
        MAX_FOOD_ID = food_data.Count;
        food_img = new Sprite[MAX_FOOD_ID];
        for (int i = 0; i < MAX_FOOD_ID; i++) {
            food_img[i] = Resources.Load<Sprite>(food_data[i]["image_path"].ToString());
        }

        // 음료 데이터 로드
        drink_data = CSVReader.Read("DB/drink_list");
        MAX_DRINK_ID = drink_data.Count;
        drink_img = new Sprite[MAX_DRINK_ID];
        for (int i = 0; i < MAX_DRINK_ID; i++) {
            drink_img[i] = Resources.Load<Sprite>(drink_data[i]["image_path"].ToString());
        }
    }

    // 음식의 id에 맞는 이미지 리턴
    public Sprite GetFoodImage(int index) {
        return food_img[index];
    }

    // 음료의 id에 맞는 이미지 리턴
    public Sprite GetDrinkImage(int index) {
        return drink_img[index];
    }
}
