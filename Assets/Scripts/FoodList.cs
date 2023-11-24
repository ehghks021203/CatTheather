using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodList : MonoBehaviour {
    public static FoodList Instance { get; private set;}
    void Awake() => Instance = this;

    [SerializeField] private GameObject foodObjectPrefab;
    Queue<Food> foodObjectQueue = new Queue<Food>();
    public int [] foodDisplay = new int[3];
    Queue<Food> drinkObjectQueue = new Queue<Food>();
    public int [] drinkDisplay = new int[3];

    public void SpawnStart() {
        Init(3);
        StartCoroutine(FoodSpawn());
        StartCoroutine(DrinkSpawn());
    }

    // 재사용할 음식 오브젝트 일정 수 만큼 생성 (3개)
    /* 
    Instantiate()와 Destroy()는 꽤 무거운 작업이기 때문에, 
    오브젝트를 재사용함으로써 해당 함수들의 사용을 최소화한다.
    */
    private void Init(int initCount) {
        for (int i = 0; i < initCount; i++) {
            // 음식 큐 초기화
            foodObjectQueue.Enqueue(CreateNewFood("food"));
            // 음료 큐 초기화
            drinkObjectQueue.Enqueue(CreateNewFood("drink"));
        }
    }

    // 재사용할 음료 오브젝트 생성
    private Food CreateNewFood(string type) {
        var newFood = Instantiate(foodObjectPrefab).GetComponent<Food>();
        // 음식 및 음료의 인덱스를 해당 큐의 길이로 지정
        newFood.index = type == "food" ? foodObjectQueue.Count : drinkObjectQueue.Count;
        // 음식인지 음료인지에 대한 타입 지정
        newFood.type = type;
        newFood.gameObject.SetActive(false);
        newFood.transform.SetParent(transform);
        return newFood;
    }

    // 큐에 있는 음식 or 음료 오브젝트 반환
    public Food GetFood(string type) {
        if (type == "food") {
            if (foodObjectQueue.Count > 0) {
                var obj = foodObjectQueue.Dequeue();
                obj.transform.SetParent(null);
                obj.gameObject.SetActive(true);
                return obj;
            }
            else {
                return null;
            }
        }
        else {
            if (drinkObjectQueue.Count > 0) {
                var obj = drinkObjectQueue.Dequeue();
                obj.transform.SetParent(null);
                obj.gameObject.SetActive(true);
                return obj;
            }
            else {
                return null;
            }
        }
    }

    // 사용이 끝난 손님 오브젝트를 다시 큐로 삽입
    public void ReturnFood(Food obj) {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform);
        if (obj.type == "food") {
            foodObjectQueue.Enqueue(obj);
            foodDisplay[obj.index] = -1;
        } else {
            drinkObjectQueue.Enqueue(obj);
            drinkDisplay[obj.index] = -1;
        }
    }
    

    

    IEnumerator FoodSpawn() {
        while (true) {
            if (foodObjectQueue.Count > 0 && !GameManaer.Instance.isGameOver) {
                var food = GetFood("food");
                food.id = Random.Range(1, InGameDataManager.Instance.inGameData.MAX_FOOD_ID);
                foodDisplay[food.index] = food.id;
                food.returnScore = InGameDataManager.Instance.GetFoodScore(food.id);
                food.gainFish = InGameDataManager.Instance.GetFoodGainFish(food.id);
                food.gainCan = InGameDataManager.Instance.GetFoodGainCan(food.id);
                food.GetComponent<SpriteRenderer>().sprite = InGameDataManager.Instance.GetFoodImage(food.id);
                food.GetComponent<SpriteRenderer>().sortingOrder = food.index;
            }
            yield return new WaitForSeconds(0.2f);
        }   
    }

    IEnumerator DrinkSpawn() {
        while (true) {
            // guestObjectQueue에 
            if (drinkObjectQueue.Count > 0 && !GameManaer.Instance.isGameOver) {
                var drink = GetFood("drink");
                drink.id = Random.Range(1, InGameDataManager.Instance.inGameData.MAX_DRINK_ID);
                drinkDisplay[drink.index] = drink.id;
                drink.returnScore = InGameDataManager.Instance.GetDrinkScore(drink.id);
                drink.gainFish = InGameDataManager.Instance.GetDrinkGainFish(drink.id);
                drink.gainCan = InGameDataManager.Instance.GetDrinkGainCan(drink.id);
                drink.GetComponent<SpriteRenderer>().sprite = InGameDataManager.Instance.GetDrinkImage(drink.id);
                drink.GetComponent<SpriteRenderer>().sortingOrder = drink.index;
            }
            yield return new WaitForSeconds(0.1f);
        }   
    }
}
