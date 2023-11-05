using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class HandIndicator : MonoBehaviour
{
    private new RectTransform transform;
    [SerializeField] private Image fill;
    [SerializeField] private float rotationOffset = 0;
    [SerializeField] private float targetAlpha = 0.5f;
    [SerializeField] private float fadeInDelay = 0.5f;
    [SerializeField] private float fadeInTime = 1f;
    private bool isLeft = false;
    //private Vector3 targetPosition = Vector3.zero;
    //private Vector3 currentPosition = Vector3.zero;

    private void Awake()
    {
        transform = GetComponent<RectTransform>();
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(targetAlpha, fadeInTime).SetDelay(fadeInDelay);
    }

    public void SetSide(bool isLeft)
    {
        this.isLeft = isLeft;
        transform.localScale = new Vector3(isLeft ? -1 : 1, 1, 1);
        fill.fillClockwise = isLeft;
    }

    public void SetFill(float value)
    {
        fill.fillAmount = value;
    }

    public void EaseFill(float toValue, float duration)
    {
        fill.DOFillAmount(toValue, duration);
    }

    public void SnapTo(Vector3 position, float rotation)
    {
        transform.position = position;
        transform.localEulerAngles = new Vector3(0, 0, isLeft ? rotation - rotationOffset : -rotation + rotationOffset);
        //currentPosition = position;
    }

    // TODO: Smooth movement
}
