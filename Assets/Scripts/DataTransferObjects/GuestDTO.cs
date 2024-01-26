using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GuestDTO
{
    [field: SerializeField] public int Id { get; set; }
    
    [field: SerializeField] public int GainFish { get; set; }
    [field: SerializeField] public int GainCan { get; set; }
    [field: SerializeField] public int ReturnScore { get; set; }

    [field: SerializeField] public Sprite FrontImage { get; set; }
    [field: SerializeField] public Sprite BackImage { get; set; }
}