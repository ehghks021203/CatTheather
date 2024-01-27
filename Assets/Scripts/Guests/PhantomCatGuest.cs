using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PhantomCatGuest : CatGuest
{
    protected int specialID = 7;
    protected bool isAlive = true;
    protected Vector2 touchPos;
    protected Vector2 unTouchPos;
    protected float throwThreshold = -4.0f;
    protected float throwSpeed = 0.0f;

    #region UNITY_LIFECYCLE_FUNCTIONS
    protected override void Update() {
        base.Update();
        if (!isAlive) {
            catSpriteRenderer.sortingOrder = 10;
            if(unTouchPos.x<=0) {
                gameObject.transform.Rotate(0,0,Time.deltaTime*450);
            }
            else {
                gameObject.transform.Rotate(0,0,(-1)*Time.deltaTime*450);
            }
        }
    }
    #endregion

    #region UNITY_EVENT_FUNCTIONS
    protected override void OnMouseDown() {
        base.OnMouseDown();
        touchPos = transform.position;
    }

    protected override void OnMouseUp() {
        base.OnMouseUp();
        unTouchPos = transform.position;
        if (transform.position.y <= throwThreshold) {
            StartCoroutine(BeThrown());
        }
    }
    #endregion

    #region CAT_GUEST_FUNCTIONS
    public override void Init() {
        data = InGameDTO.Instance.GUEST_DATA[specialID - 1];
        base.Init();
    }

    protected override void DisplayOrderRequirements() {
        base.DisplayOrderRequirements();
        catSpriteRenderer.sprite = data.FrontImage;
    }

    protected override void MoveToTheather() {
        if (collidDoorNum == 0 && transform.position.y > throwThreshold) {
            transform.position = GameManager.Instance.guestSpawner.orderCounterPos;
            catSpriteRenderer.sprite = data.FrontImage;
            catSpriteRenderer.sortingOrder = 0;
        }
        else if (collidDoorNum != 0) {
            GameOver();
            // GameOver
        }
    }

    public override bool ReceiveFoodAndDrink(FoodDTO data) {
        Debug.Log("ReceiveFoodAndDrink");
        GameOver();
        return true;
    }
    #endregion
    

    
    #region COROUTINE
    protected IEnumerator BeThrown() {
        isAlive = false;
        isDraggable = false;
        catGuestBoxCollider.enabled = false;
        HideOrderRequirements();
        throwSpeed = Vector2.Distance(unTouchPos, touchPos)/2;
        transform.DOMove(unTouchPos*throwSpeed, 1.0f);
        // 손님 오브젝트 제거
        GameManager.Instance.guestSpawner.DestroyGuest(obj: gameObject, delay: 1.3f);
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(DestroyGuest());
        yield return null;
    }

    // 손님 삭제 함수
    protected override IEnumerator DestroyGuest() {
        transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.2f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(0.2f);
    }
    #endregion
}