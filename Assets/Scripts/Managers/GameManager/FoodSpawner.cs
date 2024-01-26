using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour {
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
    private void Init(int initCount) {
        for (int i = 0; i < initCount; i++) {
            // 음식 큐 초기화
            foodObjectQueue.Enqueue(CreateNewFood(EnumTypes.FoodType.Food));
            // 음료 큐 초기화
            drinkObjectQueue.Enqueue(CreateNewFood(EnumTypes.FoodType.Drink));
        }
    }

    // 재사용할 음료 오브젝트 생성
    private Food CreateNewFood(EnumTypes.FoodType foodType) {
        var newFood = Instantiate(foodObjectPrefab).GetComponent<Food>();
        // 음식 및 음료의 인덱스를 해당 큐의 길이로 지정
        newFood.index = foodType == EnumTypes.FoodType.Food ? foodObjectQueue.Count : drinkObjectQueue.Count;
        newFood.gameObject.SetActive(false);
        newFood.transform.SetParent(transform);
        return newFood;
    }

    // 큐에 있는 음식 or 음료 오브젝트 반환
    public Food GetFood(EnumTypes.FoodType foodType) {
        if (foodType == EnumTypes.FoodType.Food) {
            if (foodObjectQueue.Count > 0) {
                var obj = foodObjectQueue.Dequeue();
                obj.transform.SetParent(null);
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
        if (obj.data.FoodType == EnumTypes.FoodType.Food) {
            foodObjectQueue.Enqueue(obj);
            foodDisplay[obj.index] = 0;
        } else {
            drinkObjectQueue.Enqueue(obj);
            drinkDisplay[obj.index] = 0;
        }
    }

    IEnumerator FoodSpawn() {
        while (true) {
            if (foodObjectQueue.Count > 0 && !GameManager.Instance.isGameOver) {
                var food = GetFood(EnumTypes.FoodType.Food);
                food.data = InGameDTO.Instance.FOOD_DATA[Random.Range(0, InGameDTO.Instance.MAX_FOOD_ID)];
                foodDisplay[food.index] = food.data.Id;
                food.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(0.2f);
        }   
    }

    IEnumerator DrinkSpawn() {
        while (true) {
            // guestObjectQueue에 
            if (drinkObjectQueue.Count > 0 && !GameManager.Instance.isGameOver) {
                var drink = GetFood(EnumTypes.FoodType.Drink);
                drink.data = InGameDTO.Instance.DRINK_DATA[Random.Range(0, InGameDTO.Instance.MAX_DRINK_ID)];
                drinkDisplay[drink.index] = drink.data.Id;
                drink.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(0.1f);
        }   
    }
}