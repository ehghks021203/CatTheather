using System.Collections.Generic;

public class InGameDTO : Singleton<InGameDTO> 
{
    public List<FoodDTO> FOOD_DATA { get; set; }
    public int MAX_FOOD_ID { get; set; }
    public List<FoodDTO> DRINK_DATA { get; set; }
    public int MAX_DRINK_ID { get; set; }
    public List<GuestDTO> GUEST_DATA { get; set; }
    public List<int> COMMON_GUEST_ID_LIST { get; set; }
    public List<int> RARE_GUEST_ID_LIST { get; set; }
    public List<int> UNIQUE_GUEST_ID_LIST { get; set; }
    public List<int> HIDDEN_GUEST_ID_LIST { get; set; }
    public List<int> SPECIAL_GUEST_ID_LIST { get; set; }
    public float RARE_GUEST_PROBABILITY = 0.30f;
    public float UNIQUE_GUEST_PROBABILITY = 0.10f;
    public float HIDDEN_GUEST_PROBABILITY = 0.05f;
    public float SPECIAL_GUEST_PROBABILITY = 0.10f;
}
