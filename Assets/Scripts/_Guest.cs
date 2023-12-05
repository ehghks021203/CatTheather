using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Guest : MonoBehaviour {
    public int index { get; set; }          // 몇 번째 손님인지
    public int id { get; set; }
    public int gainFish { get; set; }
    public int gainCan { get; set; }
    public int returnScore { get; set; }
    public Sprite frontImg { get; set; }
    public Sprite backImg { get; set; }

    [Header("Guest Values")]
    public int wantFood;      // 손님의 요구 음식
    public int wantDrink;      // 손님의 요구 음료
    public int ticket;       // 손님의 상영관 번호
    [SerializeField] private BoxCollider2D guestBoxCollider;

    [Header("Guest Control Values")]
    public bool isGiveFood = false;
    public bool isGiveDrink = false;
    private bool canDrag = false;
    private int collidDoorNum = 0;

    // 요구사항 말풍선
    [Header("Request Box")]
    [SerializeField] private GameObject ticketReqBoxPrefab;
    [SerializeField] private GameObject reqBoxSPrefab;
    [SerializeField] private GameObject reqBoxMPrefab;
    public SpriteRenderer reqFood;
    public SpriteRenderer reqDrink;

    private void OnEnable() {
        transform.localScale = new Vector2(1.0f, 1.0f);
        transform.position = new Vector2(0.0f, -12.0f);
    }

    private void Update() {
        if (index == GuestList.Instance.currentGuestCount) {
            var targetPos = new Vector2(0.0f, -1.0f);
            Move(targetPos, 8.0f);
            // 목표 지점까지 도달했다면
            if (transform.position.y >= targetPos.y && !canDrag) {
                GameManaer.Instance.showGagebar = true;
                Requirements();
                canDrag = true;
            }
        }
        else {
            var targetPos = new Vector2(0.0f, -2.0f - 1.5f * (index - GuestList.Instance.currentGuestCount));
            Move(targetPos, 8.0f);
        }
    }

    // 손님을 터치 했을 때
    private void OnMouseDown() {
        if (canDrag) {
            GetComponent<SpriteRenderer>().sprite = frontImg;
            GetComponent<SpriteRenderer>().sortingOrder = 10;
        }
    }

    // 손님을 드래그 했을 때때
    private void OnMouseDrag() {
        if (canDrag) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos + new Vector2(0.0f, 0.8f);
        }
    }

    // 터치가 끝났을 때
    private void OnMouseUp() {
        if (canDrag) {
            if (collidDoorNum == 0) {
                transform.position = new Vector2(0.0f, -1.0f);
                GetComponent<SpriteRenderer>().sprite = backImg;
                GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
            else if (collidDoorNum == ticket && isGiveFood && isGiveDrink) {
                // 원하는 음식 및 음료를 모두 제공하고 올바른 상영관 입구로 손님을 안내하였다면
                GameManaer.Instance.showGagebar = false;
                GameManaer.Instance.PlusScore(returnScore, "ticket");   // 점수 증가
                GameManaer.Instance.GainFish(gainFish);
                GameManaer.Instance.GainCan(gainCan);
                StartCoroutine(DestroyGuest()); // 손님 제거

                
            }
            else {
                GameManaer.Instance.isGameOver = true;
                Debug.Log("Game Over");
                // GameOver
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D obj) {
        if (obj.gameObject.tag == "Door") {
            // 어느 입구 위에 올라가있는지 체크 (변수에 저장)
            collidDoorNum = int.Parse(obj.gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D obj) {
        if (obj.gameObject.tag == "Door") {
            // 입구에서 벗어났을 때 값 초기화
            collidDoorNum = 0;
        }
    }


    // 손님 이동 함수
    private void Move(Vector2 targetPos, float speed) {
        // 목표 지점까지 덜 도달했다면
        if (transform.position.y <= targetPos.y) {
            // 위쪽 방향으로 이동, delatTime(1프레임 당 실행시간)를 곱해 성능 별 이동속도에 차이가 없도록 함.
            transform.Translate(Vector2.up * Time.deltaTime * speed);
        }
    }

    // 손님 요구사항 설정 및 표시 함수
    private void Requirements() {
        // 원하는 음식 및 음료 세팅 (null: -1)
        // 3분의 2 확률로 음식 주문
        if (Random.Range(0, 3) != 0) {
            // 음식 진열대에 올라와 있는 음식 중 하나 랜덤 선택
            wantFood = FoodList.Instance.foodDisplay[Random.Range(0, InGameDataManager.Instance.inGameData.MAX_FOOD_ID)];
            isGiveFood = false;                 // 음식 주문이 있기 때문에 false로 설정
        } else {
            wantFood = 0;
            isGiveFood = true;          // 음식 주문이 없기 때문에 true로 설정
        }
        // 3분의 2 확률로 음료 주문
        if (Random.Range(0, 3) != 0) {
            // 음료 진열대에 올라와 있는 음료 중 하나 랜덤 선택
            wantDrink = FoodList.Instance.drinkDisplay[Random.Range(0, InGameDataManager.Instance.inGameData.MAX_DRINK_ID)];
            isGiveDrink = false;
        } else {
            wantDrink = 0;
            isGiveDrink = true;
        }
        // 손님의 티켓 번호 이미지 설정
        ticketReqBoxPrefab.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Tickets/Ticket" + ticket.ToString());

        // 말풍선 위에 요구사항 이미지 위치 설정
        if (wantFood == 0 && wantDrink == 0) {
            // 티켓만 표시
            ticketReqBoxPrefab.SetActive(true);
        } else if (wantFood == 0) {
            // 티켓과 음료 표시
            reqDrink = reqBoxSPrefab.transform.GetChild(0).GetComponent<SpriteRenderer>();
            reqDrink.sprite = Resources.Load<Sprite>("Images/Foods/Drink" + wantDrink.ToString());
            reqDrink.color = Color.white;
            reqBoxSPrefab.SetActive(true);
            ticketReqBoxPrefab.SetActive(true);
        } else if (wantDrink == 0) {
            // 티켓과 음식 표시
            reqFood = reqBoxSPrefab.transform.GetChild(0).GetComponent<SpriteRenderer>();
            reqFood.sprite = Resources.Load<Sprite>("Images/Foods/Food" + wantFood.ToString());
            reqFood.color = Color.white;
            reqBoxSPrefab.SetActive(true);
            ticketReqBoxPrefab.SetActive(true);
        } else {
            // 모두 표시
            reqFood = reqBoxMPrefab.transform.GetChild(0).GetComponent<SpriteRenderer>();
            reqFood.sprite = Resources.Load<Sprite>("Images/Foods/Food" + wantFood.ToString());
            reqFood.color = Color.white;
            reqDrink = reqBoxMPrefab.transform.GetChild(1).GetComponent<SpriteRenderer>();
            reqDrink.sprite = Resources.Load<Sprite>("Images/Foods/Drink" + wantDrink.ToString());
            reqDrink.color = Color.white;
            reqBoxMPrefab.SetActive(true);
            ticketReqBoxPrefab.SetActive(true);
        }
        // 손님 음식 받을 준비 완료 (박스콜라이더 활성화)
        guestBoxCollider.enabled = true;
    }


    // 손님 삭제 함수
    IEnumerator DestroyGuest() {
        ticketReqBoxPrefab.SetActive(false);
        reqBoxSPrefab.SetActive(false);
        reqBoxMPrefab.SetActive(false);
        transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.2f).SetEase(Ease.InOutSine);
        transform.DOMoveY(3.5f, 0.2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.2f);
        // 변수 초기화
        canDrag = false;
        collidDoorNum = 0;
        
        guestBoxCollider.enabled = false;
        
        // 손님 오브젝트를 GuestList에 다시 돌려줌. (재시용을 위함)
        GuestList.Instance.ReturnGuest(this);
    }
}