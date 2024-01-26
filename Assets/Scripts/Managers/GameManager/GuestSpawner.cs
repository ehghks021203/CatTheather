using System.Collections;
using UnityEngine;

public class GuestSpawner : MonoBehaviour {
    [SerializeField] private GameObject [] guestObjectPrefabs;  // 0: 일반 손님, 1~: 스페셜 손님
    public int currentGuestIndex = 1;
    public int totalGuestIndex = 0;
    
    public void SpawnStart() {
        StartCoroutine(GuestSpawn());
    }

    private void CreateGuest(EnumTypes.Rarity rarity) {
        if (rarity == EnumTypes.Rarity.Special) {
            int randIndex = Random.Range(1, guestObjectPrefabs.Length);
            var guest = Instantiate(guestObjectPrefabs[randIndex]);
        }
        else {
            var guest = Instantiate(guestObjectPrefabs[0]);
            var catGuest = guest.GetComponent<NormalCatGuest>();
            switch (rarity) {
                case EnumTypes.Rarity.Hidden:
                    catGuest.data = InGameDTO.Instance.GUEST_DATA[InGameDTO.Instance.HIDDEN_GUEST_ID_LIST[Random.Range(0, InGameDTO.Instance.HIDDEN_GUEST_ID_LIST.Count)]-1];
                    break;
                case EnumTypes.Rarity.Unique:
                    catGuest.data = InGameDTO.Instance.GUEST_DATA[InGameDTO.Instance.UNIQUE_GUEST_ID_LIST[Random.Range(0, InGameDTO.Instance.UNIQUE_GUEST_ID_LIST.Count)]-1];
                    break;
                case EnumTypes.Rarity.Rare:
                    catGuest.data = InGameDTO.Instance.GUEST_DATA[InGameDTO.Instance.RARE_GUEST_ID_LIST[Random.Range(0, InGameDTO.Instance.RARE_GUEST_ID_LIST.Count)]-1];
                    break;
                case EnumTypes.Rarity.Common:
                    catGuest.data = InGameDTO.Instance.GUEST_DATA[InGameDTO.Instance.COMMON_GUEST_ID_LIST[Random.Range(0, InGameDTO.Instance.COMMON_GUEST_ID_LIST.Count)]-1];
                    break;
                default:
                    Debug.LogError("유효하지 않은 레어도 타입");
                    break;
            }
        }
    }

    public void DestroyGuest(GameObject obj, float delay=0.0f) {
        currentGuestIndex++;
        StartCoroutine(DestroyGuestCoroutine(obj, delay));
    }

    IEnumerator GuestSpawn() {
        while (true) {
            if (!GameManager.Instance.isGameOver && totalGuestIndex - currentGuestIndex < 4) {  // 만약 손님 대기열이 다섯 마리보다 작을 경우
                totalGuestIndex++;
                // 일반 손님 생성
                if (RandomCalc.ProbabilityCalc(InGameDTO.Instance.SPECIAL_GUEST_PROBABILITY)) {
                    CreateGuest(EnumTypes.Rarity.Special);
                }
                else if (RandomCalc.ProbabilityCalc(InGameDTO.Instance.HIDDEN_GUEST_PROBABILITY)) {
                    CreateGuest(EnumTypes.Rarity.Hidden);
                }
                else if (RandomCalc.ProbabilityCalc(InGameDTO.Instance.UNIQUE_GUEST_PROBABILITY)) {
                    CreateGuest(EnumTypes.Rarity.Unique);
                }
                else if (RandomCalc.ProbabilityCalc(InGameDTO.Instance.RARE_GUEST_PROBABILITY)) {
                    CreateGuest(EnumTypes.Rarity.Rare);
                }
                else {
                    CreateGuest(EnumTypes.Rarity.Common);
                }
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 2.0f));
        }
    }

    IEnumerator DestroyGuestCoroutine(GameObject obj, float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}