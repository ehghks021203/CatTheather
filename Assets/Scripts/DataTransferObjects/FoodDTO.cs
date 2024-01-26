using UnityEngine;

[System.Serializable]
public class FoodDTO 
{
    [field: SerializeField] public int Id { get; set; }
    [field: SerializeField] public EnumTypes.FoodType FoodType {get; set;}

    [field: SerializeField] public int GainFish { get; set; }
    [field: SerializeField] public int GainCan { get; set; }
    [field: SerializeField] public int ReturnScore { get; set; }

    [field: SerializeField] public Sprite Image { get; set; }
}