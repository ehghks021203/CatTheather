using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestList : MonoBehaviour {
    public static GuestList Instance { get; private set;}
    void Awake() => Instance = this;

    [SerializeField] private GameObject guestObjectPrefab;
    Queue<Guest> guestObjectQueue = new Queue<Guest>();

    public int currentGuestCount = 0;
    public int totalGuestCount = 0;
    
    
    public void SpawnStart() {
        Init(5);
        StartCoroutine(GuestSpawn());
    }

    // 재사용할 손님 오브젝트 일정 수 만큼 생성
    /* 
    Instantiate()와 Destroy()는 꽤 무거운 작업이기 때문에, 
    오브젝트를 재사용함으로써 해당 함수들의 사용을 최소화한다.
    */
    private void Init(int initCount) {
        for (int i = 0; i < initCount; i++) {
            guestObjectQueue.Enqueue(CreateNewGuest());
        }
    }

    // 재사용할 손님 오브젝트 생성
    private Guest CreateNewGuest() {
        var newGuest = Instantiate(guestObjectPrefab).GetComponent<Guest>();
        newGuest.gameObject.SetActive(false);
        newGuest.transform.SetParent(transform);
        return newGuest;
    }

    // 큐에 있는 손님 오브젝트 반환
    public Guest GetGuest() {
        if (guestObjectQueue.Count > 0) {
            var obj = guestObjectQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else {
            print("Too Many Guest!");
            return null;
        }
    }

    // 사용이 끝난 손님 오브젝트를 다시 큐로 삽입
    public void ReturnGuest(Guest obj) {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform);
        guestObjectQueue.Enqueue(obj);
        currentGuestCount++;
    }

    

    IEnumerator GuestSpawn() {
        while (true) {
            // guestObjectQueue에 
            if (guestObjectQueue.Count > 0 && !GameManaer.Instance.isGameOver) {
                var guest = GetGuest();
                guest.transform.position = new Vector3(0.0f, -12.0f, 1.0f);
                guest.index = totalGuestCount;      // 손님의 번호 부여
                guest.id = Random.Range(1, InGameDataManager.Instance.inGameData.MAX_GUEST_ID);
                guest.ticket = Random.Range(1, 4);
                guest.gainFish = InGameDataManager.Instance.GetGuestGainFish(guest.id);
                guest.gainCan = InGameDataManager.Instance.GetGuestGainCan(guest.id);
                guest.returnScore = InGameDataManager.Instance.GetGuestScore(guest.id);
                guest.frontImg = InGameDataManager.Instance.GetGuestFrontImage(guest.id);
                guest.backImg = InGameDataManager.Instance.GetGuestBackImage(guest.id);
                guest.GetComponent<SpriteRenderer>().sprite = InGameDataManager.Instance.GetGuestBackImage(guest.id);
                guest.GetComponent<SpriteRenderer>().sortingOrder = guest.index;
                totalGuestCount++;                  // 총 손님 수 + 1
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 2.0f));
        }
        
    }
}
