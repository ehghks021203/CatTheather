using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RequireGoodAnimation : MonoBehaviour
{
    [Header("Icon Images")]
    [SerializeField] private Sprite fishIcon;
    [SerializeField] private Sprite canIcon;

    [Header("Require Goods Components")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image icon;
    [SerializeField] private CanvasGroup requireGoodsCG;
    [SerializeField] private TMP_Text requireGoodsTMP;

    private Vector2 screenPoint;
    private float targetPosY;

    public void StartAnimation(int gainGoodsValue, EnumTypes.GoodsType goodsType, float anchorY = 0.0f) {
        Camera uiCamera = transform.GetComponentInParent<Canvas>().worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, uiCamera, out screenPoint);
        screenPoint = screenPoint + new Vector2(0.0f, 50.0f + anchorY);
        targetPosY = screenPoint.y + 100.0f;
        rectTransform.anchoredPosition = screenPoint;
        requireGoodsTMP.text = gainGoodsValue.ToString();
        SetImage(goodsType);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOLocalMoveY(targetPosY, 1.0f)).Join(requireGoodsCG.DOFade(1.0f, 0.5f));
        sequence.Append(requireGoodsCG.DOFade(0.0f, 0.5f));
        sequence.AppendCallback(() => { Destroy(gameObject); });
    }

    private void SetImage(EnumTypes.GoodsType goodsType) {
        switch (goodsType) {
            case EnumTypes.GoodsType.Fish:
                icon.sprite = fishIcon;
                break;
            case EnumTypes.GoodsType.Can:
                icon.sprite = canIcon;
                break;
        }
    }
}
