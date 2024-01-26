using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;


public abstract class CatGuest : MonoBehaviour {
    [Header("Cat Datas")]
    [SerializeField] public GuestDTO data;

    [Header("Cat Guest Status")]
    public int queueIndex;
    protected bool hasFoodBeenGained;
    protected bool hasDrinkBeenGained;

    [Header("Cat Guest Requirements")]
    public int requireFoodID;
    public int requireDrinkID;
    public int ticketID;

    [Header("Cat Guest Control Values")]
    protected bool isDraggable = false;
    protected int collidDoorNum = 0;
    protected readonly Vector2 orderCounterPos = new(0.0f, -1.75f);
    private readonly float moveSpeed = 8.0f;
    private readonly float foodOrderProbability = 0.4f;
    protected bool showGagebar = false;
    protected bool isGagebarOn = false;
    protected float maxTime = 8.0f;
    protected float curTime = 8.0f;

    [Header("Cat Guest Components and Prefabs")]
    protected CheckFood checkFood;
    [SerializeField] protected SpriteRenderer catSpriteRenderer;
    [SerializeField] protected BoxCollider2D catGuestBoxCollider;
    [SerializeField] protected Rigidbody2D catGuestRigidbody;
    [SerializeField] protected GameObject ticketReqBoxObj;
    [SerializeField] protected GameObject smallReqBoxObj;
    [SerializeField] protected GameObject mediumReqBoxObj;
    [SerializeField] protected GameObject gagebarObj;

    #region UNITY_LIFECYCLE_FUNCTIONS
    protected virtual void Start() {
        Init();
    }

    protected virtual void Update() {
        MoveToOrderCounter();

        if (showGagebar) {
            if (!isGagebarOn) {
                gagebarObj.SetActive(true);
                isGagebarOn = true;
                maxTime = 8.0f - 0.1f * queueIndex;
                curTime = maxTime;
                isGagebarOn = true;
            }
            if (curTime < 0) {
                GameOver();
            }
            curTime -= Time.deltaTime;
            gagebarObj.transform.GetChild(0).GetComponent<Image>().fillAmount = curTime / maxTime;
        }
    }
    #endregion

    #region UNITY_EVENT_FUNCTIONS
    // 손님을 터치 했을 때
    protected virtual void OnMouseDown() {
        if (isDraggable) {
            catSpriteRenderer.sprite = data.FrontImage;
            catSpriteRenderer.sortingOrder = 10;
        }
    }

    // 손님을 드래그 했을 때때
    protected virtual void OnMouseDrag() {
        if (isDraggable) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos + new Vector2(0.0f, 0.15f);
        }
    }

    // 터치가 끝났을 때
    protected virtual void OnMouseUp() {
        if (isDraggable) {
            MoveToTheather();
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
    #endregion

    #region CAT_GUEST_FUNCTIONS
    public virtual void Init() {
        // 음식 체크 컴포넌트 할당
        checkFood = gameObject.GetComponent<CheckFood>();
        transform.position = new Vector2(0.0f, -12.0f);
        queueIndex = GameManager.Instance.guestSpawner.totalGuestIndex;
    
        // 이미지 변경
        catSpriteRenderer.sprite = data.BackImage;
    }

    protected virtual void MoveToOrderCounter()
    {
        if (queueIndex == GameManager.Instance.guestSpawner.currentGuestIndex)
        {
            var targetPos = orderCounterPos;
            if (transform.position.y <= targetPos.y)
            {
                transform.Translate(Vector2.up*Time.deltaTime*moveSpeed);
            }
            // 목표지점까지 도달했다면
            if (transform.position.y >= targetPos.y && !isDraggable)
            {
                DisplayOrderRequirements();
                isDraggable = true;
                catGuestBoxCollider.enabled = true;
            }
        }
        else {
            var targetPos = new Vector2(0.0f, -3.5f - 1.5f * (queueIndex - GameManager.Instance.guestSpawner.currentGuestIndex));
            if (transform.position.y <= targetPos.y) {
                // 위쪽 방향으로 이동, delatTime(1프레임 당 실행시간)를 곱해 성능 별 이동속도에 차이가 없도록 함.
                transform.Translate(Vector2.up*Time.deltaTime*moveSpeed);
                catSpriteRenderer.sortingOrder = queueIndex - GameManager.Instance.guestSpawner.currentGuestIndex;
            }
        }
    }

    protected virtual void DisplayOrderRequirements()
    {
        SetFoodRequirement();
        SetDrinkRequirement();
        SetTicketRequirement();
        SetOrderBoxVisibility();
        showGagebar = true;
    }

    protected virtual void HideOrderRequirements() {
        ticketReqBoxObj.SetActive(false);
        smallReqBoxObj.SetActive(false);
        mediumReqBoxObj.SetActive(false);
        gagebarObj.SetActive(false);
        showGagebar = false;
    }

    protected virtual void SetFoodRequirement() {
        requireFoodID = RandomCalc.ProbabilityCalc(foodOrderProbability) ? GameManager.Instance.foodSpawner.foodDisplay[UnityEngine.Random.Range(0, InGameDTO.Instance.MAX_FOOD_ID)] : 0;
        hasFoodBeenGained = requireFoodID == 0;
    }

    protected virtual void SetDrinkRequirement() {
        requireDrinkID = RandomCalc.ProbabilityCalc(foodOrderProbability) ? GameManager.Instance.foodSpawner.drinkDisplay[UnityEngine.Random.Range(0, InGameDTO.Instance.MAX_DRINK_ID)] : 0;
        hasDrinkBeenGained = requireDrinkID == 0;
    }

    protected virtual void SetTicketRequirement() {
        ticketID = UnityEngine.Random.Range(1, 4);
        ticketReqBoxObj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GetTicketSprite();
    }

    protected Sprite GetTicketSprite() {
        return Resources.Load<Sprite>("Images/Tickets/Ticket" + ticketID.ToString());
    }

    protected void SetOrderBoxVisibility() {
        if (requireFoodID == 0 != (requireDrinkID == 0)) {
            if (requireFoodID != 0) {
                smallReqBoxObj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Foods/Food" + requireFoodID.ToString());
            }
            else {
                smallReqBoxObj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Foods/Drink" + requireDrinkID.ToString());
            }
            smallReqBoxObj.SetActive(true);
        }
        else if (requireFoodID != 0 && requireDrinkID != 0) {
            mediumReqBoxObj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Foods/Food" + requireFoodID.ToString());
            mediumReqBoxObj.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Foods/Drink" + requireDrinkID.ToString());
            mediumReqBoxObj.SetActive(true);
        }
        ticketReqBoxObj.SetActive(true);
    }

    public virtual bool ReceiveFoodAndDrink(FoodDTO data) {
        if (data.FoodType == EnumTypes.FoodType.Food) {
            if (data.Id == requireFoodID) {
                hasFoodBeenGained = true;
                if (requireFoodID == 0 != (requireDrinkID == 0)) {
                    smallReqBoxObj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.gray;
                }
                else {
                    mediumReqBoxObj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.gray;
                }
                return true;
            }
            else {
                // 점수 차감
                GameManager.Instance.ReturnWrongFood();
                curTime = curTime >= 1.0f ? curTime - 1.0f : 0.0f;
                return false;
            }
        }
        else {
            if (data.Id == requireDrinkID) {
                hasDrinkBeenGained = true;
                if (requireFoodID == 0 != (requireDrinkID == 0)) {
                    smallReqBoxObj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.gray;
                }
                else {
                    mediumReqBoxObj.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.gray;
                }
                return true;
            }
            else {
                // 점수 차감
                GameManager.Instance.ReturnWrongFood();
                curTime = curTime >= 1.0f ? curTime - 1.0f : 0.0f;
                return false;
            }
        }
    }

    protected virtual void MoveToTheather() {
        if (collidDoorNum == 0) {
            transform.position = orderCounterPos;
            catSpriteRenderer.sprite = data.BackImage;
            catSpriteRenderer.sortingOrder = 0;
        }
        else if (collidDoorNum == ticketID && hasFoodBeenGained && hasDrinkBeenGained) {
            // 원하는 음식 및 음료를 모두 제공하고 올바른 상영관 입구로 손님을 안내하였다면
            GameManager.Instance.ReturnGuestValues(data);
            // 손님 오브젝트 제거
            GameManager.Instance.guestSpawner.DestroyGuest(obj: gameObject, delay: 0.4f);
            StartCoroutine(DestroyGuest()); // 손님 제거
        }
        else {
            GameOver();
            // GameOver
        }
    }

    protected virtual void GameOver() {
        GameManager.Instance.GameOver();
        HideOrderRequirements();
    }
    #endregion
    
    #region COROUTINE
    // 손님 삭제 함수
    protected virtual IEnumerator DestroyGuest() {
        HideOrderRequirements();
        transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.2f).SetEase(Ease.InOutSine);
        transform.DOMoveY(3.5f, 0.2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.2f);
    }
    #endregion
}