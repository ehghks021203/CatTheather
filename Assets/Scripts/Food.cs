using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Food : MonoBehaviour {
    public int id { get; set; }         // 음식 id
    public int index { get; set; }      // 몇 번째 칸에서 등장하는지
    public string type { get; set; }    // 음식인지 음료인지

    private float offsetX = -3.0f;
    private float offsetY = -0.9f;
    private float gap = 1.1f;
    private bool canDrag = false;
    private Vector2 createPos;
    [SerializeField] private Guest collidGuest;

    

    private void OnEnable() {
        transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        createPos = type == "food" ? new Vector2(offsetX, offsetY - gap*index) : new Vector2(-offsetX, offsetY - gap*index);
        transform.position = createPos;
        StartCoroutine(CreateFood());
    }

    // 음식을 터치 했을 때
    private void OnMouseDown() {
        if (canDrag) {
            // 이미지의 depth를 15로 설정 (default: 0, 말풍선: 10)
            GetComponent<SpriteRenderer>().sortingOrder = 20;
        }
    }

    // 음식을 드래그 했을 때때
    private void OnMouseDrag() {
        if (canDrag) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
        }
    }

    // 터치가 끝났을 때
    private void OnMouseUp() {
        if (canDrag) {
            if (collidGuest != null) {
                if (type == "food") {
                    Debug.Log(collidGuest.wantFood);
                    if (collidGuest.wantFood == id && !collidGuest.isWantFood) {
                        collidGuest.isWantFood = true;
                        collidGuest.requestFood.color = Color.gray;
                        GameManaer.Instance.PlusScore(200, type);
                        DestroyFood();
                    }
                    else {
                        Debug.Log(id);
                        // 점수 마이너스
                        GameManaer.Instance.MinusScore(type);
                        GetComponent<SpriteRenderer>().sortingOrder = index;
                        transform.position = createPos;
                    }
                }
                else {
                    if (collidGuest.wantDrink == id && !collidGuest.isWantDrink) {
                        collidGuest.isWantDrink = true;
                        collidGuest.requestDrink.color = Color.gray;
                        GameManaer.Instance.PlusScore(200, type);
                        DestroyFood();
                    }
                    else {
                        // 점수 마이너스
                        GameManaer.Instance.MinusScore(type);
                        GetComponent<SpriteRenderer>().sortingOrder = index;
                        transform.position = createPos;
                    }
                }
            }
            else {
                GetComponent<SpriteRenderer>().sortingOrder = index;
                transform.position = createPos;
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D obj) {
        if (obj.gameObject.tag == "Guest") {
            collidGuest = obj.GetComponent<Guest>();
            //obj.GetComponent<Guest>()
        }
    }

    private void OnTriggerExit2D(Collider2D obj) {
        if (obj.gameObject.tag == "Guest") {
            collidGuest = null;
        }
    }

    public void DestroyFood() {
        // 변수 초기화
        canDrag = false;

        // 음식 오브젝트를 FoodList에 다시 돌려줌. (재시용을 위함)
        FoodList.Instance.ReturnFood(this);
    }

    IEnumerator CreateFood() {
        transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.2f);
        canDrag = true;
    }
}