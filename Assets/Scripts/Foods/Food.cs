using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Food : MonoBehaviour {
    public FoodDTO data;
    public int index;       // 몇 번째 칸에서 등장하는지

    private float offsetX = -3.0f;
    private float offsetY = -1.1f;
    private float gap = 1.1f;
    private bool canDrag = false;
    private Vector2 createPos;
    [SerializeField] private GameObject collidGuestObj;

    

    private void OnEnable() {
        // 이미지 변경
        GetComponent<SpriteRenderer>().sprite = data.Image;
        GetComponent<SpriteRenderer>().sortingOrder = index;

        transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        createPos = data.FoodType == EnumTypes.FoodType.Food ? new Vector2(offsetX, offsetY - gap*index) : new Vector2(-offsetX, offsetY - gap*index);
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
            if (collidGuestObj != null) {
                Debug.Log("aaa");
                if (collidGuestObj.GetComponent<CatGuest>().ReceiveFoodAndDrink(data)) {
                    GameManager.Instance.ReturnFoodValues(data);
                    DestroyFood();
                }
                else {
                    transform.position = createPos;
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
            //obj.GetComponent<Guest>()
            collidGuestObj = obj.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D obj) {
        if (obj.gameObject.tag == "Guest") {
            collidGuestObj = null;
        }
    }

    public void DestroyFood() {
        // 변수 초기화
        canDrag = false;

        // 음식 오브젝트를 FoodList에 다시 돌려줌. (재시용을 위함)
        GameManager.Instance.foodSpawner.ReturnFood(this);
    }

    IEnumerator CreateFood() {
        transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.2f);
        canDrag = true;
    }
}