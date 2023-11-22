using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guest : MonoBehaviour {
    public int index { get; set; }          // 몇 번째 손님인지
    public int id { get; set; }
    public int wantFood { get; set; }       // 손님의 요구 음식
    public int wantDrink { get; set; }      // 손님의 요구 음료
    public int ticket { get; set; }         // 손님의 상영관 번호
    public int gainFish { get; set; }
    public int gainCan { get; set; }
    public int returnScore { get; set; }

    public Sprite frontImg { get; set; }
    public Sprite backImg { get; set; }

    public bool isWantFood = false;
    public bool isWantDrink = false;
    private bool canDrag = false;
    private int collidDoorNum = 0;


    // 요구사항 말풍선
    [SerializeField] private GameObject requestBoxPrefab;
    [SerializeField] private SpriteRenderer requestTicket;
    public SpriteRenderer requestFood;
    public SpriteRenderer requestDrink;


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
            GetComponent<SpriteRenderer>().sprite = backImg;
            GetComponent<SpriteRenderer>().sortingOrder = 10;
        }
    }

    // 손님을 드래그 했을 때때
    private void OnMouseDrag() {
        if (canDrag) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
        }
    }

    // 터치가 끝났을 때
    private void OnMouseUp() {
        if (canDrag) {
            if (collidDoorNum == 0) {
                transform.position = new Vector2(0.0f, -1.0f);
                GetComponent<SpriteRenderer>().sprite = frontImg;
                GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
            else if (collidDoorNum == ticket && isWantFood && isWantDrink) {
                // 원하는 음식 및 음료를 모두 제공하고 올바른 상영관 입구로 손님을 안내하였다면
                GameManaer.Instance.showGagebar = false;
                GameManaer.Instance.PlusScore(returnScore, "ticket");   // 점수 증가
                GameManaer.Instance.GainFish(gainFish);
                GameManaer.Instance.GainCan(gainCan);
                DestroyGuest(); // 손님 제거

                
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
            requestFood.sprite = InGameDataManager.Instance.GetFoodImage(wantFood);
            requestFood.color = Color.white;
            isWantFood = false;                 // 음식 주문이 있기 때문에 false로 설정
        } else {
            wantFood = -1;
            requestFood.sprite = null;  // 요구 음식 이미지를 null로 설정
            isWantFood = true;          // 음식 주문이 없기 때문에 true로 설정
        }
        // 3분의 2 확률로 음료 주문
        if (Random.Range(0, 3) != 0) {
            // 음료 진열대에 올라와 있는 음료 중 하나 랜덤 선택
            wantDrink = FoodList.Instance.drinkDisplay[Random.Range(0, InGameDataManager.Instance.inGameData.MAX_DRINK_ID)];
            requestDrink.sprite = InGameDataManager.Instance.GetDrinkImage(wantDrink);
            requestDrink.color = Color.white;
            isWantDrink = false;
        } else {
            wantDrink = -1;
            requestDrink.sprite = null;
            isWantDrink = true;
        }
        // 손님의 티켓 번호 이미지 설정
        requestTicket.sprite = Resources.Load<Sprite>("Images/Tickets/Ticket" + ticket.ToString());
        
        // 말풍선 위에 요구사항 이미지 위치 설정
        if (wantFood == -1 && wantDrink == -1) {
            requestTicket.transform.position = new Vector2(2.2f, 1.05f);
        } else if (wantFood == -1) {
            requestDrink.transform.position = new Vector2(1.6f, 1.05f);
            requestTicket.transform.position = new Vector2(2.8f, 1.05f);
        } else if (wantDrink == -1) {
            requestFood.transform.position = new Vector2(1.6f, 1.05f);
            requestTicket.transform.position = new Vector2(2.8f, 1.05f);
        } else {
            requestFood.transform.position = new Vector2(1.0f, 1.05f);
            requestDrink.transform.position = new Vector2(2.2f, 1.05f);
            requestTicket.transform.position = new Vector2(3.4f, 1.05f);
        }

        // 요구사항 말풍선 표시
        requestBoxPrefab.SetActive(true);
    }


    // 손님 삭제 함수
    private void DestroyGuest() {
        // 변수 초기화
        canDrag = false;
        collidDoorNum = 0;
        requestBoxPrefab.SetActive(false);
        
        // 손님 오브젝트를 GuestList에 다시 돌려줌. (재시용을 위함)
        GuestList.Instance.ReturnGuest(this);
    }
}