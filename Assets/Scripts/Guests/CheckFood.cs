using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFood : MonoBehaviour
{
    public int foodID;
    public int drinkID;
    public bool hasFoodBeenGained;
    public bool hasDrinkBeenGained;

    public void Init(int foodID, int drinkID) {
        this.foodID = foodID;
        this.drinkID = drinkID;
    }

    public bool GainFood(FoodDTO data) {
        if (data.FoodType == EnumTypes.FoodType.Food) {
            if (data.Id == foodID) {
                
                return true;
            }
            else {
                // 점수 차감
                return false;
            }
        }
        else {
            return data.Id == drinkID;
        }
    }
}
